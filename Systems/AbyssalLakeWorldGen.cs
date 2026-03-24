using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Etobudet1modtipo.Systems
{
    public sealed class AbyssalLakeWorldGen : ModSystem
    {
        private const int MaxRegionCells = 120000;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(pass => pass.Name.Equals("Micro Biomes"));
            if (index == -1)
            {
                index = tasks.FindIndex(pass => pass.Name.Equals("Final Cleanup"));
            }

            if (index == -1)
            {
                index = tasks.FindIndex(pass => pass.Name.Equals("Lakes"));
            }

            if (index == -1)
            {
                index = tasks.FindIndex(pass => pass.Name.Equals("Shinies"));
            }

            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy("Abyssal Lake", GenerateAbyssalLake));
            }
        }

        private static void GenerateAbyssalLake(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Carving abyssal lake...";

            int oceanDistance = WorldGen.oceanDistance > 0 ? WorldGen.oceanDistance : 320;
            if (!TryFindEdgeLakeCandidate(oceanDistance, out LakeCandidate lake) &&
                !TryCreateFallbackLake(oceanDistance, out lake))
            {
                return;
            }

            if (!TryGetMouthRun(lake, out int mouthY, out int mouthMinX, out int mouthMaxX))
            {
                mouthY = Math.Min(lake.MaxY, lake.MinY + Math.Max(1, lake.Depth / 2));
                mouthMinX = lake.MinX;
                mouthMaxX = lake.MaxX;
            }

            int anchorX = (mouthMinX + mouthMaxX) / 2;
            int originY = Math.Clamp(mouthY, 30, Main.maxTilesY - 600);

            int minBottom = Math.Max(lake.MaxY + 300, (int)Main.rockLayer + 280);
            int maxBottom = Main.maxTilesY - 420;
            if (maxBottom < minBottom + 80)
            {
                maxBottom = Main.maxTilesY - 300;
            }

            if (maxBottom <= minBottom + 20)
            {
                return;
            }

            int bottomY = WorldGen.genRand.Next(minBottom, maxBottom + 1);
            int totalDepth = bottomY - originY;
            if (totalDepth < 180)
            {
                return;
            }

            int mouthHalfWidth = Math.Clamp((mouthMaxX - mouthMinX + 1) / 2, 7, 22);
            int startHalfWidth = mouthHalfWidth;
            int bellyHalfWidth = Math.Clamp(startHalfWidth + WorldGen.genRand.Next(10, 20), 18, 36);
            int shaftHalfWidth = Math.Clamp(bellyHalfWidth - WorldGen.genRand.Next(3, 8), 13, 28);
            int tipHalfWidth = WorldGen.genRand.Next(3, 6);

            int neckDepth = WorldGen.genRand.Next(28, 46);
            int bellyDepth = WorldGen.genRand.Next(150, 250);
            int taperDepth = WorldGen.genRand.Next(120, 170);
            int bellyEndDepth = Math.Min(totalDepth - taperDepth, neckDepth + bellyDepth);
            bellyEndDepth = Math.Max(bellyEndDepth, neckDepth + 30);
            int shaftEndDepth = Math.Max(bellyEndDepth + 30, totalDepth - taperDepth);

            Dictionary<int, int> halfWidthByY = new Dictionary<int, int>(totalDepth + 8);
            Dictionary<int, int> centerOffsetByY = new Dictionary<int, int>(totalDepth + 8);

            for (int y = originY; y <= bottomY + 3; y++)
            {
                if (!WorldGen.InWorld(anchorX, y, 2))
                {
                    continue;
                }

                int depth = y - originY;
                float halfWidth = GetAbyssHalfWidth(
                    depth,
                    totalDepth,
                    neckDepth,
                    bellyEndDepth,
                    shaftEndDepth,
                    startHalfWidth,
                    bellyHalfWidth,
                    shaftHalfWidth,
                    tipHalfWidth);

                int centerOffset = 0;
                if (depth > neckDepth)
                {
                    float swayA = (float)Math.Sin(depth * 0.017f + anchorX * 0.015f) * 3.0f;
                    float swayB = (float)Math.Sin(depth * 0.0075f + anchorX * 0.021f) * 2.0f;
                    centerOffset = (int)Math.Round(swayA + swayB);
                }

                int jitter = (Hash2D(anchorX / 7, y / 9, anchorX, y) % 3) - 1;
                int carveHalfWidth = Math.Max(4, (int)Math.Round(halfWidth) + jitter);

                halfWidthByY[y] = carveHalfWidth;
                centerOffsetByY[y] = centerOffset;

                int localCenter = anchorX + centerOffset;
                for (int x = localCenter - carveHalfWidth; x <= localCenter + carveHalfWidth; x++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    float edge = Math.Abs(x - localCenter) / (float)(carveHalfWidth + 0.5f);
                    if (edge > 1.1f)
                    {
                        continue;
                    }

                    int edgeNoise = Hash2D(x / 3, y / 3, anchorX, originY) & 7;
                    if (edge > 0.9f && edgeNoise < 2)
                    {
                        continue;
                    }

                    Tile tile = Main.tile[x, y];
                    tile.ClearEverything();
                    tile.LiquidType = LiquidID.Water;
                    tile.LiquidAmount = byte.MaxValue;
                    tile.IsHalfBlock = false;
                    tile.Slope = SlopeType.Solid;
                }
            }

            StabilizeAbyssWalls(anchorX, originY, bottomY, halfWidthByY, centerOffsetByY);
            FrameAbyss(anchorX, originY, bottomY, bellyHalfWidth);
        }

        private static bool TryFindEdgeLakeCandidate(int oceanDistance, out LakeCandidate lake)
        {
            lake = default;
            HashSet<int> visited = new HashSet<int>(8192);

            bool leftFirst = WorldGen.genRand.NextBool();
            bool foundA = TryScanSideForLake(leftFirst, oceanDistance, visited, out LakeCandidate candA, out int scoreA);
            bool foundB = TryScanSideForLake(!leftFirst, oceanDistance, visited, out LakeCandidate candB, out int scoreB);

            if (!foundA && !foundB)
            {
                return false;
            }

            if (foundA && (!foundB || scoreA <= scoreB))
            {
                lake = candA;
                return true;
            }

            lake = candB;
            return true;
        }

        private static bool TryScanSideForLake(
            bool leftSide,
            int oceanDistance,
            HashSet<int> visited,
            out LakeCandidate bestLake,
            out int bestScore)
        {
            bestLake = default;
            bestScore = int.MaxValue;

            int minX = leftSide ? 40 : Math.Max(40, Main.maxTilesX - oceanDistance - 260);
            int maxX = leftSide ? Math.Min(Main.maxTilesX - 40, oceanDistance + 260) : Main.maxTilesX - 40;
            if (maxX <= minX + 20)
            {
                return false;
            }

            int minY = Math.Max(40, (int)Main.worldSurface - 30);
            int maxY = Math.Min(Main.maxTilesY - 320, (int)Main.worldSurface + 220);

            for (int x = minX; x <= maxX; x += 6)
            {
                for (int y = minY; y <= maxY; y += 4)
                {
                    if (!IsWaterCell(x, y))
                    {
                        continue;
                    }

                    int key = TileKey(x, y);
                    if (!visited.Add(key))
                    {
                        continue;
                    }

                    LakeCandidate region = CollectWaterRegion(x, y, visited);
                    if (!region.IsValid)
                    {
                        continue;
                    }

                    if (region.CellCount < 50 || region.CellCount > MaxRegionCells - 10)
                    {
                        continue;
                    }

                    if (region.Depth < 6 || region.Width < 12)
                    {
                        continue;
                    }

                    if (region.MinY > Main.worldSurface + 70)
                    {
                        continue;
                    }

                    if (!TryGetMouthRun(region, out int runY, out int runMinX, out int runMaxX))
                    {
                        continue;
                    }

                    int mouthWidth = runMaxX - runMinX + 1;
                    int runDepth = EstimateRunDepth(runMinX, runMaxX, runY, 320);
                    if (runDepth < 4)
                    {
                        continue;
                    }

                    int skyColumns = CountRunSkyColumns(runMinX, runMaxX, runY, 24);
                    if (skyColumns < Math.Max(3, mouthWidth / 4))
                    {
                        continue;
                    }

                    int centerX = (runMinX + runMaxX) / 2;
                    int edgeDistance = leftSide ? centerX : Main.maxTilesX - 1 - centerX;
                    int surfacePenalty = Math.Abs(runY - ((int)Main.worldSurface + 12));
                    int fallingRisk = EvaluateFallingRisk(centerX, runY);
                    int widthPenalty = Math.Abs(mouthWidth - 22) * 2 + Math.Max(0, mouthWidth - 70) * 3;
                    int depthReward = Math.Min(runDepth * 10, 420) + Math.Min(region.Depth * 2, 140);
                    int score = edgeDistance * 3 + surfacePenalty * 2 + fallingRisk + widthPenalty - depthReward - skyColumns * 4;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestLake = region;
                    }
                }
            }

            return bestLake.IsValid;
        }

        private static LakeCandidate CollectWaterRegion(int seedX, int seedY, HashSet<int> visited)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(seedX, seedY));

            int minX = seedX;
            int maxX = seedX;
            int minY = seedY;
            int maxY = seedY;
            int deepestX = seedX;
            int deepestY = seedY;
            int cellCount = 0;

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                if (!WorldGen.InWorld(p.X, p.Y, 1) || !IsWaterCell(p.X, p.Y))
                {
                    continue;
                }

                cellCount++;
                if (cellCount > MaxRegionCells)
                {
                    return default;
                }

                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY)
                {
                    maxY = p.Y;
                    deepestX = p.X;
                    deepestY = p.Y;
                }

                EnqueueNeighbor(p.X + 1, p.Y, visited, queue);
                EnqueueNeighbor(p.X - 1, p.Y, visited, queue);
                EnqueueNeighbor(p.X, p.Y + 1, visited, queue);
                EnqueueNeighbor(p.X, p.Y - 1, visited, queue);
            }

            return new LakeCandidate(minX, maxX, minY, maxY, deepestX, deepestY, cellCount);
        }

        private static bool TryGetMouthRun(LakeCandidate lake, out int mouthY, out int mouthMinX, out int mouthMaxX)
        {
            mouthY = lake.MinY;
            mouthMinX = lake.DeepestX;
            mouthMaxX = lake.DeepestX;

            int bestScore = int.MinValue;
            int scanTop = Math.Max(20, lake.MinY - 8);
            int scanBottom = Math.Min(lake.MaxY, lake.MinY + 120);

            for (int y = scanTop; y <= scanBottom; y++)
            {
                int runStart = int.MinValue;
                for (int x = lake.MinX; x <= lake.MaxX + 1; x++)
                {
                    bool water = x <= lake.MaxX && IsWaterCell(x, y);
                    if (water)
                    {
                        if (runStart == int.MinValue)
                        {
                            runStart = x;
                        }

                        continue;
                    }

                    if (runStart == int.MinValue)
                    {
                        continue;
                    }

                    int runEnd = x - 1;
                    int width = runEnd - runStart + 1;
                    if (width >= 6 && width <= 96 &&
                        HasSolidBank(runStart - 1, y) &&
                        HasSolidBank(runEnd + 1, y))
                    {
                        int runDepth = EstimateRunDepth(runStart, runEnd, y, 320);
                        if (runDepth >= 3)
                        {
                            int skyColumns = CountRunSkyColumns(runStart, runEnd, y, 22);
                            int widthPenalty = Math.Abs(width - 22) * 2 + Math.Max(0, width - 72) * 3;
                            int surfacePenalty = Math.Abs(y - ((int)Main.worldSurface + 12));
                            int score = runDepth * 11 + skyColumns * 5 - widthPenalty - surfacePenalty;

                            if (score > bestScore)
                            {
                                bestScore = score;
                                mouthY = y;
                                mouthMinX = runStart;
                                mouthMaxX = runEnd;
                            }
                        }
                    }

                    runStart = int.MinValue;
                }
            }

            return bestScore > int.MinValue;
        }

        private static bool HasSolidBank(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 2))
            {
                return false;
            }

            for (int yy = y - 1; yy <= y + 2; yy++)
            {
                Tile t = Framing.GetTileSafely(x, yy);
                if (t.HasTile && Main.tileSolid[t.TileType])
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountRunSkyColumns(int runMinX, int runMaxX, int y, int scanHeight)
        {
            int topY = Math.Max(5, y - scanHeight);
            int count = 0;

            for (int x = runMinX; x <= runMaxX; x++)
            {
                bool blocked = false;
                for (int yy = y - 1; yy >= topY; yy--)
                {
                    Tile t = Framing.GetTileSafely(x, yy);
                    if (t.HasTile && Main.tileSolid[t.TileType])
                    {
                        blocked = true;
                        break;
                    }
                }

                if (!blocked)
                {
                    count++;
                }
            }

            return count;
        }

        private static int EstimateRunDepth(int runMinX, int runMaxX, int y, int maxDepth)
        {
            int bestDepth = 0;
            int minX = runMinX + 1;
            int maxX = runMaxX - 1;
            if (minX > maxX)
            {
                minX = runMinX;
                maxX = runMaxX;
            }

            int width = Math.Max(1, maxX - minX + 1);
            int step = Math.Max(1, width / 6);

            for (int x = minX; x <= maxX; x += step)
            {
                int localDeepestY = y;
                int misses = 0;
                int endY = Math.Min(Main.maxTilesY - 15, y + maxDepth);

                for (int yy = y; yy <= endY; yy++)
                {
                    if (IsWaterCell(x, yy))
                    {
                        localDeepestY = yy;
                        misses = 0;
                    }
                    else
                    {
                        misses++;
                        if (misses >= 3)
                        {
                            break;
                        }
                    }
                }

                int localDepth = localDeepestY - y;
                if (localDepth > bestDepth)
                {
                    bestDepth = localDepth;
                }
            }

            return bestDepth;
        }

        private static void EnqueueNeighbor(int x, int y, HashSet<int> visited, Queue<Point> queue)
        {
            if (!WorldGen.InWorld(x, y, 1))
            {
                return;
            }

            int key = TileKey(x, y);
            if (visited.Add(key))
            {
                queue.Enqueue(new Point(x, y));
            }
        }

        private static bool TryCreateFallbackLake(int oceanDistance, out LakeCandidate lake)
        {
            lake = default;

            bool leftSide = WorldGen.genRand.NextBool();
            int minX = leftSide ? 70 : Main.maxTilesX - oceanDistance + 50;
            int maxX = leftSide ? oceanDistance - 70 : Main.maxTilesX - 70;
            if (maxX <= minX + 10)
            {
                return false;
            }

            int bestX = 0;
            int bestY = 0;
            int bestScore = int.MaxValue;
            for (int i = 0; i < 36; i++)
            {
                int x = WorldGen.genRand.Next(minX, maxX);
                int y = Math.Max(45, FindSurfaceY(x) - 3);
                int score = EvaluateFallingRisk(x, y);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestX = x;
                    bestY = y;
                }
            }

            if (bestX == 0)
            {
                return false;
            }

            int halfWidth = WorldGen.genRand.Next(18, 27);
            int depth = WorldGen.genRand.Next(10, 18);
            int minWaterX = bestX;
            int maxWaterX = bestX;
            int minWaterY = bestY;
            int maxWaterY = bestY;
            int deepestX = bestX;
            int deepestY = bestY;
            int count = 0;

            for (int x = bestX - halfWidth; x <= bestX + halfWidth; x++)
            {
                if (!WorldGen.InWorld(x, bestY, 1))
                {
                    continue;
                }

                float n = Math.Abs(x - bestX) / (float)halfWidth;
                float bowl = 1f - n * n;
                if (bowl <= 0f)
                {
                    continue;
                }

                int localDepth = Math.Max(2, (int)Math.Round(depth * bowl));
                for (int y = bestY; y <= bestY + localDepth; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    Tile t = Main.tile[x, y];
                    t.ClearEverything();
                    t.LiquidType = LiquidID.Water;
                    t.LiquidAmount = byte.MaxValue;
                    t.IsHalfBlock = false;
                    t.Slope = SlopeType.Solid;

                    if (x < minWaterX) minWaterX = x;
                    if (x > maxWaterX) maxWaterX = x;
                    if (y < minWaterY) minWaterY = y;
                    if (y > maxWaterY)
                    {
                        maxWaterY = y;
                        deepestX = x;
                        deepestY = y;
                    }

                    count++;
                }
            }

            if (count < 40)
            {
                return false;
            }

            // Harden nearby falling tiles so the fallback lake keeps its shape.
            for (int x = minWaterX - 4; x <= maxWaterX + 4; x++)
            {
                for (int y = minWaterY - 10; y <= maxWaterY + 4; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    Tile t = Main.tile[x, y];
                    if (!t.HasTile || !IsFallingTile(t.TileType))
                    {
                        continue;
                    }

                    t.TileType = GetStableReplacement(t.TileType, y);
                    t.IsHalfBlock = false;
                    t.Slope = SlopeType.Solid;
                }
            }

            for (int x = minWaterX - 5; x <= maxWaterX + 5; x++)
            {
                for (int y = minWaterY - 5; y <= maxWaterY + 5; y++)
                {
                    if (WorldGen.InWorld(x, y, 1))
                    {
                        WorldGen.TileFrame(x, y, true, false);
                    }
                }
            }

            lake = new LakeCandidate(minWaterX, maxWaterX, minWaterY, maxWaterY, deepestX, deepestY, count);
            return true;
        }

        private static void StabilizeAbyssWalls(
            int anchorX,
            int topY,
            int bottomY,
            Dictionary<int, int> halfWidthByY,
            Dictionary<int, int> centerOffsetByY)
        {
            for (int y = topY - 30; y <= bottomY + 8; y++)
            {
                if (!halfWidthByY.TryGetValue(y, out int halfWidth))
                {
                    continue;
                }

                int offset = centerOffsetByY.TryGetValue(y, out int o) ? o : 0;
                int localCenter = anchorX + offset;
                int minX = localCenter - halfWidth - 8;
                int maxX = localCenter + halfWidth + 8;

                for (int x = minX; x <= maxX; x++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    Tile tile = Main.tile[x, y];
                    bool insideCavity = x >= localCenter - halfWidth && x <= localCenter + halfWidth;
                    if (insideCavity)
                    {
                        continue;
                    }

                    if (!HasCavityNeighbor(x, y))
                    {
                        continue;
                    }

                    bool nearCavity = x >= localCenter - halfWidth - 2 && x <= localCenter + halfWidth + 2;
                    if (!nearCavity)
                    {
                        continue;
                    }

                    if (!tile.HasTile)
                    {
                        // Seal side caverns that steal water from the abyss shaft.
                        tile.HasTile = true;
                        tile.TileType = (ushort)(y <= Main.worldSurface + 90 ? TileID.Dirt : TileID.Stone);
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                        continue;
                    }

                    if (IsFallingTile(tile.TileType))
                    {
                        tile.TileType = GetStableReplacement(tile.TileType, y);
                        tile.IsHalfBlock = false;
                        tile.Slope = SlopeType.Solid;
                        tile.LiquidAmount = 0;
                    }
                }
            }
        }

        private static void FrameAbyss(int centerX, int topY, int bottomY, int halfWidth)
        {
            int padX = halfWidth + 16;
            int padY = 14;
            for (int x = centerX - padX; x <= centerX + padX; x++)
            {
                for (int y = topY - padY; y <= bottomY + padY; y++)
                {
                    if (WorldGen.InWorld(x, y, 1))
                    {
                        WorldGen.TileFrame(x, y, true, false);
                    }
                }
            }
        }

        private static int EvaluateFallingRisk(int centerX, int surfaceY)
        {
            int score = 0;
            int maxY = Math.Min(Main.maxTilesY - 10, surfaceY + 240);
            for (int x = centerX - 10; x <= centerX + 10; x++)
            {
                if (x < 5 || x >= Main.maxTilesX - 5)
                {
                    continue;
                }

                for (int y = surfaceY - 6; y <= maxY; y += 4)
                {
                    Tile t = Framing.GetTileSafely(x, y);
                    if (!t.HasTile)
                    {
                        continue;
                    }

                    if (IsFallingTile(t.TileType))
                    {
                        score += 3;
                    }
                    else if (IsDesertLike(t.TileType))
                    {
                        score += 1;
                    }
                }
            }

            return score;
        }

        private static float GetAbyssHalfWidth(
            int depth,
            int totalDepth,
            int neckDepth,
            int bellyEndDepth,
            int shaftEndDepth,
            int startHalfWidth,
            int bellyHalfWidth,
            int shaftHalfWidth,
            int tipHalfWidth)
        {
            if (depth <= 0)
            {
                return startHalfWidth;
            }

            if (depth < neckDepth)
            {
                float t = depth / (float)Math.Max(1, neckDepth);
                return MathHelper.Lerp(startHalfWidth, bellyHalfWidth, SmoothStep01(t));
            }

            if (depth < bellyEndDepth)
            {
                float t = (depth - neckDepth) / (float)Math.Max(1, bellyEndDepth - neckDepth);
                return MathHelper.Lerp(bellyHalfWidth, shaftHalfWidth, t * 0.45f);
            }

            if (depth < shaftEndDepth)
            {
                return shaftHalfWidth;
            }

            float tailT = (depth - shaftEndDepth) / (float)Math.Max(1, totalDepth - shaftEndDepth);
            return MathHelper.Lerp(shaftHalfWidth, tipHalfWidth, SmoothStep01(tailT));
        }

        private static bool IsWaterCell(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
            {
                return false;
            }

            Tile t = Framing.GetTileSafely(x, y);
            return !t.HasTile && t.LiquidAmount > 0 && t.LiquidType == LiquidID.Water;
        }

        private static bool HasCavityNeighbor(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int tx = x + dx;
                    int ty = y + dy;
                    if (!WorldGen.InWorld(tx, ty, 1))
                    {
                        continue;
                    }

                    Tile n = Main.tile[tx, ty];
                    if (!n.HasTile || n.LiquidAmount > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsFallingTile(int tileType)
        {
            return tileType == TileID.Sand ||
                   tileType == TileID.Ebonsand ||
                   tileType == TileID.Crimsand ||
                   tileType == TileID.Pearlsand ||
                   tileType == TileID.Silt ||
                   tileType == TileID.Slush ||
                   tileType == TileID.DesertFossil;
        }

        private static bool IsDesertLike(int tileType)
        {
            return tileType == TileID.Sand ||
                   tileType == TileID.Ebonsand ||
                   tileType == TileID.Crimsand ||
                   tileType == TileID.Pearlsand ||
                   tileType == TileID.HardenedSand ||
                   tileType == TileID.CorruptHardenedSand ||
                   tileType == TileID.CrimsonHardenedSand ||
                   tileType == TileID.HallowHardenedSand ||
                   tileType == TileID.Sandstone ||
                   tileType == TileID.CorruptSandstone ||
                   tileType == TileID.CrimsonSandstone ||
                   tileType == TileID.HallowSandstone ||
                   tileType == TileID.DesertFossil;
        }

        private static ushort GetStableReplacement(int tileType, int y)
        {
            return tileType switch
            {
                TileID.Sand => TileID.HardenedSand,
                TileID.Ebonsand => TileID.CorruptHardenedSand,
                TileID.Crimsand => TileID.CrimsonHardenedSand,
                TileID.Pearlsand => TileID.HallowHardenedSand,
                TileID.DesertFossil => TileID.Sandstone,
                TileID.Silt => TileID.Stone,
                TileID.Slush => TileID.IceBlock,
                _ => (ushort)(y <= Main.worldSurface + 90 ? TileID.Dirt : TileID.Stone),
            };
        }

        private static int FindSurfaceY(int x)
        {
            int maxScan = (int)Main.worldSurface + 220;
            for (int y = 40; y < maxScan; y++)
            {
                Tile t = Framing.GetTileSafely(x, y);
                if (t.HasTile && Main.tileSolid[t.TileType])
                {
                    return y;
                }
            }

            return (int)Main.worldSurface;
        }

        private static int TileKey(int x, int y) => x + y * Main.maxTilesX;

        private static float SmoothStep01(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        private static int Hash2D(int x, int y, int sx, int sy)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + sx * 224682251 + sy * 326648991;
                h = (h ^ (h >> 13)) * 1274126177;
                return h ^ (h >> 16);
            }
        }

        private readonly struct LakeCandidate
        {
            public readonly int MinX;
            public readonly int MaxX;
            public readonly int MinY;
            public readonly int MaxY;
            public readonly int DeepestX;
            public readonly int DeepestY;
            public readonly int CellCount;

            public int Width => MaxX - MinX + 1;
            public int Depth => MaxY - MinY + 1;
            public bool IsValid => CellCount > 0;

            public LakeCandidate(int minX, int maxX, int minY, int maxY, int deepestX, int deepestY, int cellCount)
            {
                MinX = minX;
                MaxX = maxX;
                MinY = minY;
                MaxY = maxY;
                DeepestX = deepestX;
                DeepestY = deepestY;
                CellCount = cellCount;
            }
        }
    }
}
