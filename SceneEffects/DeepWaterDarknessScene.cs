using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.SceneEffects
{
    public class DeepWaterDarknessScene : ModSystem
    {
        private const int DeepWaterStartTiles = 30;
        private static float cachedDarkness;

        public override void PostUpdateEverything()
        {
            if (Main.dedServ)
            {
                return;
            }

            cachedDarkness = CalculateDeepWaterDarkness(Main.LocalPlayer);
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            float darkness = MathHelper.Clamp(cachedDarkness, 0f, 1f);
            if (darkness <= 0.02f)
            {
                return;
            }

            float tileMul = 1f - darkness * 0.42f;
            float backMul = 1f - darkness * 0.52f;

            tileColor = new Color(
                (byte)MathHelper.Clamp(tileColor.R * tileMul * 0.78f, 0f, 255f),
                (byte)MathHelper.Clamp(tileColor.G * tileMul * 0.86f, 0f, 255f),
                (byte)MathHelper.Clamp(tileColor.B * tileMul * 0.96f, 0f, 255f),
                tileColor.A
            );

            backgroundColor = new Color(
                (byte)MathHelper.Clamp(backgroundColor.R * backMul * 0.7f, 0f, 255f),
                (byte)MathHelper.Clamp(backgroundColor.G * backMul * 0.82f, 0f, 255f),
                (byte)MathHelper.Clamp(backgroundColor.B * backMul * 0.94f, 0f, 255f),
                backgroundColor.A
            );
        }

        private static float CalculateDeepWaterDarkness(Player player)
        {
            if (!player.active || player.dead || !player.wet)
            {
                return 0f;
            }

            int centerX = (int)(player.Center.X / 16f);
            int centerY = (int)(player.Center.Y / 16f);
            int[] sampleOffsets = { -5, 0, 5 };

            float depthFactorSum = 0f;
            float roofFactorSum = 0f;
            int samples = 0;

            foreach (int offset in sampleOffsets)
            {
                int sampleX = Utils.Clamp(centerX + offset, 1, Main.maxTilesX - 2);
                int surfaceY = FindWaterSurfaceY(sampleX, centerY);
                int waterDepthTiles = Math.Max(0, centerY - surfaceY);
                float depthFactor = MathHelper.Clamp((waterDepthTiles - DeepWaterStartTiles) / 60f, 0f, 1f);
                float roofFactor = GetRoofCoverageFactor(sampleX, surfaceY);

                depthFactorSum += depthFactor;
                roofFactorSum += roofFactor;
                samples++;
            }

            if (samples == 0)
            {
                return 0f;
            }

            float avgDepthFactor = depthFactorSum / samples;
            float avgRoofFactor = roofFactorSum / samples;
            float worldDepthFactor = MathHelper.Clamp((centerY - (float)Main.worldSurface) / 360f, 0f, 1f);

            float darkness = avgDepthFactor * 0.65f + avgRoofFactor * 0.2f + worldDepthFactor * 0.15f;
            return MathHelper.Clamp(darkness, 0f, 1f);
        }

        private static int FindWaterSurfaceY(int x, int startY)
        {
            int minY = Math.Max(1, startY - 180);
            int surfaceY = startY;

            for (int y = startY; y >= minY; y--)
            {
                Tile tile = Main.tile[x, y];
                bool isWater = tile != null && tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 100;
                if (!isWater)
                {
                    return y + 1;
                }

                surfaceY = y;
            }

            return surfaceY;
        }

        private static float GetRoofCoverageFactor(int x, int surfaceY)
        {
            int checkTop = Math.Max(surfaceY - 70, 1);
            int solidCount = 0;
            int total = 0;

            for (int y = surfaceY - 1; y >= checkTop; y--)
            {
                Tile tile = Main.tile[x, y];
                if (tile == null)
                {
                    continue;
                }

                total++;
                if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                {
                    solidCount++;
                }
            }

            if (total == 0)
            {
                return 0f;
            }

            return MathHelper.Clamp(solidCount / (float)total, 0f, 1f);
        }
    }
}
