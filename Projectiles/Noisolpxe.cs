using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class Noisolpxe : ModProjectile
    {
        private const int TotalFrames = 5;
        private const int FrameWidth = 177;
        private const int FrameHeight = 210;
        private const int LifetimeTicks = 15;
        private bool activated;

        private int PrepDelayTicks => (int)MathHelper.Clamp(Projectile.ai[0], 0f, 120f);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = TotalFrames;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = FrameWidth;
            Projectile.height = FrameHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            int age = (int)Projectile.localAI[0]++;
            int prepDelay = PrepDelayTicks;

            if (age < prepDelay)
            {
                SpawnPreparingTorchDust(age, prepDelay);
                return;
            }

            if (!activated)
            {
                Activate();
            }

            int activeAge = age - prepDelay;
            if (activeAge >= LifetimeTicks)
            {
                Projectile.Kill();
                return;
            }

            Projectile.frame = GetFrameByAge(activeAge);

            float glowStrength = 1f - activeAge / (float)LifetimeTicks;
            Lighting.AddLight(Projectile.Center, 1.25f * glowStrength, 0.45f * glowStrength, 0.12f * glowStrength);
        }

        private static int GetFrameByAge(int age)
        {
            float unit = LifetimeTicks / 21f; // 1 + 5 + 5 + 5 + 5
            float firstTransition = unit;
            float secondTransition = unit * 6f;
            float thirdTransition = unit * 11f;
            float fourthTransition = unit * 16f;

            if (age < firstTransition)
            {
                return 0;
            }

            if (age < secondTransition)
            {
                return 1;
            }

            if (age < thirdTransition)
            {
                return 2;
            }

            if (age < fourthTransition)
            {
                return 3;
            }

            return 4;
        }

        private void Activate()
        {
            activated = true;
            SpawnActivationTorchDust();
            SpawnChildProjectiles();
        }

        private void SpawnPreparingTorchDust(int age, int prepDelay)
        {
            float progress = prepDelay <= 0 ? 1f : age / (float)prepDelay;
            float radius = MathHelper.Lerp(16f, 44f, progress);
            float spin = Main.GlobalTimeWrappedHourly * 6.2f;
            int count = 4;

            for (int i = 0; i < count; i++)
            {
                float angle = spin + MathHelper.TwoPi * i / count;
                Vector2 offset = angle.ToRotationVector2() * radius;
                Vector2 tangent = offset.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2) * 0.7f;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Torch, tangent, 100, default, Main.rand.NextFloat(1f, 1.3f));
                d.noGravity = true;
            }
        }

        private void SpawnActivationTorchDust()
        {
            for (int i = 0; i < 26; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.4f, 6.8f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, velocity, 70, default, Main.rand.NextFloat(1.15f, 1.65f));
                d.noGravity = true;
            }
        }

        private void SpawnChildProjectiles()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<Effect>(),
                0,
                0f,
                Projectile.owner);

            int count = Main.rand.Next(3, 6);
            int type = ModContent.ProjectileType<Noisolpxeanise>();
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count + Main.rand.NextFloat(-0.35f, 0.35f);
                Vector2 spawnOffset = angle.ToRotationVector2() * Main.rand.NextFloat(150f, 260f);
                Vector2 spawnPos = Projectile.Center + spawnOffset;
                Vector2 toCenter = (Projectile.Center - spawnPos).SafeNormalize(Vector2.UnitY);
                Vector2 velocity = toCenter * Main.rand.NextFloat(8.5f, 13.5f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    velocity,
                    type,
                    Projectile.damage,
                    2f,
                    Projectile.owner,
                    0f,
                    i * 5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int age = (int)Projectile.localAI[0];
            if (age < PrepDelayTicks)
            {
                return false;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0, Projectile.frame * FrameHeight, FrameWidth, FrameHeight);
            Vector2 origin = new Vector2(FrameWidth * 0.5f, FrameHeight * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color glowTrail = new Color(255, 120, 40, 0) * (0.36f * progress);
                float trailScale = 1.06f + 0.22f * progress;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, drawPos, frame, glowTrail, 0f, origin, trailScale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}
