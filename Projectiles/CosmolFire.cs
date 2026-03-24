using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class CosmolFire : ModProjectile
    {
        private const int PlayerHitCooldown = 5;
        private static readonly Color CoreColor = new Color(10, 7, 18, 0);
        private static readonly Color OuterStartColor = new Color(232, 220, 255, 210);
        private static readonly Color OuterMidColor = new Color(196, 160, 245, 90);
        private static readonly Color OuterLateColor = new Color(148, 112, 214, 120);
        private static readonly Color OuterFadeColor = new Color(70, 52, 102, 80);

        public static int Lifetime => 90;
        public static int Fadetime => 80;
        public ref float Time => ref Projectile.ai[0];
        private int BurnDuration => Projectile.ai[1] > 0f ? (int)Projectile.ai[1] : 1200;

        public override string Texture => "Etobudet1modtipo/Projectiles/CosmolFire";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 240;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Default;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Time++;

            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time, true);
            Projectile.frame = Utils.Clamp((int)(timeRatio * Main.projFrames[Type]), 0, Main.projFrames[Type] - 1);

            if (Projectile.velocity.LengthSquared() > 0.001f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Time < Fadetime && Main.rand.NextBool(6))
                SpawnCinderDust();

            Lighting.AddLight(Projectile.Center, 0.16f, 0.05f, 0.22f);
        }

        private void SpawnCinderDust()
        {
            Vector2 cinderPos = Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Lifetime, 0.5f, 1f);
            float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
            Dust cinder = Dust.NewDustDirect(cinderPos, 4, 4, DustID.Shadowflame, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, OuterStartColor);

            if (Main.rand.NextBool(3))
            {
                cinder.scale *= 2f;
                cinder.velocity *= 2f;
            }

            cinder.noGravity = true;
            cinder.scale *= cinderSize * 1.2f;
            cinder.velocity += Projectile.velocity
                * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f)
                * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
            Time++;
            Projectile.timeLeft--;
            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)Utils.Remap(Time, 0f, Fadetime, 8f, 32f);

            if (Time > Fadetime)
                size = (int)Utils.Remap(Time, Fadetime, Lifetime, 32f, 0f);

            hitbox.Inflate(size, size);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox) && Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0)
                ? null
                : false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (CooldownSlot >= 0 && CooldownSlot < target.hurtCooldowns.Length)
                target.hurtCooldowns[CooldownSlot] = PlayerHitCooldown;

            target.AddBuff(BuffID.ShadowFlame, BurnDuration);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            float length = Time > Fadetime - 10f ? 0.1f : 0.15f;
            float velocityOffset = System.MathF.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time, true);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

            if (timeRatio >= 1f)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                Color outlineColor = GetFlameColor(timeRatio);
                outlineColor *= (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

                Color coreColor = Color.Lerp(outlineColor, CoreColor, 0.88f);
                coreColor.A = 0;

                Vector2 drawPosition = Projectile.Center - Main.screenPosition - Projectile.velocity * velocityOffset * j;
                float spin = Main.GlobalTimeWrappedHourly * (j + 1f) * 1.45f;
                float mainRotation = Projectile.rotation - j * MathHelper.PiOver4 - spin;
                float trailRotation = mainRotation + MathHelper.PiOver4;
                Vector2 trailOffset = Projectile.velocity * velocityOffset * length * 0.5f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPosition - trailOffset,
                    frame,
                    coreColor * 0.22f,
                    trailRotation,
                    origin,
                    fireSize * 0.95f,
                    SpriteEffects.None,
                    0
                );

                for (int i = 0; i < 4; i++)
                {
                    float angle = mainRotation + MathHelper.TwoPi * i / 4f;
                    Vector2 outlineOffset = angle.ToRotationVector2() * (0.9f + fireSize * 0.85f);
                    Main.EntitySpriteDraw(
                        texture,
                        drawPosition + outlineOffset,
                        frame,
                        outlineColor * 0.48f,
                        mainRotation,
                        origin,
                        fireSize * 1.03f,
                        SpriteEffects.None,
                        0
                    );
                }

                Main.EntitySpriteDraw(
                    texture,
                    drawPosition,
                    frame,
                    coreColor,
                    mainRotation,
                    origin,
                    fireSize,
                    SpriteEffects.None,
                    0
                );
            }

            return false;
        }

        private static Color GetFlameColor(float timeRatio)
        {
            if (timeRatio < 0.1f)
                return Color.Lerp(Color.Transparent, OuterStartColor, Utils.GetLerpValue(0f, 0.1f, timeRatio, true));

            if (timeRatio < 0.2f)
                return Color.Lerp(OuterStartColor, OuterMidColor, Utils.GetLerpValue(0.1f, 0.2f, timeRatio, true));

            if (timeRatio < 0.35f)
                return OuterMidColor;

            if (timeRatio < 0.7f)
                return Color.Lerp(OuterMidColor, OuterLateColor, Utils.GetLerpValue(0.35f, 0.7f, timeRatio, true));

            if (timeRatio < 0.85f)
                return Color.Lerp(OuterLateColor, OuterFadeColor, Utils.GetLerpValue(0.7f, 0.85f, timeRatio, true));

            return Color.Lerp(OuterFadeColor, Color.Transparent, Utils.GetLerpValue(0.85f, 1f, timeRatio, true));
        }
    }
}
