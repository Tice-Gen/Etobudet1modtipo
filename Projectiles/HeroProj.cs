using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Particles;

namespace Etobudet1modtipo.Projectiles
{
    public class HeroProj : ModProjectile
    {
        private const int MaxBounces = 5;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.aiStyle = 0;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.extraUpdates = 1;
        }


        public override void AI()
        {
            Projectile.rotation += 0.3f;


            if (Main.rand.NextBool(2))
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Terra
                );

                Dust dust = Main.dust[dustIndex];
                dust.velocity = Projectile.velocity * 0.25f
                                + Main.rand.NextVector2Circular(1.5f, 1.5f);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.8f, 0.2f);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
{
    Projectile.ai[0]++;


    for (int i = 0; i < 7; i++)
    {
        int d = Dust.NewDust(
            Projectile.Center,
            0,
            0,
            ModContent.DustType<HeroParticle>()
        );

        Dust dust = Main.dust[d];
        dust.velocity = Main.rand.NextVector2Circular(2.8f, 2.8f);
        dust.scale = Main.rand.NextFloat(0.7f, 1.0f);
        dust.noGravity = true;
    }

    if (Projectile.ai[0] >= MaxBounces)
    {
        Projectile.Kill();
        return false;
    }


    if (Projectile.velocity.X != oldVelocity.X)
        Projectile.velocity.X = -oldVelocity.X;

    if (Projectile.velocity.Y != oldVelocity.Y)
        Projectile.velocity.Y = -oldVelocity.Y;

    Terraria.Audio.SoundEngine.PlaySound(
        SoundID.Item10,
        Projectile.position
    );

    return false;
}



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, Projectile.height / 2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                Vector2 drawPos =
                    Projectile.oldPos[i]
                    - Main.screenPosition
                    + origin
                    + new Vector2(0f, Projectile.gfxOffY);

                float progress =
                    (float)(Projectile.oldPos.Length - i)
                    / Projectile.oldPos.Length;

                Color color =
                    Projectile.GetAlpha(Color.LimeGreen)
                    * progress * 0.5f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.oldRot[i],
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }


        public override void OnKill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(
                SoundID.Item10,
                Projectile.position
            );


            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Terra
                );

                Dust dust = Main.dust[d];
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                dust.noGravity = true;
                dust.scale = 1.4f;
            }


            for (int i = 0; i < 18; i++)
            {
                int d = Dust.NewDust(
                    Projectile.Center,
                    0,
                    0,
                    ModContent.DustType<HeroParticle>()
                );

                Dust dust = Main.dust[d];
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.scale = Main.rand.NextFloat(0.9f, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
