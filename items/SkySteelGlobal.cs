using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class SkySteelGlobal : GlobalItem
    {
        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.type != ModContent.ItemType<SkySteel>())
                return;


            gravity = 0f;
            maxFallSpeed = 0f;
            item.velocity *= 0f;


            float time = (Main.GameUpdateCount + item.whoAmI * 15) * 0.05f;
            float bob = (float)Math.Sin(time) * 0.6f;
            item.position.Y += bob;


            if (Main.netMode == NetmodeID.Server)
                return;


            Vector2 center = item.position + new Vector2(item.width * 0.5f, item.height * 0.5f);


            float baseRadius = 28f;
            float ringRotationSpeed = 0.9f;
            float tiltA = MathHelper.ToRadians(25f);
            float tiltB = MathHelper.ToRadians(-25f);
            float spawnProbabilityPerTick = 0.18f;
            int particlesPerSpawn = 3;


            for (int ringIndex = 0; ringIndex < 2; ringIndex++)
            {
                if (Main.rand.NextFloat() > spawnProbabilityPerTick)
                    continue;

                float tilt = (ringIndex == 0) ? tiltA : tiltB;

                float radius = baseRadius + ringIndex * 6f;


                for (int i = 0; i < particlesPerSpawn; i++)
                {

                    float angle = time * ringRotationSpeed + i * MathHelper.TwoPi / particlesPerSpawn + Main.rand.NextFloat(-0.18f, 0.18f);
                    Vector2 ringPos = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;


                    ringPos = ringPos.RotatedBy(tilt);

                    Vector2 spawnPos = center + ringPos;


                    Vector2 tangent = ringPos.RotatedBy(MathHelper.PiOver2);
                    tangent.Normalize();
                    Vector2 velocity = tangent * (0.35f + Main.rand.NextFloat() * 0.25f);


                    int dustType = DustID.Mercury;
                    Dust d = Dust.NewDustPerfect(spawnPos, dustType, velocity, 0, default, 1.0f);
                    if (d != null)
                    {
                        d.noGravity = true;
                        d.scale = 0.9f + Main.rand.NextFloat() * 0.8f;
                        d.fadeIn = 0.4f + Main.rand.NextFloat() * 0.8f;
                        d.velocity *= 0.7f;
                    }
                }
            }


            Lighting.AddLight(center, 0.12f, 0.08f, 0.18f);
        }
    }
}
