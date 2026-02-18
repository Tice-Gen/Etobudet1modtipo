using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class CircleoftheDyingStar : ModProjectile
    {
        private int shootTimer = 0;
        private int frameCounter = 0;

        private Vector2 pendingSpawnPos;
        private int delayTimer = 0;
        private bool waitingToSpawn = false;

        private const float SpawnRadius = 225f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft > 1750)
                Projectile.alpha -= 10;
            else if (Projectile.timeLeft < 50)
                Projectile.alpha += 10;

            Projectile.alpha = Utils.Clamp(Projectile.alpha, 0, 255);

            Projectile.Center = player.Center;

            frameCounter++;
            if (frameCounter >= 8)
            {
                frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            if (waitingToSpawn)
            {
                delayTimer++;
                if (delayTimer >= 15)
                {
                    SpawnDeadStar(pendingSpawnPos, player);
                    waitingToSpawn = false;
                }
                return;
            }

            shootTimer++;
            if (shootTimer >= 20)
            {
                shootTimer = 0;

                float angle = MathHelper.ToRadians(Main.rand.Next(0, 360));
                Vector2 offset = angle.ToRotationVector2() * SpawnRadius;
                Vector2 spawnPos = player.Center + offset;

                CreateSnowRing(spawnPos);

                pendingSpawnPos = spawnPos;
                delayTimer = 0;
                waitingToSpawn = true;
            }
        }

        private void CreateSnowRing(Vector2 center)
        {
            int dustCount = 24;
            float radius = 40f;

            for (int i = 0; i < dustCount; i++)
            {
                float angle = MathHelper.TwoPi / dustCount * i;
                Vector2 offset = angle.ToRotationVector2() * radius;

                Dust dust = Dust.NewDustPerfect(
                    center + offset,
                    DustID.Snow,
                    Vector2.Zero,
                    0,
                    default,
                    1.2f
                );

                dust.noGravity = true;
                dust.velocity = offset * 0.05f;
            }
        }

        private void SpawnDeadStar(Vector2 spawnPos, Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 direction = player.Center - spawnPos;
            direction.Normalize();
            direction *= 9f;

            Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                spawnPos,
                direction,
                ModContent.ProjectileType<DeadStar>(),
                40,
                2f,
                Main.myPlayer
            );
        }
    }
}
