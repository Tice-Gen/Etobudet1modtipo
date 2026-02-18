using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class BurstingStarAnise : ModProjectile
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
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 60000;
            Projectile.aiStyle = 2;
            AIType = ProjectileID.StarAnise;
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
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;

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
                    SpriteEffects.None
                );
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            int fragmentType = ModContent.ProjectileType<FragmentOfAnise>();
            int damage = Projectile.damage / 2;
            float speed = 8f;

            Vector2[] dirs =
            {
                new(0, -1), new(0, 1),
                new(1, 0), new(-1, 0),
                new(1, -1), new(1, 1),
                new(-1, -1), new(-1, 1)
            };

            foreach (Vector2 dir in dirs)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    center,
                    Vector2.Normalize(dir) * speed,
                    fragmentType,
                    damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            SoundEngine.PlaySound(SoundID.Item14, center);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(center - Vector2.One * 10, 20, 20, DustID.GoldCoin);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
            }
        }
    }
}
