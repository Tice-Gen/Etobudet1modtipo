using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class BloomingSpores : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            

            Projectile.penetrate = -1; 
            
            Projectile.timeLeft = 300; 
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            
            Projectile.alpha = 255; 
            Projectile.knockBack = 4f;




            Projectile.ArmorPenetration = 10000;




            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20; 
        }

        public override void AI()
        {

            if (Projectile.timeLeft > 60) 
            {
                int targetAlpha = 70; 
                if (Projectile.alpha > targetAlpha)
                {
                    Projectile.alpha -= 3;
                    if (Projectile.alpha < targetAlpha) Projectile.alpha = targetAlpha;
                }
            }


            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 5; 
                if (Projectile.alpha > 255) Projectile.alpha = 255;
            }


            Projectile.rotation += 0.05f;
            Projectile.velocity *= 0.98f;



            if (Projectile.timeLeft == 3)
            {
                Projectile.alpha = 255;


                Projectile.damage = (int)(Projectile.damage * 1.3f);
                


                for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                {
                    Projectile.localNPCImmunity[i] = 0;
                }


                Projectile.Resize(150, 150); 
                

                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.position);


                for (int i = 0; i < 50; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkFairy, 0f, 0f, 100, default(Color), 2f);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity *= 1.4f;
                    dust.noGravity = true;
                }
            }
        }
    }
}