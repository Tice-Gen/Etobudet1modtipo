using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class ImprovedStaffProjectile : ModProjectile
    {
        private int hitTimer = 0;
        private bool secondHitScheduled = false;
        private int targetID = -1;

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DiamondBolt;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DiamondBolt);
            AIType = ProjectileID.DiamondBolt;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {

            if (!secondHitScheduled)
            {
                float homingStrength = 0.15f;
                NPC target = FindTarget();
                if (target != null)
                {
                    Vector2 direction = target.Center - Projectile.Center;
                    direction.Normalize();
                    direction *= 10f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, homingStrength);
                }
            }


            if (secondHitScheduled)
            {
                hitTimer++;
                if (hitTimer >= 60)
                {
                    if (targetID >= 0 && targetID < Main.npc.Length)
                    {
                        NPC npc = Main.npc[targetID];
                        if (npc.active)
                        {
                            npc.StrikeNPC(new NPC.HitInfo()
                            {
                                Damage = (int)(Projectile.damage * 1.5f),
                                Knockback = 0f,
                                HitDirection = 0
                            });
                        }
                    }
                    secondHitScheduled = false;
                    Projectile.Kill();
                }
            }
        }

        private NPC FindTarget()
        {
            NPC chosen = null;
            float distanceMax = 400f;
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < distanceMax)
                    {
                        distanceMax = distance;
                        chosen = npc;
                    }
                }
            }
            return chosen;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            targetID = target.whoAmI;
            hitTimer = 0;
            secondHitScheduled = true;


        }
    }
}