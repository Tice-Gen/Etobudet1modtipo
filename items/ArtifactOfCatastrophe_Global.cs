using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Etobudet1modtipo.items
{

    public class ArtifactOfCatastrophe_Global : GlobalItem
    {

        public override void PostUpdate(Item item)
        {

            if (item.type != ModContent.ItemType<ArtifactOfCatastrophe>()) return;


            if (Main.netMode == NetmodeID.Server) return;


            const int totalParticles = 15;
            const int starPoints = 5;
            const float radius = 95f;
            const int particlesPerSegment = totalParticles / starPoints;
            int[] starOrder = new int[] { 0, 2, 4, 1, 3, 0 };


            float rotation = Main.GameUpdateCount * 0.035f;

            Vector2 center = item.Center;


            Vector2[] verts = new Vector2[starPoints];
            for (int i = 0; i < starPoints; i++)
            {
                float angle = i * MathHelper.TwoPi / starPoints - MathHelper.PiOver2;
                verts[i] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            }


            if (Main.GameUpdateCount % 4 != 0)
                return;

            for (int seg = 0; seg < starPoints; seg++)
            {
                Vector2 a = verts[starOrder[seg]];
                Vector2 b = verts[starOrder[seg + 1]];

                for (int k = 0; k < particlesPerSegment; k++)
                {
                    float t = (k + 1f) / (particlesPerSegment + 1f);
                    Vector2 localPos = Vector2.Lerp(a, b, t);
                    localPos = localPos.RotatedBy(rotation);

                    Vector2 spawnPos = center + localPos;


                    Dust d = Dust.NewDustPerfect(
                        spawnPos,
                        DustID.PurpleTorch,
                        Vector2.Zero,
                        0,
                        new Color(200, 110, 255),
                        1.35f
                    );

                    if (d != null)
                    {
                        d.noGravity = true;
                        d.velocity = Vector2.Zero;
                        d.fadeIn = 0.9f;
                        d.scale = 1.15f + Main.rand.NextFloat(0f, 0.35f);


                        Lighting.AddLight(spawnPos, 0.55f, 0.12f, 0.7f);
                    }
                }
            }
        }



    }
}
