using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class ProtectingAnise : ModProjectile
    {

        private const float ShootCooldown = 10f;
        private const float ProjectileSpeed = 8f;
        private const float RotationSpeed = MathHelper.TwoPi / 60f;

        private const float BaseRadius = 70f;
        private const float StarAmplitude = 25f;
        private const int StarPoints = 5;
        private const float FollowSmoothness = 20f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.minionSlots = 1f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<ProtectingAniseBuff>());
                return;
            }

            if (player.HasBuff(ModContent.BuffType<ProtectingAniseBuff>()))
                Projectile.timeLeft = 2;


            int index = 0;
            int total = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    if (p.whoAmI < Projectile.whoAmI)
                        index++;

                    total++;
                }
            }

            if (total < 1) total = 1;


            Vector2 baseCenter = player.Center + new Vector2(0, -60);

            Vector2 desiredPosition;

            if (total > 1)
            {

                float time = Main.GlobalTimeWrappedHourly * 0.8f;
                float angle = MathHelper.TwoPi * index / total + time;


                float starFactor = (float)Math.Cos(StarPoints * angle);
                float radius = BaseRadius + starFactor * StarAmplitude;

                Vector2 offset = angle.ToRotationVector2() * radius;
                desiredPosition = baseCenter + offset;
            }
            else
            {

                desiredPosition = baseCenter;
            }


            Vector2 move = desiredPosition - Projectile.Center;

            if (move.Length() > 800f)
                Projectile.Center = desiredPosition;

            Projectile.velocity = (Projectile.velocity * FollowSmoothness + move) / (FollowSmoothness + 1f);


            Projectile.rotation += RotationSpeed;


            Projectile.ai[0]++;
            if (Projectile.ai[0] >= ShootCooldown)
            {
                Projectile.ai[0] = 0f;
                int target = FindNearestTarget(600f);
                if (target != -1)
                {
                    NPC npc = Main.npc[target];
                    Vector2 dir = npc.Center - Projectile.Center;
                    dir.Normalize();
                    dir *= ProjectileSpeed;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center,
                        dir,
                        ProjectileID.HornetStinger,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }
        }

        private int FindNearestTarget(float maxDetectDistance)
        {
            int closest = -1;
            float closestDist = maxDetectDistance;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = i;
                    }
                }
            }
            return closest;
        }
    }
}
