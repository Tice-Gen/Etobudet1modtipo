using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class AniserangProj : ModProjectile
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
            Projectile.rotation += 0.01f * Projectile.direction;

            if (Main.rand.NextBool(8))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7, 0f, 0f, 150, default, 0.9f);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.30f)
                target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 300);
        }
    }
}
