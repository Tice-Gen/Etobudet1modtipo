using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class AdamantiteShard : ModProjectile
    {
        private const float OrbitRadius = 54f;
        private const float OrbitSpeed = 0.11f;
        private const float LaunchSpeed = 20f;
        private const int LaunchLifetime = 180;
        private const float FlightSpinSpeed = 0.45f;
        private const float OrbitSmoothFactor = 0.2f;
        private bool dealtDamage;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            if (Projectile.ai[2] == 1f)
            {
                Projectile.penetrate = 1;
                UpdateLaunchedState();
                return;
            }

            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
            {
                LaunchWithoutParent();
                return;
            }

            Projectile parent = Main.projectile[parentIndex];
            if (!parent.active || parent.type != ModContent.ProjectileType<AdamantiteYoyoProj>() || parent.owner != Projectile.owner)
            {
                LaunchWithoutParent();
                return;
            }



            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            ApplySymmetricOrbit(parent);
        }

        private void UpdateLaunchedState()
        {
            if (Projectile.velocity.LengthSquared() < 0.01f)
                Projectile.velocity = Vector2.UnitY * LaunchSpeed;

            Projectile.rotation += FlightSpinSpeed;
        }

        private void LaunchAtTarget(Vector2 targetCenter)
        {
            Vector2 toTarget = targetCenter - Projectile.Center;
            Vector2 direction = toTarget.SafeNormalize(Vector2.UnitY);

            Projectile.ai[2] = 1f;
            Projectile.velocity = direction * LaunchSpeed;
            Projectile.timeLeft = LaunchLifetime;
            Projectile.netUpdate = true;
        }

        private void LaunchWithoutParent()
        {
            Projectile.ai[2] = 1f;
            if (Projectile.velocity.LengthSquared() < 0.01f)
                Projectile.velocity = Main.rand.NextVector2Unit() * LaunchSpeed;
            Projectile.timeLeft = LaunchLifetime;
            Projectile.netUpdate = true;
        }

        private void ApplySymmetricOrbit(Projectile parent)
        {
            int orbitCount = 0;
            int myIndex = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != Projectile.type)
                    continue;
                if ((int)p.ai[0] != parent.whoAmI || p.ai[2] != 0f)
                    continue;

                if (p.whoAmI < Projectile.whoAmI)
                    myIndex++;

                orbitCount++;
            }

            if (orbitCount <= 0)
                orbitCount = 1;

            float angleOffset = MathHelper.TwoPi * myIndex / orbitCount;
            float targetAngle = Main.GameUpdateCount * OrbitSpeed + angleOffset;
            Vector2 targetOffset = targetAngle.ToRotationVector2() * OrbitRadius;
            Vector2 targetCenter = parent.Center + targetOffset;

            float parentSpeed = parent.velocity.Length();
            float catchupLerp = MathHelper.Clamp(
                OrbitSmoothFactor + parentSpeed * 0.035f,
                OrbitSmoothFactor,
                0.9f
            );

            if (Vector2.DistanceSquared(Projectile.Center, targetCenter) > 1600f)
                Projectile.Center = targetCenter;
            else
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter, catchupLerp);

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.25f;
        }

        private NPC FindNearestTarget()
        {
            NPC bestTarget = null;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy(Projectile))
                    continue;

                float distSq = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                if (distSq >= bestDistSq)
                    continue;

                bestDistSq = distSq;
                bestTarget = npc;
            }

            return bestTarget;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.ScalingArmorPenetration += 1f;
            int shardCount = GetDamageScalingShardCount();
            if (Projectile.ai[2] == 0f)
                ApplyOrbitDamageScaling(ref modifiers, shardCount);
            else
                ApplyFlightDamageScaling(ref modifiers, shardCount);
        }

        private static void ApplyOrbitDamageScaling(ref NPC.HitModifiers modifiers, int shardCount)
        {
            switch (shardCount)
            {
                case 1:
                    modifiers.FinalDamage *= 3f;
                    break;
                case 2:
                    modifiers.FinalDamage *= 2f;
                    break;
                case 3:

                    break;
                case 4:
                    modifiers.FlatBonusDamage += -10;
                    break;
                case 5:
                    modifiers.FlatBonusDamage += -20;
                    break;
                default:

                    modifiers.FlatBonusDamage += -35;
                    break;
            }
        }

        private static void ApplyFlightDamageScaling(ref NPC.HitModifiers modifiers, int shardCount)
        {


            switch (shardCount)
            {
                case 1:
                    modifiers.FinalDamage *= 0.45f;
                    break;
                case 2:
                    modifiers.FinalDamage *= 0.7f;
                    break;
                case 3:

                    break;
                case 4:
                    modifiers.FlatBonusDamage += 10;
                    break;
                case 5:
                    modifiers.FlatBonusDamage += 20;
                    break;
                default:

                    modifiers.FlatBonusDamage += 35;
                    break;
            }
        }

        private int GetDamageScalingShardCount()
        {
            if (Projectile.ai[2] == 1f)
            {
                int launchedWithCount = (int)Projectile.ai[1];
                if (launchedWithCount > 0)
                    return launchedWithCount;
            }

            return GetOrbitingShardCount();
        }

        private int GetOrbitingShardCount()
        {
            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
                return 1;

            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != Projectile.type)
                    continue;
                if ((int)p.ai[0] != parentIndex || p.ai[2] != 0f)
                    continue;

                count++;
            }

            if (count < 1)
                count = 1;

            return count;
        }

        public override void OnKill(int timeLeft)
        {
            if (!dealtDamage)
            {
                SoundStyle failSound = new SoundStyle("Etobudet1modtipo/Sounds/ShardHit_fail")
                {
                    Volume = 0.85f,
                    Pitch = Main.rand.NextFloat(-0.04f, 0.04f),
                    PitchVariance = 0f,
                    MaxInstances = 20
                };
                SoundEngine.PlaySound(failSound, Projectile.Center);
            }

            Vector2 dustPos = Projectile.Center - new Vector2(6f, 6f);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(dustPos, 12, 12, DustID.Adamantite);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            dealtDamage = true;

            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/ShardHit")
            {
                Volume = 0.9f,
                Pitch = Main.rand.NextFloat(-0.04f, 0.04f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);
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
                Color color = lightColor * (0.35f * progress);
                float scale = Projectile.scale * (0.85f + 0.15f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
