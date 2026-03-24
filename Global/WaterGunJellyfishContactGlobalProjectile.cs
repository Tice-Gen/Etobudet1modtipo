using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Global
{
    public class WaterGunJellyfishContactGlobalProjectile : GlobalProjectile
    {
        private const float ContactLineWidth = 18f;
        private const float OwnerAssistLineWidth = 20f;
        private const float ForwardProbeLength = 8f;
        private const float OwnerAssistMaxDistance = 48f * 16f;

        public override bool InstancePerEntity => false;

        public override void AI(Projectile projectile)
        {
            if (!projectile.active || !IsWaterGunProjectile(projectile.type))
            {
                return;
            }

            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player owner = Main.player[projectile.owner];
            if (owner == null || !owner.active)
            {
                return;
            }

            Vector2 lineStart = projectile.oldPosition + projectile.Size * 0.5f;
            Vector2 lineEnd = projectile.Center;
            Vector2 forwardEnd = lineEnd + projectile.velocity.SafeNormalize(Vector2.Zero) * ForwardProbeLength;
            bool canUseOwnerAssistLine = Vector2.DistanceSquared(owner.MountedCenter, lineEnd) <= OwnerAssistMaxDistance * OwnerAssistMaxDistance;

            int jellyType = ModContent.NPCType<JellyFish>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc == null || !npc.active || npc.type != jellyType)
                {
                    continue;
                }

                bool touches = projectile.Hitbox.Intersects(npc.Hitbox) ||
                               CheckLineTouch(npc, lineStart, lineEnd, ContactLineWidth) ||
                               CheckLineTouch(npc, lineEnd, forwardEnd, ContactLineWidth);

                if (!touches && canUseOwnerAssistLine)
                {
                    touches = CheckLineTouch(npc, owner.MountedCenter, lineEnd, OwnerAssistLineWidth);
                }

                if (!touches)
                {
                    continue;
                }

                if (npc.ModNPC is JellyFish jelly)
                {
                    jelly.RegisterWaterGunHitFromProjectile(projectile);
                }
            }
        }

        private static bool IsWaterGunProjectile(int projectileType)
        {
            return projectileType == ProjectileID.WaterGun || projectileType == ProjectileID.WaterStream;
        }

        private static bool CheckLineTouch(NPC npc, Vector2 lineStart, Vector2 lineEnd, float width)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(npc.position, npc.Size, lineStart, lineEnd, width, ref collisionPoint);
        }
    }
}
