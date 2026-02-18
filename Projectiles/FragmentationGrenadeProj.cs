using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace Etobudet1modtipo.Projectiles
{
    public class FragmentationGrenadeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;

            Projectile.DamageType = DamageClass.Ranged;


            Projectile.aiStyle = 2;
            AIType = ProjectileID.Grenade;

            Projectile.tileCollide = true;
        }

        public override void Kill(int timeLeft)
        {



            Explode();




            SpawnFragments();




            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke,
                    Main.rand.NextFloat(-4, 4),
                    Main.rand.NextFloat(-4, 4)
                );
            }
        }

        private void Explode()
        {
            int radius = 120;

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = radius;
            Projectile.Center = Projectile.position;

            Projectile.penetrate = -1;

            Projectile.Damage();
        }

        private void SpawnFragments()
        {
            int count = 14;

            for (int i = 0; i < count; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Unit() *
                    Main.rand.NextFloat(6f, 10f);

                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    velocity,
                    328,
                    Projectile.damage / 2,
                    1f,
                    Projectile.owner
                );

                Projectile p = Main.projectile[proj];

                p.friendly = true;
                p.hostile = false;
                p.owner = Projectile.owner;
                p.DamageType = DamageClass.Ranged;
                p.timeLeft = 180;
            }
        }
    }
}
