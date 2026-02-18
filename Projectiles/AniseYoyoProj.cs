using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Projectiles
{
    public class AniseYoyoProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 8;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 260;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);


            int type = ProjectileID.StarAnise;
            float speed = 8f;
            int damage = Projectile.damage / 3;
            float knockback = 3f;

            for (int i = 0; i < 1; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * speed;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    type,
                    damage,
                    knockback,
                    Projectile.owner
                );
            }


            Vector2 center = Projectile.Center;
            for (int i = 0; i < 18; i++)
            {
                Dust.NewDust(center - new Vector2(8, 8), 16, 16, DustID.FireworkFountain_Yellow);
            }
        }
    }
}