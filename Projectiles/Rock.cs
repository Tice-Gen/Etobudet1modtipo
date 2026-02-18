using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class Rock : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 16;
            Projectile.aiStyle = 2;
            AIType = ProjectileID.StarAnise;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Stone,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f
                );
            }
        }
    }
}
