using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class DeadStar : ModProjectile
    {
        private int frameCounter = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.85f, 0.85f, 0.85f);
            Projectile.rotation += 0.1f;

            frameCounter++;
            if (frameCounter >= 5)
            {
                frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 5)
                    Projectile.frame = 0;
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 origin = frame.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * progress * 0.55f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    color,
                    Projectile.oldRot[i],
                    origin,
                    Projectile.scale * progress,
                    SpriteEffects.None,
                    0
                );
            }

            Vector2 centerDrawPos = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.White * 0.42f * Projectile.Opacity;
            float glowRadius = 3f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f) * glowRadius;
                Main.EntitySpriteDraw(
                    texture,
                    centerDrawPos + offset,
                    frame,
                    glowColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.04f,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }
    }
}
