using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class RuneosamaInvisibleProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;


            Projectile.friendly = true;
            



            Projectile.ArmorPenetration = 999999999;



            Projectile.damage = 5000;


            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 60;


            Projectile.alpha = 255;


            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;


            Projectile.DamageType = DamageClass.Melee;

            Projectile.noEnchantmentVisuals = true;
            Projectile.aiStyle = -1;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {








            


        }


        public override void AI()
        {








            float spawnDistance = 70f;
            float swordSpeed = 10f;

            float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            Vector2 spawnPos = Projectile.Center + new Vector2(spawnDistance, 0f).RotatedBy(angle);
            Vector2 toCenter = Projectile.Center - spawnPos;
            Vector2 projVelocity = Vector2.Normalize(toCenter) * swordSpeed;


            int fixedDamage = 1000;



            int projType = ProjectileID.Muramasa;

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    projVelocity,
                    projType,
                    fixedDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }
    }
}