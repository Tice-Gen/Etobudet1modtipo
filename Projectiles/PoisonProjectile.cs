using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Projectiles
{
    public class PoisonProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;

        bool initialized = false;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.damage = 0;
        }

        public override void AI()
        {
            if (!initialized)
            {

                Projectile.damage = 0;
                initialized = true;
            }


            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned,
                Projectile.velocity.X * 0.8f, Projectile.velocity.Y * 0.8f, 100, default, 1.2f);

            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.9f;
            Main.dust[dust].velocity += Projectile.velocity * 0.2f;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 30 * 60);


        }
    }
}
