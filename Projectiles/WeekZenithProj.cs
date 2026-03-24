using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class WeekZenithProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {

            Projectile.rotation += 0.05f * Projectile.direction;


            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 120, default, 1.1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Color trailColor = new Color(235, 40, 40, 0) * (0.45f * progress);
                float trailScale = Projectile.scale * (0.85f + progress * 0.25f);

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    trailColor,
                    Projectile.oldRot[i],
                    drawOrigin,
                    trailScale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 12; i++)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, speed.X, speed.Y, 150, default, 1.3f);
            }


            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
