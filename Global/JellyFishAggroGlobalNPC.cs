using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Global
{
    public class JellyFishAggroGlobalNPC : GlobalNPC
    {
        private const int DangerousTargetLifeThreshold = 1000;
        private const float MaxAggroDistance = 30f * 16f;
        private const float AggroSwitchMargin = 12f;
        private const float SharedAggroRadius = 30f * 16f;
        private const int AggroDurationTicks = 12 * 60;
        private const float GroundPullSpeed = 2.2f;
        private const float AirPullSpeed = 3.1f;
        private const float GroundPullLerp = 0.075f;
        private const float AirPullLerp = 0.11f;
        private const float JumpToTargetSpeed = 4.8f;

        private int aggroSourceJellyWhoAmI = -1;
        private Vector2 aggroClusterAnchor;
        private int aggroTimer;

        public override bool InstancePerEntity => true;

        public void RegisterHitByJellyfish(NPC jelly)
        {
            int jellyType = ModContent.NPCType<JellyFish>();
            if (jelly == null || !jelly.active || jelly.type != jellyType)
            {
                return;
            }

            aggroSourceJellyWhoAmI = jelly.whoAmI;
            aggroClusterAnchor = jelly.Center;
            aggroTimer = AggroDurationTicks;
        }

        public override void PostAI(NPC npc)
        {
            if (aggroTimer > 0)
            {
                aggroTimer--;
            }

            if (!TryGetJellyFishAggroTarget(npc, out NPC jelly))
            {
                return;
            }

            Vector2 toJelly = jelly.Center - npc.Center;
            Vector2 direction = toJelly.SafeNormalize(new Vector2(npc.direction == 0 ? 1f : npc.direction, 0f));

            float speed = npc.noGravity ? AirPullSpeed : GroundPullSpeed;
            float lerp = npc.noGravity ? AirPullLerp : GroundPullLerp;
            Vector2 desiredVelocity = direction * speed;
            npc.velocity = Vector2.Lerp(npc.velocity, desiredVelocity, lerp);

            if (direction.X > 0.05f)
            {
                npc.direction = 1;
                npc.spriteDirection = 1;
            }
            else if (direction.X < -0.05f)
            {
                npc.direction = -1;
                npc.spriteDirection = -1;
            }

            if (!npc.noGravity && npc.collideX && npc.velocity.Y == 0f && toJelly.Y < -8f)
            {
                npc.velocity.Y = -JumpToTargetSpeed;
            }
        }

        private bool TryGetJellyFishAggroTarget(NPC npc, out NPC jellyTarget)
        {
            jellyTarget = null;

            if (npc == null ||
                !npc.active ||
                npc.life <= 0 ||
                npc.dontTakeDamage ||
                npc.friendly ||
                npc.townNPC ||
                npc.boss ||
                npc.damage <= 0 ||
                aggroTimer <= 0)
            {
                return false;
            }

            if (npc.lifeMax > DangerousTargetLifeThreshold && npc.damage > 0)
            {
                return false;
            }

            int jellyType = ModContent.NPCType<JellyFish>();
            if (npc.type == jellyType)
            {
                return false;
            }

            float nearestPlayerDist = float.MaxValue;
            bool hasActivePlayer = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player == null || !player.active || player.dead)
                {
                    continue;
                }

                hasActivePlayer = true;
                float dist = Vector2.Distance(npc.Center, player.Center);
                if (dist < nearestPlayerDist)
                {
                    nearestPlayerDist = dist;
                }
            }

            Vector2 clusterCenter = aggroClusterAnchor;
            if (aggroSourceJellyWhoAmI >= 0 && aggroSourceJellyWhoAmI < Main.maxNPCs)
            {
                NPC sourceJelly = Main.npc[aggroSourceJellyWhoAmI];
                if (sourceJelly != null && sourceJelly.active && sourceJelly.type == jellyType)
                {
                    clusterCenter = sourceJelly.Center;
                    aggroClusterAnchor = clusterCenter;
                }
            }

            float clusterRadiusSq = SharedAggroRadius * SharedAggroRadius;
            NPC bestJelly = null;
            float bestJellyDist = MaxAggroDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (candidate == null || !candidate.active || candidate.type != jellyType)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(candidate.Center, clusterCenter) > clusterRadiusSq)
                {
                    continue;
                }

                if (!Collision.CanHitLine(npc.Center, 1, 1, candidate.Center, 1, 1))
                {
                    continue;
                }

                float dist = Vector2.Distance(npc.Center, candidate.Center);
                if (dist < bestJellyDist)
                {
                    bestJellyDist = dist;
                    bestJelly = candidate;
                }
            }

            if (bestJelly == null)
            {
                return false;
            }

            if (!hasActivePlayer || bestJellyDist + AggroSwitchMargin < nearestPlayerDist)
            {
                jellyTarget = bestJelly;
                return true;
            }

            return false;
        }
    }
}
