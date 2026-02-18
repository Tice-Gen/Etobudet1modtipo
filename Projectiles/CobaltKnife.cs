using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class CobaltKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 6000;
            

            Projectile.extraUpdates = 1;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                

                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(new Color(0, 150, 255, 100)) * progress;
                

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {

            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Cobalt,
                    Projectile.velocity.X * 0.3f,
                    Projectile.velocity.Y * 0.3f,
                    150,
                    default,
                    Main.rand.NextFloat(0.8f, 1.2f)
                );

                var dust = Main.dust[dustIndex];
                dust.velocity *= 1.6f;
                dust.noGravity = true;
                dust.fadeIn = 0.8f;
            }
        }
    }
}