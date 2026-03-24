using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Global
{
    public class CascadeSparkGlobalProjectile : GlobalProjectile
    {
        private const int SparkInterval = 36;
        private const float SparkSpeed = 6.5f;
        private const float SparkDamageMultiplier = 0.45f;
        private const float SparkKnockbackMultiplier = 0.6f;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.type == ProjectileID.Cascade;
        }

        public override void AI(Projectile projectile)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Player owner = Main.player[projectile.owner];
            if (!owner.active || owner.dead)
                return;

            projectile.localAI[0]++;
            if (projectile.localAI[0] < SparkInterval)
                return;

            projectile.localAI[0] = 0f;

            Vector2 sparkVelocity = Main.rand.NextVector2Unit() * SparkSpeed;
            int sparkDamage = System.Math.Max(1, (int)System.MathF.Round(projectile.damage * SparkDamageMultiplier));
            float sparkKnockback = projectile.knockBack * SparkKnockbackMultiplier;

            int spark = Projectile.NewProjectile(
                projectile.GetSource_FromThis(),
                projectile.Center,
                sparkVelocity,
                ProjectileID.Spark,
                sparkDamage,
                sparkKnockback,
                projectile.owner);

            if (spark >= 0 && spark < Main.maxProjectiles)
                Main.projectile[spark].DamageType = DamageClass.Melee;
        }
    }
}
