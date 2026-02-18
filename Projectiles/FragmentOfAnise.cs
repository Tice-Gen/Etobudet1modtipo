using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class FragmentOfAnise : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 21;
            Projectile.height = 21;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.timeLeft = 60000;

            Projectile.aiStyle = 2;
            AIType = ProjectileID.StarAnise;


            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
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
                Vector2 drawPos = Projectile.oldPos[i]
                    - Main.screenPosition
                    + Projectile.Size / 2f;

                Color color = new Color(255, 255, 100, 0) * progress * 0.6f;
                float scale = Projectile.scale * progress;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.oldRot[i],
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 center = Projectile.Center;

            SoundEngine.PlaySound(SoundID.Item50, center);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(
                    center - Vector2.One * 10,
                    20,
                    20,
                    DustID.GoldCoin
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }
}
