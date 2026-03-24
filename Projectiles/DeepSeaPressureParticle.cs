using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaPressureParticle : ModProjectile
    {
        private const int DelayBeforeRush = 60;

        public override string Texture => "Terraria/Images/Projectile_4";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] <= DelayBeforeRush)
            {
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Vector2 rushDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                Projectile.velocity = rushDirection * 14f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust trail = Dust.NewDustPerfect(
                Projectile.Center,
                180,
                -Projectile.velocity * 0.08f,
                110,
                default,
                1.05f
            );
            trail.noGravity = true;

            if (Projectile.localAI[0] > DelayBeforeRush && Projectile.timeLeft <= 1)
            {
                SpawnImpactDust(Projectile.Center);
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        private static void SpawnImpactDust(Vector2 center)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3.2f, 3.2f);
                Dust hitDust = Dust.NewDustPerfect(center, 180, velocity, 80, default, 1.15f);
                hitDust.noGravity = true;
            }
        }
    }
}
