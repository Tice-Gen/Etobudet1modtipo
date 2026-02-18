using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class ScytheQueuePlayer : ModPlayer
    {
        public int ScytheTimer = 0;
        public int ScythesLeft = 0;
        public Vector2 ShootVelocity;
        public int ShootDamage;
        public float ShootKnockback;

        public void StartScytheQueue(Vector2 velocity, int damage, float kb)
        {
            ScythesLeft = 10;
            ScytheTimer = 0;
            ShootVelocity = velocity;
            ShootDamage = damage;
            ShootKnockback = kb;
        }

        public override void PostUpdate()
        {
            if (ScythesLeft > 0)
            {
                ScytheTimer++;
                if (ScytheTimer >= 3)
                {
                    ScytheTimer = 0;
                    SpawnScythe();
                    ScythesLeft--;
                }
            }
        }

        private void SpawnScythe()
        {
            float step = 90f / 10f;
            float currentAngle = -45f + (step * (10 - ScythesLeft));
            Vector2 finalVelocity = ShootVelocity.RotatedBy(MathHelper.ToRadians(currentAngle)) * 0.2f;

            if (Player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Player.GetSource_ItemUse(Player.HeldItem),
                    Player.MountedCenter,
                    finalVelocity,
                    ProjectileID.DemonScythe,
                    ShootDamage,
                    ShootKnockback,
                    Player.whoAmI
                );
            }
        }
    }
}
