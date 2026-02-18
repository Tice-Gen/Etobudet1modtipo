using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class ExplosiveAnise : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 33;
            Projectile.height = 33;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = 2;
            AIType = ProjectileID.StarAnise;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;

                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(new Color(255, 200, 50, 0)) * progress * 0.7f;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * progress, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Item14, center);

            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(center - new Vector2(8, 8), 16, 16, DustID.FireworkFountain_Yellow, 0f, 0f, 100, default, 1.2f);
                Main.dust[dustIndex].velocity *= 2.2f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
    }
}