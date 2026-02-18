using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Projectiles
{
    public class VenomProjectile : ModProjectile
    {

        public override string Texture => "Etobudet1modtipo/Projectiles/VenomProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {

            Lighting.AddLight(Projectile.Center, 0f, 0.8f, 0.2f);


            int dust = Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.VenomStaff,
                Projectile.velocity.X * 0.8f,
                Projectile.velocity.Y * 0.8f,
                100,
                default,
                1.2f
            );

            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.9f;
            Main.dust[dust].velocity += Projectile.velocity * 0.2f;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(BuffID.Venom, 60 * 60);
        }
    }
}