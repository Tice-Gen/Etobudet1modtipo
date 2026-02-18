using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.DataStructures;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.Systems
{
    public class WorldGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (index != -1)
                tasks.Insert(index + 1, new PassLegacy("Generate Anise Forest (spawn area)", GenerateAniseForest));
        }

        private void GenerateAniseForest(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Growing Anise Forest near spawn...";
            Mod.Logger.Info("Etobudet1modtipo: AniseForest generation started near spawn.");

            int worldW = Main.maxTilesX;
            int worldH = Main.maxTilesY;
            int biomeWidth = 60;
            int semicircleDepth = 30;

            int spawnX = Main.spawnTileX;

            int biomeLeft = spawnX - biomeWidth / 2;
            int biomeRight = spawnX + biomeWidth / 2;

            biomeLeft = Math.Max(10, biomeLeft);
            biomeRight = Math.Min(worldW - 10, biomeRight);

            HashSet<ushort> replaceable = new()
            {
                TileID.Dirt,
                TileID.Mud,
                TileID.ClayBlock,
                TileID.Silt,
                TileID.Grass,
                TileID.Sand,
                TileID.HardenedSand
            };

            var rnd = WorldGen.genRand;


            for (int x = biomeLeft; x <= biomeRight; x++)
            {
                int surfaceY = -1;
                for (int y = 0; y < worldH; y++)
                {
                    Tile t = Framing.GetTileSafely(x, y);
                    if (t.HasTile && Main.tileSolid[t.TileType])
                    {
                        surfaceY = y;
                        break;
                    }
                }
                if (surfaceY == -1) continue;


                Tile topTile = Framing.GetTileSafely(x, surfaceY);
                if (topTile.HasTile && replaceable.Contains(topTile.TileType))
                {
                    topTile.TileType = (ushort)ModContent.TileType<AniseGrassTile>();
                    WorldGen.SquareTileFrame(x, surfaceY, true);
                }


                int maxSubDepth = 5;
                int extraCount = rnd.Next(0, 3);
                for (int i = 0; i < extraCount; i++)
                {
                    int dy = rnd.Next(1, maxSubDepth + 1);
                    int py = surfaceY + dy;
                    if (py <= 1 || py >= worldH - 2) continue;

                    Tile below = Framing.GetTileSafely(x, py);
                    if (below.HasTile && replaceable.Contains(below.TileType))
                    {
                        below.TileType = (ushort)ModContent.TileType<AniseGrassTile>();
                        WorldGen.SquareTileFrame(x, py, true);
                    }
                }


                if (rnd.NextBool(12))
                {
                    int fx = x;
                    int bottomY = surfaceY - 1;
                    if (bottomY <= 1 || bottomY >= worldH - 1) continue;


                    if (Main.tile[fx, bottomY].HasTile) continue;

                    int tFlower1 = ModContent.TileType<AniseFlower1>();
                    int tFlower2 = ModContent.TileType<AniseFlower2>();
                    int tFlowerBig = ModContent.TileType<AniseFlowerBig>();

                    int choice = rnd.Next(3);
                    int flowerTile = (choice == 0) ? tFlower1 : (choice == 1) ? tFlower2 : tFlowerBig;
                    int flowerHeight = (flowerTile == tFlowerBig) ? 1 : 2;

                    bool canPlace = true;
                    for (int yy = bottomY; yy > bottomY - flowerHeight; yy--)
                    {
                        if (yy <= 0 || yy >= worldH - 1) { canPlace = false; break; }
                        if (Main.tile[fx, yy].HasTile) { canPlace = false; break; }
                    }

                    if (!canPlace) continue;


                    if (flowerHeight == 1)
                    {
                        bool placed = WorldGen.PlaceTile(fx, bottomY, flowerTile, mute: true, forced: true);
                        if (!placed) continue;
                    }
                    else
                    {
                        int topY = bottomY - (flowerHeight - 1);
                        bool placed = WorldGen.PlaceObject(fx, topY, flowerTile, mute: true);
                        if (!placed) continue;
                    }

                    WorldGen.SquareTileFrame(fx, bottomY, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, fx, bottomY, 3);
                }
            }



            int centerX = (biomeLeft + biomeRight) / 2;
            int semicircleRadius = Math.Max(1, (biomeRight - biomeLeft) / 2);
            int hradius = semicircleRadius;
            int vradius = semicircleDepth;


            int centerSurfaceY = -1;
            for (int y = 0; y < worldH; y++)
            {
                Tile t = Framing.GetTileSafely(centerX, y);
                if (t.HasTile && Main.tileSolid[t.TileType])
                {
                    centerSurfaceY = y;
                    break;
                }
            }

            if (centerSurfaceY != -1)
            {
                for (int dx = -hradius; dx <= hradius; dx++)
                {
                    int px = centerX + dx;
                    if (px < 0 || px >= worldW) continue;


                    double normalized = 1.0 - ((double)dx * dx) / ((double)hradius * hradius);
                    if (normalized <= 0.0) continue;
                    int yExtent = (int)Math.Floor(Math.Sqrt(normalized) * vradius);
                    if (yExtent <= 0) continue;

                    int startY = centerSurfaceY + 1;
                    int endY = centerSurfaceY + yExtent;
                    endY = Math.Min(endY, worldH - 2);

                    for (int py = startY; py <= endY; py++)
                    {
                        Tile cur = Framing.GetTileSafely(px, py);

                        if (!cur.HasTile)
                        {

                            cur.HasTile = true;
                            cur.TileType = (ushort)TileID.Mud;
                            cur.TileFrameX = 0;
                            cur.TileFrameY = 0;
                        }
                        else if (replaceable.Contains(cur.TileType))
                        {
                            cur.TileType = (ushort)TileID.Mud;
                        }
                        else
                        {

                            continue;
                        }



                        if (rnd.Next(100) < 12)
                        {
                            cur.TileType = (ushort)ModContent.TileType<AniseGrassTile>();
                        }

                        WorldGen.SquareTileFrame(px, py, true);

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, px, py, 3);
                    }
                }
            }


            int midX = (biomeLeft + biomeRight) / 2;
            int area = Math.Max(10, biomeWidth / 2);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, midX, (int)Main.worldSurface, area);

            for (int xx = biomeLeft; xx <= biomeRight; xx += 10)
                for (int yy = (int)Main.worldSurface - 20; yy <= Main.worldSurface + 40; yy += 10)
                    WorldGen.TileFrame(xx, yy, true, false);

            Mod.Logger.Info($"Etobudet1modtipo: AniseForest generated near spawn X={spawnX}, area={biomeLeft}-{biomeRight}");
        }
    }
}
