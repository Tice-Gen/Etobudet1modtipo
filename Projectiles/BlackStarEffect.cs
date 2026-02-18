using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Projectiles
{
    public class BlackStarEffect : ModProjectile
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/DarkPortal"; 


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {

            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Projectile.ai[0];
                Projectile.localAI[0] = 1f;
            }


            Projectile.alpha += 4; 
            if (Projectile.alpha > 255)
                Projectile.Kill();


            Projectile.scale += 0.01f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0, 0, 0, 255 - Projectile.alpha); 

        }
    }
}