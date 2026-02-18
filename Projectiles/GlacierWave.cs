using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class GlacierWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, 0.35f, 0.7f, 1.1f);

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Snow,
                    0f,
                    0f,
                    100,
                    default,
                    1.0f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.3f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero)
                    continue;

                Vector2 drawPos =
                    Projectile.oldPos[k]
                    - Main.screenPosition
                    + drawOrigin
                    + new Vector2(0f, Projectile.gfxOffY);

                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;

                Color color =
                    Projectile.GetAlpha(new Color(120, 210, 255, 170))
                    * progress * progress * 0.75f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.oldRot[k],
                    drawOrigin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 480);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    0f,
                    0f,
                    100,
                    default,
                    1.5f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
            }
        }
    }
}
