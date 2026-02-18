using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class StoneBallProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
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


            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
        }
    }
}
