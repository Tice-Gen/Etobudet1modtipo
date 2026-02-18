
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class LunarZigzag : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.aiStyle = 0;
        }


        public override void AI()
        {
            Projectile.ai[0]++;


            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                Projectile.localAI[1] = Projectile.velocity.ToRotation();

                Projectile.ai[1] = Main.rand.NextFloat(-0.35f, 0.35f);
            }

            float baseAngle = Projectile.localAI[1];
            float speed = Projectile.velocity.Length();


            int period = 10;
            float amplitude = MathHelper.ToRadians(55f);

            int phase = (int)(Projectile.ai[0] / period);
            bool positive = (phase % 2) == 0;

            float angleOffset = positive ? amplitude : -amplitude;
            float newAngle = baseAngle + angleOffset;


            Projectile.velocity = newAngle.ToRotationVector2() * speed;


            int prevPhase = (int)((Projectile.ai[0] - 1f) / period);
            if (prevPhase != phase)
            {
                Projectile.netUpdate = true;
            }


            int spawnAmount = 2 + Main.rand.Next(3);
            for (int i = 0; i < spawnAmount; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(6f, 6f);

                Vector2 dustVel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * 0.18f * Main.rand.NextFloat(0.8f, 1.6f);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    DustID.Vortex,
                    dustVel,
                    0,
                    default,
                    Main.rand.NextFloat(0.8f, 1.6f)
                );
                d.noGravity = true;
                d.fadeIn = 0.05f;

                d.velocity += Projectile.velocity * 0.03f;
            }


            if (Main.rand.NextBool(6))
            {
                Vector2 bigVel = Main.rand.NextVector2Circular(3f, 3f) + Projectile.velocity * Main.rand.NextFloat(0.15f, 0.4f);
                Dust big = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                    DustID.Vortex,
                    bigVel,
                    0,
                    default,
                    Main.rand.NextFloat(1.6f, 2.6f)
                );
                big.noGravity = true;
                big.fadeIn = 0.15f;
            }



            Projectile.rotation += Projectile.ai[1];

            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;


            Lighting.AddLight(Projectile.Center, 0.18f, 0.36f, 0.65f);
        }

        public override void OnKill(int timeLeft)
        {

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item105, Projectile.position);


            int count = 48;
            for (int i = 0; i < count; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(6f, 6f);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + vel,
                    DustID.Vortex,
                    vel * Main.rand.NextFloat(0.9f, 1.8f),
                    0,
                    default,
                    Main.rand.NextFloat(1.4f, 2.6f)
                );
                d.noGravity = true;
                d.fadeIn = 0.2f;
            }

            for (int i = 0; i < 6; i++)
                Lighting.AddLight(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f), 0.22f, 0.45f, 0.78f);
        }
    }
}
