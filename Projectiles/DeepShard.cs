using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepShard : ModProjectile
    {
        private const int DelayTicks = 60;
        private const float RushSpeed = 18f;
        private const int ExplosionRadius = 70;
        private const int ExplosionDamage = 10000;
        private const int DetonationRememberTicks = 30;
        private const float SameCenterTolerance = 10f;
        private static readonly List<DetonationEntry> RecentDetonations = new();
        private bool exploded;
        private bool launched;
        private bool idleAnchorInitialized;
        private bool switchedToConverge;
        private int trackedNpcId = -1;
        private Vector2 centerMarker;
        private Vector2 warmupBaseOffset;

        private readonly struct DetonationEntry
        {
            public readonly Vector2 Center;
            public readonly uint Tick;

            public DetonationEntry(Vector2 center, uint tick)
            {
                Center = center;
                Tick = tick;
            }
        }

        public override string Texture => "Etobudet1modtipo/Projectiles/DeepShard1";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void AI()
        {
            if (!idleAnchorInitialized)
            {
                idleAnchorInitialized = true;
                trackedNpcId = (int)Projectile.ai[0];
                centerMarker = ResolveInitialCenterMarker();
                if (trackedNpcId < 0 || trackedNpcId >= Main.maxNPCs)
                {
                    trackedNpcId = FindClosestHostileToPoint(centerMarker, 180f);
                }
                warmupBaseOffset = Projectile.Center - centerMarker;
                if (warmupBaseOffset.LengthSquared() < 4f)
                {
                    warmupBaseOffset = new Vector2(100f, 0f).RotatedBy(Projectile.whoAmI * 1.3f);
                }
            }

            NPC liveTarget = GetTrackedNpc();
            if (liveTarget != null)
            {
                centerMarker = liveTarget.Center;
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= DelayTicks)
            {
                Vector2 chaseTarget = centerMarker;
                if (liveTarget != null)
                {
                    chaseTarget = liveTarget.Center;
                }

                float orbitAngle = Projectile.localAI[0] * 0.05f;
                Vector2 warmupGoal = chaseTarget + warmupBaseOffset.RotatedBy(orbitAngle);
                Vector2 desiredChase = (warmupGoal - Projectile.Center).SafeNormalize(Vector2.UnitY) * (RushSpeed * 0.75f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredChase, 0.25f);
                SpawnIdleDust();
                return;
            }

            if (!launched)
            {
                launched = true;
            }

            if (!switchedToConverge)
            {
                switchedToConverge = true;
                Projectile.velocity = Vector2.Zero;
            }

            Vector2 targetCenter = centerMarker;
            Vector2 desired = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitY) * RushSpeed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.35f);
            SpawnFlightDust();

            if (TryGetSiblingCollisionPoint(out Vector2 collisionPoint))
            {
                Explode(collisionPoint);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (exploded)
            {
                return;
            }

            SpawnExplosionDust();
        }

        private NPC GetTrackedNpc()
        {
            if (trackedNpcId < 0 || trackedNpcId >= Main.maxNPCs)
            {
                return null;
            }

            NPC npc = Main.npc[trackedNpcId];
            if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.life <= 0)
            {
                return null;
            }

            return npc;
        }

        private static int FindClosestHostileToPoint(Vector2 point, float maxDistance)
        {
            int best = -1;
            float bestDist = maxDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.life <= 0)
                {
                    continue;
                }

                float dist = Vector2.Distance(point, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = i;
                }
            }

            return best;
        }

        private bool TryGetSiblingCollisionPoint(out Vector2 collisionPoint)
        {
            const float collideDistance = 18f;
            int myTargetId = (int)Projectile.ai[0];
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (!other.active || other.whoAmI == Projectile.whoAmI || other.owner != Projectile.owner)
                {
                    continue;
                }

                if (other.ModProjectile is not DeepShard)
                {
                    continue;
                }

                if ((int)other.ai[0] != myTargetId)
                {
                    continue;
                }

                if (Vector2.Distance(Projectile.Center, other.Center) > collideDistance)
                {
                    continue;
                }

                collisionPoint = (Projectile.Center + other.Center) * 0.5f;
                return true;
            }

            collisionPoint = default;
            return false;
        }

        private void Explode(Vector2 impactCenter)
        {
            if (exploded)
            {
                return;
            }

            exploded = true;
            Projectile.Center = impactCenter;
            Projectile.velocity = Vector2.Zero;

            bool firstDetonation = TryRegisterDetonation(impactCenter);
            if (!firstDetonation)
            {
                Projectile.Kill();
                return;
            }

            SoundStyle blastSound = new SoundStyle("Etobudet1modtipo/Sounds/ConetionBell")
            {
                Volume = 1f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(blastSound, Projectile.Center);
            AddImpactShake();
            KillSiblingShards(impactCenter);
            SpawnExplosionAnimation(impactCenter);

            SpawnExplosionDust();
            Projectile.Kill();
        }

        private static bool TryRegisterDetonation(Vector2 center)
        {
            uint now = Main.GameUpdateCount;
            for (int i = RecentDetonations.Count - 1; i >= 0; i--)
            {
                if (now - RecentDetonations[i].Tick > DetonationRememberTicks)
                {
                    RecentDetonations.RemoveAt(i);
                }
            }

            for (int i = 0; i < RecentDetonations.Count; i++)
            {
                if (Vector2.Distance(RecentDetonations[i].Center, center) <= SameCenterTolerance)
                {
                    return false;
                }
            }

            RecentDetonations.Add(new DetonationEntry(center, now));
            return true;
        }

        private void KillSiblingShards(Vector2 center)
        {
            int myTargetId = (int)Projectile.ai[0];
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (!other.active || other.whoAmI == Projectile.whoAmI || other.owner != Projectile.owner)
                {
                    continue;
                }

                if (other.ModProjectile is not DeepShard)
                {
                    continue;
                }

                if ((int)other.ai[0] != myTargetId &&
                    Vector2.Distance(other.Center, center) > SameCenterTolerance)
                {
                    continue;
                }

                other.Kill();
            }
        }

        private Vector2 ResolveInitialCenterMarker()
        {
            const float offset = 100f;
            if (Projectile.type == ModContent.ProjectileType<DeepShard1>())
            {
                return Projectile.Center + new Vector2(offset, offset);
            }

            if (Projectile.type == ModContent.ProjectileType<DeepShard2>())
            {
                return Projectile.Center + new Vector2(-offset, offset);
            }

            if (Projectile.type == ModContent.ProjectileType<DeepShard3>())
            {
                return Projectile.Center + new Vector2(-offset, -offset);
            }

            if (Projectile.type == ModContent.ProjectileType<DeepShard4>())
            {
                return Projectile.Center + new Vector2(offset, -offset);
            }

            return Projectile.Center;
        }

        private void AddImpactShake()
        {
            if (Main.dedServ)
            {
                return;
            }

            Player localPlayer = Main.LocalPlayer;
            float maxDistance = 900f;
            float distance = Vector2.Distance(localPlayer.Center, Projectile.Center);
            if (distance > maxDistance)
            {
                return;
            }

            float strength = MathHelper.Lerp(18f, 5f, distance / maxDistance);
            Vector2 direction = Main.rand.NextVector2Unit();
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(
                Projectile.Center,
                direction,
                strength,
                10f,
                34,
                1000f,
                "DeepShardImpact"));
        }

        private void SpawnIdleDust()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonWater, Vector2.Zero, 140, default, 0.95f);
                d.noGravity = true;
            }
        }

        private void SpawnFlightDust()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonWater, -Projectile.velocity * 0.12f, 90, default, 1.1f);
            d.noGravity = true;
        }

        private void SpawnExplosionDust()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonWater, vel, 70, default, 1.3f);
                d.noGravity = true;
            }
        }

        private void SpawnExplosionAnimation(Vector2 center)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int projIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                center,
                Vector2.Zero,
                ModContent.ProjectileType<DeepSeaExplotion>(),
                ExplosionDamage,
                0f,
                Projectile.owner,
                ExplosionRadius);

            if (projIndex >= 0 && projIndex < Main.maxProjectiles)
            {
                Main.projectile[projIndex].Center = center;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(80, 210, 255, 0) * (0.6f * progress);
                float scale = Projectile.scale * (0.82f + 0.18f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, trailColor, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            Color glow = new Color(160, 240, 255, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, glow, Projectile.rotation, origin, Projectile.scale * 1.08f, SpriteEffects.None, 0);
            return true;
        }
    }

    public class DeepShard1 : DeepShard
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/DeepShard1";
    }

    public class DeepShard2 : DeepShard
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/DeepShard2";
    }

    public class DeepShard3 : DeepShard
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/DeepShard3";
    }

    public class DeepShard4 : DeepShard
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/DeepShard4";
    }
}
