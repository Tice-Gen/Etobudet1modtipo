using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class WeekPlasmaPulse : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 56;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            float fadeIn = Utils.GetLerpValue(0f, 8f, 56f - Projectile.timeLeft, true);
            float fadeOut = Utils.GetLerpValue(0f, 10f, Projectile.timeLeft, true);
            float pulse = 0.92f + 0.12f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 11f + Projectile.whoAmI);
            Projectile.scale = fadeIn * fadeOut * pulse;

            Lighting.AddLight(Projectile.Center, 0.09f, 0.48f, 0.55f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float pulse = 0.85f + 0.15f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 8f + Projectile.whoAmI * 0.2f);

            Color baseGlow = new Color(70, 225, 255, 0);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 oldDrawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float stretchY = Projectile.scale * (1.35f + progress * 2.1f) * pulse;
                float stretchX = Projectile.scale * (0.62f + progress * 0.5f) * pulse;
                Color trailColor = baseGlow * (0.155f * progress);

                Main.EntitySpriteDraw(texture, oldDrawPos, null, trailColor, Projectile.rotation, origin, new Vector2(stretchX, stretchY), SpriteEffects.None, 0);
            }

            // Glow only: draw soft expanded layers and skip any dense core.
            for (int i = 0; i < 14; i++)
            {
                float layerScale = Projectile.scale * (1.35f + i * 0.17f) * pulse;
                Color layer = baseGlow * (0.096f - i * 0.0053f);
                if (layer == default)
                    continue;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    layer,
                    Projectile.rotation,
                    origin,
                    layerScale,
                    SpriteEffects.None,
                    0
                );
            }

            Color aura = baseGlow * 0.22f;
            for (int i = 0; i < 4; i++)
            {
                float angle = MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 2.8f;
                Vector2 offset = angle.ToRotationVector2() * 3f;
                Main.EntitySpriteDraw(texture, drawPos + offset, null, aura, Projectile.rotation, origin, Projectile.scale * 1.38f * pulse, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
