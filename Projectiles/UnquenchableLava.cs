
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class UnquenchableLava : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;


            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = 2;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 600;

            Projectile.aiStyle = 0;
        }

        public override void AI()
        {


            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;


            }


            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 10f)
            {
                Projectile.ai[0] = 0f;


                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ProjectileID.DD2ExplosiveTrapT2Explosion,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );


                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);


                for (int i = 0; i < 10; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                    
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.Lava,
                        velocity,
                        100,
                        Color.OrangeRed,
                        Main.rand.NextFloat(1.2f, 2.0f)
                    );

                    dust.noGravity = true;
                }
            }


            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Lava,
                    -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f),
                    100,
                    Color.OrangeRed,
                    1.1f
                );

                dust.noGravity = true;
            }


            Lighting.AddLight(
                Projectile.Center,
                1.4f,
                0.6f,
                0.1f
            );
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }


        public override void OnKill(int timeLeft)
        {

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ProjectileID.DD2ExplosiveTrapT3Explosion,
                Projectile.damage * 2,
                Projectile.knockBack,
                Projectile.owner
            );


            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);


            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Lava,
                    velocity,
                    100,
                    Color.OrangeRed,
                    Main.rand.NextFloat(1.5f, 3.0f)
                );

                dust.noGravity = true;
                

                if (Main.rand.NextBool(4))
                {
                    dust.color = new Color(255, Main.rand.Next(150, 220), 30);
                }
            }


            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
                
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.SolarFlare,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.OrangeRed,
                    2.5f
                );
                
                dust.noGravity = true;
            }


            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12f, 12f);
                
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Torch,
                    velocity,
                    100,
                    Color.Yellow,
                    0.8f
                );
                
                dust.noGravity = true;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Vector2 origin = texture.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = (Projectile.oldPos.Length - i)
                                 / (float)Projectile.oldPos.Length;

                Color color = Color.OrangeRed * progress * 0.6f;

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
                    Projectile.scale * (0.9f + progress * 0.2f),
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }
    }
}