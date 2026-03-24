using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.Subworlds
{
    public static class OtherWatersSubworld
    {
        public const string Name = "OtherWaters";
        public const int IslandWidthTiles = 100;
        public const int OceanSurfaceTileY = 140;
        public const int LogicalMaxOceanDepthTiles = 10000;
        public const int PhysicalMaxOceanDepthTiles = 2060;
        public const int MaxOceanDepthTiles = LogicalMaxOceanDepthTiles;
        public const int FloorThicknessTiles = 120;
        public const int FloorStartTileY = OceanSurfaceTileY + PhysicalMaxOceanDepthTiles;
        public const int WorldWidthTiles = 1200;
        public const int WorldHeightTiles = FloorStartTileY + FloorThicknessTiles + 80;
        public const int TunnelBreakDistanceFromFloorTiles = 1000;

        private const int IslandBaseTopY = OceanSurfaceTileY - 16;
        private const int IslandPeakExtraHeight = 8;
        private const int SphereMinDiameterTiles = 100;
        private const int SphereMaxDiameterTiles = 360;
        private const float SphereMinAspectRatio = 0.96f;
        private const float SphereMaxAspectRatio = 1.04f;

        public static List<GenPass> BuildTasks()
        {
            return new List<GenPass>
            {
                new PassLegacy("Other Waters Terrain", GenerateOtherWaters)
            };
        }

        private static void GenerateOtherWaters(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Filling Other Waters...";

            int deepSeaStoneTile = ModContent.TileType<DeepSeaStoneTile>();
            int titanStoneTile = ResolveStaticId(typeof(TileID), deepSeaStoneTile,
                "TitanstoneBlock",
                "Titanstone",
                "TitanStone");
            int mudstoneTile = ResolveStaticId(typeof(TileID), TileID.Mud,
                "Mudstone",
                "MudstoneBlock",
                "MudstoneBrick");
            int floorTile = deepSeaStoneTile;
            int[] floorTopByX = BuildFloorProfile();
            int[] tunnelStopByX = BuildTunnelStopProfile(floorTopByX);

            Main.worldSurface = OceanSurfaceTileY;
            Main.rockLayer = OceanSurfaceTileY + 300;

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                float columnProgress = x / (float)Math.Max(1, Main.maxTilesX - 1);
                progress.Set(columnProgress * 0.65f);
                int floorTop = floorTopByX[x];
                int tunnelStopY = tunnelStopByX[x];

                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearEverything();

                    if (y < tunnelStopY)
                    {
                        int caveNoise = Hash2D(x / 12, y / 12, x, y) & 15;
                        tile.HasTile = true;
                        tile.TileType = (ushort)(caveNoise < 5 ? mudstoneTile : titanStoneTile);
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                    }
                    else if (y >= tunnelStopY && y < floorTop)
                    {
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = byte.MaxValue;
                    }
                    else if (y >= floorTop)
                    {
                        tile.HasTile = true;
                        tile.TileType = (ushort)floorTile;
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                    }
                }
            }

            int[] wallPool =
            {
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "TitanstoneBlockWall",
                    "TitanstoneWall",
                    "TitanStoneWall"),
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "FracturedStoneWall",
                    "FracturedStoneWallUnsafe",
                    "FracturedStone"),
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "StalactiteStoneWall",
                    "StalactiteStoneWallUnsafe",
                    "StalactiteStone")
            };

            GenerateDeepSeaStoneVeins(floorTopByX, deepSeaStoneTile);
            PlaceSpawnInCave(tunnelStopByX, titanStoneTile, mudstoneTile);
            GenerateLabyrinthFromSpawn(Main.spawnTileX, Main.spawnTileY - 2, tunnelStopByX, wallPool);

            for (int x = 2; x < Main.maxTilesX - 2; x++)
            {
                int tunnelStop = tunnelStopByX[x];
                for (int y = tunnelStop - 2; y <= tunnelStop + 2; y++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        WorldGen.TileFrame(x, y, true, false);
                    }
                }

                int floorTop = floorTopByX[x];
                for (int y = floorTop - 2; y <= floorTop + 2; y++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        WorldGen.TileFrame(x, y, true, false);
                    }
                }
            }

            progress.Set(1f);
        }

        private static List<OceanSphere> GenerateMainOceanSphereClusters(int[] tunnelStopByX, int titanStoneTile, int mudstoneTile)
        {
            int[] wallPool =
            {
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "TitanstoneBlockWall",
                    "TitanstoneWall",
                    "TitanStoneWall"),
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "FracturedStoneWall",
                    "FracturedStoneWallUnsafe",
                    "FracturedStone"),
                ResolveStaticId(typeof(WallID), WallID.Stone,
                    "StalactiteStoneWall",
                    "StalactiteStoneWallUnsafe",
                    "StalactiteStone")
            };

            int oreLuminite = ResolveStaticId(typeof(TileID), -1,
                "LunarOre",
                "LuminiteOre");

            int sphereCount = Math.Clamp(Main.maxTilesX / 120, 8, 14);
            int minRadius = SphereMinDiameterTiles / 2;
            int maxRadius = SphereMaxDiameterTiles / 2;
            List<OceanSphere> spheres = new List<OceanSphere>(sphereCount);
            List<Point16> tunnelPoints = new List<Point16>(2000);

            for (int attempt = 0; attempt < sphereCount * 10 && spheres.Count < sphereCount; attempt++)
            {
                int baseRadius = WorldGen.genRand.Next(minRadius, maxRadius + 1);
                int radiusVariance = Math.Max(3, (int)(baseRadius * 0.08f));
                int rx = Math.Clamp(baseRadius + WorldGen.genRand.Next(-radiusVariance, radiusVariance + 1), minRadius, maxRadius);
                int ry = Math.Clamp(baseRadius + WorldGen.genRand.Next(-radiusVariance, radiusVariance + 1), minRadius, maxRadius);

                float aspect = rx / (float)ry;
                if (aspect < SphereMinAspectRatio)
                {
                    rx = Math.Clamp((int)Math.Round(ry * SphereMinAspectRatio), minRadius, maxRadius);
                }
                else if (aspect > SphereMaxAspectRatio)
                {
                    ry = Math.Clamp((int)Math.Round(rx / SphereMaxAspectRatio), minRadius, maxRadius);
                }

                int centerX = WorldGen.genRand.Next(rx + 30, Main.maxTilesX - rx - 30);
                int minY = OceanSurfaceTileY + ry + 40;
                int maxY = tunnelStopByX[centerX] - ry - 24;
                if (maxY <= minY)
                {
                    continue;
                }

                int centerY = WorldGen.genRand.Next(minY, maxY);
                OceanSphere sphere = new OceanSphere(centerX, centerY, rx, ry);
                if (IntersectsAny(spheres, sphere))
                {
                    continue;
                }

                FillSphereWithWallsAndStone(sphere, titanStoneTile, mudstoneTile, wallPool);
                CarveSphereTunnels(sphere, wallPool, tunnelPoints, tunnelStopByX);
                spheres.Add(sphere);
            }

            if (oreLuminite >= 0 && spheres.Count > 0)
            {
                int veinCount = WorldGen.genRand.Next(5, 11);
                for (int i = 0; i < veinCount; i++)
                {
                    PlaceLuminiteVeinInSpheres(spheres, tunnelPoints, oreLuminite);
                }
            }

            return spheres;
        }

        private static bool IntersectsAny(List<OceanSphere> spheres, OceanSphere candidate)
        {
            for (int i = 0; i < spheres.Count; i++)
            {
                OceanSphere s = spheres[i];
                int dx = Math.Abs(candidate.CenterX - s.CenterX);
                int dy = Math.Abs(candidate.CenterY - s.CenterY);
                int maxDx = candidate.RadiusX + s.RadiusX - 20;
                int maxDy = candidate.RadiusY + s.RadiusY - 20;
                if (dx < maxDx && dy < maxDy)
                {
                    return true;
                }
            }

            return false;
        }

        private static void FillSphereWithWallsAndStone(OceanSphere sphere, int titanStoneTile, int mudstoneTile, int[] wallPool)
        {
            int minX = sphere.CenterX - sphere.RadiusX;
            int maxX = sphere.CenterX + sphere.RadiusX;
            int minY = sphere.CenterY - sphere.RadiusY;
            int maxY = sphere.CenterY + sphere.RadiusY;

            for (int x = minX; x <= maxX; x++)
            {
                if (x < 1 || x >= Main.maxTilesX - 1)
                {
                    continue;
                }

                float nx = (x - sphere.CenterX) / (float)sphere.RadiusX;
                float nx2 = nx * nx;

                for (int y = minY; y <= maxY; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    float ny = (y - sphere.CenterY) / (float)sphere.RadiusY;
                    if (nx2 + ny * ny > 1f)
                    {
                        continue;
                    }

                    float edgeDistance = nx2 + ny * ny;
                    float edgeBlend = MathHelper.Clamp((edgeDistance - 0.72f) / 0.28f, 0f, 1f);
                    int patchNoise = Hash2D(x / 6, y / 6, sphere.CenterX, sphere.CenterY) & 31;
                    bool isMudPatch = patchNoise < 8;

                    Tile tile = Main.tile[x, y];
                    tile.HasTile = true;
                    tile.TileType = (ushort)((edgeBlend > 0.55f || isMudPatch) ? mudstoneTile : titanStoneTile);
                    tile.IsHalfBlock = false;
                    tile.Slope = SlopeType.Solid;
                    tile.LiquidAmount = 0;
                    int wallNoise = Hash2D(x / 10, y / 10, sphere.CenterX * 3, sphere.CenterY * 3);
                    tile.WallType = (ushort)wallPool[Math.Abs(wallNoise) % wallPool.Length];
                }
            }
        }

        private static void CarveSphereTunnels(OceanSphere sphere, int[] wallPool, List<Point16> tunnelPoints, int[] tunnelStopByX)
        {
            int tunnelCount = WorldGen.genRand.Next(2, 5);
            for (int i = 0; i < tunnelCount; i++)
            {
                bool through = WorldGen.genRand.NextBool(3);
                float angle = WorldGen.genRand.NextFloat() * MathHelper.TwoPi;
                float dirX = (float)Math.Cos(angle);
                float dirY = (float)Math.Sin(angle);
                int width = WorldGen.genRand.Next(5, 13);

                float startX;
                float startY;
                int length;
                if (through)
                {
                    float span = Math.Min(sphere.RadiusX, sphere.RadiusY) * 0.95f;
                    startX = sphere.CenterX - dirX * span;
                    startY = sphere.CenterY - dirY * span;
                    length = (int)(span * 1.35f) + WorldGen.genRand.Next(15, 55);
                }
                else
                {
                    float span = Math.Min(sphere.RadiusX, sphere.RadiusY) * 0.95f;
                    startX = sphere.CenterX + dirX * span;
                    startY = sphere.CenterY + dirY * span;
                    dirX = -dirX;
                    dirY = -dirY;
                    length = (int)(span * WorldGen.genRand.NextFloat(0.35f, 0.85f));
                }

                CarveTunnel((int)startX, (int)startY, dirX, dirY, length, width, wallPool, tunnelPoints, tunnelStopByX);
            }
        }

        private static void CarveTunnel(int startX, int startY, float dirX, float dirY, int length, int width, int[] wallPool, List<Point16> tunnelPoints, int[] tunnelStopByX)
        {
            float x = startX;
            float y = startY;
            float widthWavePhase = WorldGen.genRand.NextFloat(MathHelper.TwoPi);

            for (int step = 0; step < length; step++)
            {
                if (!WorldGen.InWorld((int)x, (int)y, width + 2))
                {
                    break;
                }

                int currentX = Math.Clamp((int)x, 0, Main.maxTilesX - 1);
                if ((int)y >= tunnelStopByX[currentX] - 2)
                {
                    break;
                }

                float widthWave = 0.84f + 0.16f * (float)Math.Sin(widthWavePhase + step * 0.14f);
                int radius = Math.Max(2, (int)Math.Round(width * 0.5f * widthWave));
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        if (dx * dx + dy * dy > radius * radius)
                        {
                            continue;
                        }

                        int tx = (int)x + dx;
                        int ty = (int)y + dy;
                        if (!WorldGen.InWorld(tx, ty, 1))
                        {
                            continue;
                        }

                        if (ty >= tunnelStopByX[tx])
                        {
                            continue;
                        }

                        Tile tile = Main.tile[tx, ty];
                        tile.HasTile = false;
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = byte.MaxValue;
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.WallType = (ushort)wallPool[WorldGen.genRand.Next(wallPool.Length)];
                    }
                }

                if (step % 7 == 0)
                {
                    tunnelPoints.Add(new Point16((short)x, (short)y));
                }

                dirX += WorldGen.genRand.NextFloat(-0.12f, 0.12f);
                dirY += WorldGen.genRand.NextFloat(-0.12f, 0.12f);
                Vector2 direction = new Vector2(dirX, dirY);
                if (direction.LengthSquared() < 0.0001f)
                {
                    direction = Vector2.UnitY;
                }
                else
                {
                    direction.Normalize();
                }

                dirX = direction.X;
                dirY = direction.Y;
                x += dirX;
                y += dirY;
            }
        }

        private static void PlaceSpawnInCave(int[] tunnelStopByX, int titanStoneTile, int mudstoneTile)
        {
            int spawnX = Main.maxTilesX / 2 + WorldGen.genRand.Next(-80, 81);
            int spawnY = OceanSurfaceTileY + WorldGen.genRand.Next(170, 241);

            spawnY = Math.Min(spawnY, tunnelStopByX[Math.Clamp(spawnX, 0, Main.maxTilesX - 1)] - 120);
            CarveSpawnChamber(spawnX, spawnY, titanStoneTile, mudstoneTile);
            Main.spawnTileX = spawnX;
            Main.spawnTileY = spawnY + 2;
        }

        private static void GenerateLabyrinthFromSpawn(int startX, int startY, int[] tunnelStopByX, int[] wallPool)
        {
            Queue<TunnelBranch> branches = new Queue<TunnelBranch>();
            int initialStop = tunnelStopByX[Math.Clamp(startX, 0, Main.maxTilesX - 1)];
            int initialSteps = Math.Max(220, (initialStop - startY) + 180);

            float initialDirX = WorldGen.genRand.NextFloat(-0.28f, 0.28f);
            float initialDirY = 1f;
            branches.Enqueue(new TunnelBranch(startX, startY + 2, initialDirX, initialDirY, initialSteps, WorldGen.genRand.Next(7, 11), 0));

            // Seed horizontal exploration from the spawn cave so the maze spreads left/right.
            int sideSeedCount = 8;
            for (int i = 0; i < sideSeedCount; i++)
            {
                float t = (i + 1) / (float)(sideSeedCount + 1);
                int y = startY + (int)Math.Round((initialStop - startY) * t * 0.92f);
                int horizontal = WorldGen.genRand.NextBool() ? 1 : -1;
                float dirX = horizontal * WorldGen.genRand.NextFloat(0.7f, 0.98f);
                float dirY = WorldGen.genRand.NextFloat(0.08f, 0.28f);
                int steps = WorldGen.genRand.Next(80, 200);
                int width = WorldGen.genRand.Next(5, 10);
                branches.Enqueue(new TunnelBranch(startX, y, dirX, dirY, steps, width, 1));
            }

            int processed = 0;
            const int maxBranches = 420;
            while (branches.Count > 0 && processed < maxBranches)
            {
                TunnelBranch branch = branches.Dequeue();
                processed++;
                CarveLabyrinthBranch(branch, tunnelStopByX, wallPool, branches);
            }
        }

        private static void CarveLabyrinthBranch(TunnelBranch branch, int[] tunnelStopByX, int[] wallPool, Queue<TunnelBranch> branches)
        {
            float x = branch.StartX;
            float y = branch.StartY;
            float dirX = branch.DirX;
            float dirY = branch.DirY;
            float widthWavePhase = WorldGen.genRand.NextFloat(MathHelper.TwoPi);

            for (int step = 0; step < branch.Steps; step++)
            {
                if (!WorldGen.InWorld((int)x, (int)y, branch.Width + 2))
                {
                    break;
                }

                int cx = Math.Clamp((int)x, 0, Main.maxTilesX - 1);
                int stopY = tunnelStopByX[cx];
                if ((int)y >= stopY - 2)
                {
                    break;
                }

                float widthWave = 0.82f + 0.18f * (float)Math.Sin(widthWavePhase + step * 0.15f);
                int radius = Math.Max(2, (int)Math.Round(branch.Width * 0.5f * widthWave));

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        if (dx * dx + dy * dy > radius * radius)
                        {
                            continue;
                        }

                        int tx = (int)x + dx;
                        int ty = (int)y + dy;
                        if (!WorldGen.InWorld(tx, ty, 1) || ty >= tunnelStopByX[tx])
                        {
                            continue;
                        }

                        Tile tile = Main.tile[tx, ty];
                        tile.HasTile = false;
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = byte.MaxValue;
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.WallType = (ushort)wallPool[WorldGen.genRand.Next(wallPool.Length)];
                    }
                }

                if (step > 18 && step % 10 == 0 && branch.Depth < 5 && WorldGen.genRand.NextFloat() < 0.2f)
                {
                    float turn;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        // Prefer side branches instead of always downward forks.
                        turn = (WorldGen.genRand.NextBool() ? 1f : -1f) * WorldGen.genRand.NextFloat(1.15f, 1.9f);
                    }
                    else
                    {
                        turn = (WorldGen.genRand.NextBool() ? 1f : -1f) * WorldGen.genRand.NextFloat(0.45f, 1.1f);
                    }

                    Vector2 newDir = new Vector2(dirX, dirY).RotatedBy(turn);
                    float depthDownBias = branch.Depth <= 1 ? 0.1f : 0.02f;
                    newDir.Y = MathHelper.Clamp(newDir.Y + depthDownBias, -0.35f, 0.96f);
                    if (newDir.LengthSquared() > 0.0001f)
                    {
                        newDir.Normalize();
                    }
                    else
                    {
                        newDir = Vector2.UnitY;
                    }

                    int newSteps = (int)(branch.Steps * WorldGen.genRand.NextFloat(0.4f, 0.82f));
                    int newWidth = Math.Clamp(branch.Width + WorldGen.genRand.Next(-2, 3), 4, 12);
                    branches.Enqueue(new TunnelBranch((int)x, (int)y, newDir.X, newDir.Y, newSteps, newWidth, branch.Depth + 1));
                }

                dirX += WorldGen.genRand.NextFloat(-0.16f, 0.16f);
                dirY += WorldGen.genRand.NextFloat(-0.12f, 0.12f);
                dirY += branch.Depth <= 1 ? 0.03f : 0.008f;

                Vector2 direction = new Vector2(dirX, dirY);
                if (direction.LengthSquared() < 0.0001f)
                {
                    direction = Vector2.UnitY;
                }
                else
                {
                    direction.Normalize();
                }

                if (branch.Depth == 0)
                {
                    direction.Y = Math.Max(0.34f, direction.Y);
                }
                else
                {
                    direction.Y = MathHelper.Clamp(direction.Y, -0.42f, 0.95f);
                }

                direction.Normalize();
                dirX = direction.X;
                dirY = direction.Y;
                x += dirX;
                y += dirY;
            }
        }

        private static void CarveSpawnChamber(int centerX, int centerY, int titanStoneTile, int mudstoneTile)
        {
            const int chamberRadiusX = 20;
            const int chamberRadiusY = 13;

            for (int x = centerX - chamberRadiusX - 2; x <= centerX + chamberRadiusX + 2; x++)
            {
                for (int y = centerY - chamberRadiusY - 2; y <= centerY + chamberRadiusY + 2; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    float nx = (x - centerX) / (float)chamberRadiusX;
                    float ny = (y - centerY) / (float)chamberRadiusY;
                    float d = nx * nx + ny * ny;
                    Tile tile = Main.tile[x, y];

                    if (d <= 1f)
                    {
                        tile.HasTile = false;
                        tile.LiquidAmount = 0;
                        tile.WallType = WallID.Stone;
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                    }
                    else if (d <= 1.25f)
                    {
                        int noise = Hash2D(x / 4, y / 4, centerX, centerY) & 7;
                        tile.HasTile = true;
                        tile.TileType = (ushort)(noise < 3 ? mudstoneTile : titanStoneTile);
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                        tile.WallType = WallID.Stone;
                    }
                }
            }
        }

        private static int Hash2D(int x, int y, int seedX, int seedY)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + seedX * 224682251 + seedY * 326648991;
                h = (h ^ (h >> 13)) * 1274126177;
                return h ^ (h >> 16);
            }
        }

        private static void PlaceLuminiteVeinInSpheres(List<OceanSphere> spheres, List<Point16> tunnelPoints, int oreLuminite)
        {
            int startX;
            int startY;

            if (tunnelPoints.Count > 0 && WorldGen.genRand.NextBool(3))
            {
                Point16 point = tunnelPoints[WorldGen.genRand.Next(tunnelPoints.Count)];
                startX = point.X + WorldGen.genRand.Next(-7, 8);
                startY = point.Y + WorldGen.genRand.Next(-7, 8);
            }
            else
            {
                OceanSphere sphere = spheres[WorldGen.genRand.Next(spheres.Count)];
                int tries = 0;
                do
                {
                    startX = WorldGen.genRand.Next(sphere.CenterX - sphere.RadiusX + 4, sphere.CenterX + sphere.RadiusX - 3);
                    startY = WorldGen.genRand.Next(sphere.CenterY - sphere.RadiusY + 4, sphere.CenterY + sphere.RadiusY - 3);
                    tries++;
                } while (tries < 20 && !sphere.Contains(startX, startY));
            }

            int steps = WorldGen.genRand.Next(16, 42);
            int x = startX;
            int y = startY;
            for (int i = 0; i < steps; i++)
            {
                if (!WorldGen.InWorld(x, y, 2))
                {
                    break;
                }

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int tx = x + dx;
                        int ty = y + dy;
                        if (!WorldGen.InWorld(tx, ty, 1))
                        {
                            continue;
                        }

                        Tile tile = Main.tile[tx, ty];
                        if (!tile.HasTile)
                        {
                            continue;
                        }

                        if (WorldGen.genRand.NextBool(2))
                        {
                            tile.TileType = (ushort)oreLuminite;
                            tile.IsHalfBlock = false;
                            tile.Slope = SlopeType.Solid;
                            tile.LiquidAmount = 0;
                        }
                    }
                }

                x += WorldGen.genRand.Next(-1, 2);
                y += WorldGen.genRand.Next(-1, 2);
            }
        }

        private static int ResolveStaticId(Type idType, int fallback, params string[] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                FieldInfo field = idType.GetField(candidates[i], BindingFlags.Public | BindingFlags.Static);
                if (field == null)
                {
                    continue;
                }

                object value = field.GetValue(null);
                if (value is int intId)
                {
                    return intId;
                }

                if (value is ushort ushortId)
                {
                    return ushortId;
                }

                if (value is short shortId)
                {
                    return shortId;
                }

                if (value is byte byteId)
                {
                    return byteId;
                }
            }

            return fallback;
        }

        public static int ToLogicalDepth(int physicalDepth)
        {
            if (physicalDepth <= 0)
            {
                return 0;
            }

            if (physicalDepth >= PhysicalMaxOceanDepthTiles)
            {
                return LogicalMaxOceanDepthTiles;
            }

            return (int)Math.Round(physicalDepth * (LogicalMaxOceanDepthTiles / (double)PhysicalMaxOceanDepthTiles));
        }

        private static int[] BuildFloorProfile()
        {
            int[] profile = new int[Main.maxTilesX];
            int[] styles = BuildFloorStyles();
            int current = FloorStartTileY;
            int minTop = FloorStartTileY - 180;
            int maxTop = Math.Min(FloorStartTileY + 45, Main.maxTilesY - FloorThicknessTiles - 4);

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int style = styles[x];
                switch (style)
                {
                    case 0:
                        current += WorldGen.genRand.Next(-1, 2);
                        if (WorldGen.genRand.NextBool(10))
                        {
                            current += WorldGen.genRand.Next(-2, 3);
                        }
                        break;
                    case 1:
                        current += WorldGen.genRand.Next(-3, 4);
                        if (WorldGen.genRand.NextBool(5))
                        {
                            current += WorldGen.genRand.Next(-6, 7);
                        }
                        break;
                    default:
                        current += WorldGen.genRand.Next(-2, 3);
                        if (x % 36 == 0)
                        {
                            current += WorldGen.genRand.Next(-10, 11);
                        }
                        break;
                }

                current = Math.Clamp(current, minTop, maxTop);
                profile[x] = current;
            }

            for (int pass = 0; pass < 2; pass++)
            {
                int[] smoothed = new int[Main.maxTilesX];
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    int sum = 0;
                    int count = 0;

                    for (int dx = -3; dx <= 3; dx++)
                    {
                        int sx = x + dx;
                        if (sx < 0 || sx >= Main.maxTilesX)
                        {
                            continue;
                        }

                        sum += profile[sx];
                        count++;
                    }

                    smoothed[x] = sum / Math.Max(1, count);
                }

                profile = smoothed;
            }

            for (int x = 25; x < Main.maxTilesX - 75; x++)
            {
                if (WorldGen.genRand.NextFloat() >= 0.003f)
                {
                    continue;
                }

                int flatTop = profile[x];
                for (int i = 0; i < 50 && x + i < Main.maxTilesX; i++)
                {
                    profile[x + i] = flatTop;
                }

                x += 49;
            }

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                profile[x] = Math.Clamp(profile[x], minTop, maxTop);
            }

            return profile;
        }

        private static int[] BuildFloorStyles()
        {
            int[] styles = new int[Main.maxTilesX];
            int x = 0;
            while (x < Main.maxTilesX)
            {
                int segmentLength = WorldGen.genRand.Next(70, 190);
                int style = WorldGen.genRand.Next(3);
                int end = Math.Min(Main.maxTilesX, x + segmentLength);
                for (int i = x; i < end; i++)
                {
                    styles[i] = style;
                }

                x = end;
            }

            return styles;
        }

        private static int[] BuildTunnelStopProfile(int[] floorTopByX)
        {
            int[] profile = new int[Main.maxTilesX];
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int floorTop = floorTopByX[x];
                int minStop = OceanSurfaceTileY + 260;
                int maxStop = floorTop - 120;
                int stop = floorTop - TunnelBreakDistanceFromFloorTiles;
                profile[x] = Math.Clamp(stop, minStop, maxStop);
            }

            int[] smoothed = new int[Main.maxTilesX];
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int sum = 0;
                int count = 0;
                for (int dx = -4; dx <= 4; dx++)
                {
                    int sx = x + dx;
                    if (sx < 0 || sx >= Main.maxTilesX)
                    {
                        continue;
                    }

                    sum += profile[sx];
                    count++;
                }

                smoothed[x] = sum / Math.Max(1, count);
            }

            return smoothed;
        }

        private static void GenerateDeepSeaStoneVeins(int[] floorTopByX, int deepSeaStoneTile)
        {
            int attempts = Main.maxTilesX / 90;
            for (int i = 0; i < attempts; i++)
            {
                if (WorldGen.genRand.NextFloat() > 0.38f)
                {
                    continue;
                }

                int startX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int minY = floorTopByX[startX] + 6;
                int maxY = Math.Min(Main.maxTilesY - 10, floorTopByX[startX] + FloorThicknessTiles - 8);
                if (minY >= maxY)
                {
                    continue;
                }

                int startY = WorldGen.genRand.Next(minY, maxY);
                int veinLength = WorldGen.genRand.Next(5, 31);
                int x = startX;
                int y = startY;

                for (int step = 0; step < veinLength; step++)
                {
                    if (!WorldGen.InWorld(x, y, 3))
                    {
                        break;
                    }

                    int floorTop = floorTopByX[x];
                    if (y >= floorTop + 2 && y < floorTop + FloorThicknessTiles - 2)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.HasTile = true;
                        tile.TileType = (ushort)deepSeaStoneTile;
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                    }

                    x += WorldGen.genRand.Next(-1, 2);
                    y += WorldGen.genRand.Next(-1, 2);
                }
            }
        }

        private static int GetIslandTopY(int dx, int halfIsland)
        {
            float normalized = Math.Abs(dx) / (float)Math.Max(1, halfIsland);
            float curve = 1f - normalized * normalized;
            if (curve < 0f)
            {
                curve = 0f;
            }

            int peakOffset = (int)Math.Round(curve * IslandPeakExtraHeight);
            return IslandBaseTopY - peakOffset;
        }

        private static int GetIslandBottomY(int dx, int halfIsland)
        {
            float normalized = Math.Abs(dx) / (float)Math.Max(1, halfIsland);
            float thicknessCurve = 1f - normalized;
            if (thicknessCurve < 0f)
            {
                thicknessCurve = 0f;
            }

            int thickness = 14 + (int)Math.Round(thicknessCurve * 22f);
            return IslandBaseTopY + thickness;
        }

        private readonly struct OceanSphere
        {
            public readonly int CenterX;
            public readonly int CenterY;
            public readonly int RadiusX;
            public readonly int RadiusY;

            public OceanSphere(int centerX, int centerY, int radiusX, int radiusY)
            {
                CenterX = centerX;
                CenterY = centerY;
                RadiusX = radiusX;
                RadiusY = radiusY;
            }

            public bool Contains(int x, int y)
            {
                float nx = (x - CenterX) / (float)RadiusX;
                float ny = (y - CenterY) / (float)RadiusY;
                return nx * nx + ny * ny <= 1f;
            }
        }

        private readonly struct TunnelBranch
        {
            public readonly int StartX;
            public readonly int StartY;
            public readonly float DirX;
            public readonly float DirY;
            public readonly int Steps;
            public readonly int Width;
            public readonly int Depth;

            public TunnelBranch(int startX, int startY, float dirX, float dirY, int steps, int width, int depth)
            {
                StartX = startX;
                StartY = startY;
                DirX = dirX;
                DirY = dirY;
                Steps = steps;
                Width = width;
                Depth = depth;
            }
        }
    }
}
