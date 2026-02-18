using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class IronArrowProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;


            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            AIType = ProjectileID.WoodenArrowFriendly;
        }

        public override void AI()
        {

            Projectile.velocity.Y += 0.3f;


            Projectile.velocity *= 1.01f;


            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color color = new Color(180, 180, 180, 120) * progress * 0.6f;

                Vector2 drawPos =
                    Projectile.oldPos[i]
                    + Projectile.Size / 2f
                    - Main.screenPosition;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }


        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Tink, Projectile.Center);


            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Mercury,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    default,
                    1.2f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.6f;
            }
        }
    }
}
