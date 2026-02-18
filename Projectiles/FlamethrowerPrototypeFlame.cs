using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class FlamethrowerPrototypeFlame : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; 
            Projectile.tileCollide = true;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;

            Vector2 direction = Projectile.velocity;
            if (direction != Vector2.Zero)
                direction.Normalize();

            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                int dustIndex = Dust.NewDust(Projectile.position + offset, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[dustIndex];
                dust.velocity = direction * Main.rand.NextFloat(2f, 4f);
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.4f)
            {
                target.AddBuff(BuffID.OnFire, 600);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
