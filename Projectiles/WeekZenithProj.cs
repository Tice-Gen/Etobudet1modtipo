using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class WeekZenithProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {


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
            Projectile.tileCollide = true;
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
