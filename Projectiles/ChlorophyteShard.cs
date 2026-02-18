using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class ChlorophyteShard : ModProjectile
    {
        private const float OrbitRadius = 76f;
        private const float RingFlatten = 0.42f;
        private const float BaseSpinSpeed = 0.06f;
        private const float TiltMaxDegrees = 20f;
        private const float TiltSwingSpeed = 0.018f;
        private const float BaseFollowLerp = 0.35f;
        private const float BurstSpinSpeed = 0.5f;
        private const float SpawnExpandTicks = 16f;
        private const int BurstDeathProjectileType = 228;
        private const int BurstDeathDustType = 128;
        private const int BurstDeathDustCount = 18;
        private static uint lastBurstSoundTick;

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
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[2] == 1f)
            {
                UpdateBurstState();
                return;
            }

            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[parentIndex];
            if (!parent.active || parent.type != ModContent.ProjectileType<ChlorophyteYoyoProj>() || parent.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            int orbitCount = 0;
            int orbitIndex = 0;
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
                    orbitIndex++;

                orbitCount++;
            }

            if (orbitCount < 1)
                orbitCount = 1;

            float slotPhase = MathHelper.TwoPi * orbitIndex / orbitCount;


            float angle = Main.GameUpdateCount * BaseSpinSpeed + slotPhase;
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            Vector2 targetOffset = new Vector2(cos * OrbitRadius, sin * OrbitRadius * RingFlatten);

            float tiltRadians = MathHelper.ToRadians(TiltMaxDegrees) * (float)System.Math.Sin(Main.GameUpdateCount * TiltSwingSpeed);
            targetOffset = targetOffset.RotatedBy(tiltRadians);

            Projectile.localAI[0] = MathHelper.Min(Projectile.localAI[0] + 1f, SpawnExpandTicks);
            float spawnProgress = MathHelper.Clamp(Projectile.localAI[0] / SpawnExpandTicks, 0f, 1f);
            float spawnEase = 1f - (1f - spawnProgress) * (1f - spawnProgress);
            targetOffset *= spawnEase;

            Vector2 targetCenter = parent.Center + targetOffset;


            float depth = MathHelper.Clamp(targetOffset.Y / OrbitRadius * 0.5f + 0.5f, 0f, 1f);
            Projectile.scale = MathHelper.Lerp(0.78f, 1.06f, depth);
            Projectile.alpha = (int)MathHelper.Lerp(105f, 12f, depth);

            float parentSpeed = parent.velocity.Length();
            float followLerp = MathHelper.Clamp(
                BaseFollowLerp + parentSpeed * 0.045f,
                BaseFollowLerp,
                0.92f
            );

            if (spawnProgress >= 1f && Vector2.DistanceSquared(Projectile.Center, targetCenter) > 2304f)
                Projectile.Center = targetCenter;
            else
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter, followLerp);

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.4f;
        }

        private void UpdateBurstState()
        {
            Projectile.penetrate = 1;
            Projectile.rotation += BurstSpinSpeed;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[2] != 1f)
                return;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    BurstDeathProjectileType,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            if (lastBurstSoundTick != Main.GameUpdateCount)
            {
                lastBurstSoundTick = Main.GameUpdateCount;
                SoundStyle spawnVanillaProjSound = new SoundStyle("Etobudet1modtipo/Sounds/InShardToThePoison")
                {
                    Volume = 0.9f,
                    Pitch = Main.rand.NextFloat(-0.02f, 0.02f),
                    PitchVariance = 0f,
                    MaxInstances = 20
                };
                SoundEngine.PlaySound(spawnVanillaProjSound, Projectile.Center);
            }

            for (int i = 0; i < BurstDeathDustCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3.2f, 3.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, BurstDeathDustType, velocity, 0, default, 1.15f);
                dust.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.ScalingArmorPenetration += 0.825f;

            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                float currentMultiplier = target.takenDamageMultiplier;
                float targetMultiplier = 1f - (1f - currentMultiplier) * 0.825f;
                modifiers.FinalDamage *= targetMultiplier / currentMultiplier;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            string soundPath = Main.rand.NextBool()
                ? "Etobudet1modtipo/Sounds/MetalHit_Low"
                : "Etobudet1modtipo/Sounds/MetalHit_Medium";

            SoundStyle shardHitSound = new SoundStyle(soundPath)
            {
                Volume = 0.9f,
                Pitch = Main.rand.NextFloat(-0.015f, 0.015f),
                PitchVariance = 0f,
                MaxInstances = 24
            };
            SoundEngine.PlaySound(shardHitSound, Projectile.Center);
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
                Color color = lightColor * (0.3f * progress);
                float scale = Projectile.scale * (0.88f + 0.12f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
