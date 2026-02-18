using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Etobudet1modtipo.Tiles;
using Terraria.Localization;

namespace Etobudet1modtipo.Common.Systems
{
    public class SeaSaltWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (index != -1)
            {
                tasks.Insert(index + 1, new SeaSaltGenPass("Sea Salt Gen", 180f));
            }
        }
    }

    public class SeaSaltGenPass : GenPass
    {
        public SeaSaltGenPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Etobudet1modtipo.CreatingSeaSalt", "Seeding Sea Salt...");

            int seaSaltTile = ModContent.TileType<SeaSaltTile>();
            int saltCrystalTile = ModContent.TileType<SaltCrystalTile>();

            PlaceInOceans(seaSaltTile);
            PlaceInLakes(seaSaltTile);
            PlaceSaltCrystals(seaSaltTile, saltCrystalTile);
        }

        private static void PlaceInOceans(int seaSaltTile)
        {
            int oceanDistance = WorldGen.oceanDistance;
            if (oceanDistance <= 0)
            {
                oceanDistance = 300;
            }

            int clustersPerOcean = (int)(Main.maxTilesX / 4200f * 40f);
            if (clustersPerOcean < 30) clustersPerOcean = 30;
            if (clustersPerOcean > 90) clustersPerOcean = 90;

            for (int side = 0; side < 2; side++)
            {
                int minX = (side == 0) ? 50 : Main.maxTilesX - oceanDistance + 50;
                int maxX = (side == 0) ? oceanDistance - 50 : Main.maxTilesX - 50;
                if (minX >= maxX) continue;

                for (int i = 0; i < clustersPerOcean; i++)
                {
                    int x = WorldGen.genRand.Next(minX, maxX);
                    if (TryFindOceanFloor(x, out int floorY))
                    {
                        PlaceCluster(seaSaltTile, x, floorY, WorldGen.genRand);
                    }
                }
            }
        }

        private static void PlaceInLakes(int seaSaltTile)
        {
            int oceanDistance = WorldGen.oceanDistance;
            if (oceanDistance <= 0)
            {
                oceanDistance = 300;
            }

            int lakeAttempts = (int)(Main.maxTilesX / 4200f * 220f);
            if (lakeAttempts < 200) lakeAttempts = 200;
            if (lakeAttempts > 450) lakeAttempts = 450;

            for (int attempt = 0; attempt < lakeAttempts; attempt++)
            {
                int x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                if (x < oceanDistance + 80 || x > Main.maxTilesX - oceanDistance - 80) continue;

                int y = WorldGen.genRand.Next((int)Main.worldSurface + 20, Main.maxTilesY - 300);
                Tile start = Framing.GetTileSafely(x, y);
                if (start.HasTile) continue;
                if (start.LiquidAmount <= 0 || start.LiquidType != LiquidID.Water) continue;

                if (!TryGetSmallLake(new Point(x, y), out List<Point> waterCells))
                    continue;

                int waterCount = waterCells.Count;
                if (waterCount < 40) continue;
                if (WorldGen.genRand.NextFloat() >= 0.40f) continue;

                int baseX = waterCells[0].X;
                int bottomY = waterCells[0].Y;
                for (int i = 1; i < waterCells.Count; i++)
                {
                    if (waterCells[i].Y > bottomY)
                    {
                        bottomY = waterCells[i].Y;
                        baseX = waterCells[i].X;
                    }
                }

                int baseY = bottomY + 1;
                for (int dy = 0; dy < 6; dy++)
                {
                    int ty = baseY + dy;
                    if (!WorldGen.InWorld(baseX, ty, 1)) break;

                    Tile floor = Framing.GetTileSafely(baseX, ty);
                    if (floor.HasTile && Main.tileSolid[floor.TileType])
                    {
                        PlaceCluster(seaSaltTile, baseX, ty, WorldGen.genRand);
                        break;
                    }
                }
            }
        }

        private static bool TryFindOceanFloor(int x, out int floorY)
        {
            int startY = (int)Main.worldSurface;
            int endY = Main.maxTilesY - 200;

            for (int y = startY; y < endY; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (!tile.HasTile || !Main.tileSolid[tile.TileType]) continue;

                Tile above = Framing.GetTileSafely(x, y - 1);
                if (above.LiquidAmount > 0 && above.LiquidType == LiquidID.Water && !above.HasTile)
                {
                    floorY = y;
                    return true;
                }
            }

            floorY = -1;
            return false;
        }

        private static void PlaceCluster(int seaSaltTile, int baseX, int baseY, UnifiedRandom rand)
        {
            int count = rand.Next(10, 16);
            int placed = 0;
            int attempts = 0;

            while (placed < count && attempts < count * 8)
            {
                attempts++;
                int x = baseX + rand.Next(-3, 4);
                int y = baseY + rand.Next(-2, 3);
                if (!WorldGen.InWorld(x, y, 1)) continue;

                Tile tile = Framing.GetTileSafely(x, y);
                if (!tile.HasTile) continue;
                if (!Main.tileSolid[tile.TileType]) continue;

                tile.TileType = (ushort)seaSaltTile;
                tile.HasTile = true;
                WorldGen.SquareTileFrame(x, y, true);
                placed++;
            }
        }

        private static bool TryGetSmallLake(Point start, out List<Point> waterCells)
        {
            waterCells = new List<Point>(64);

            Queue<Point> queue = new Queue<Point>();
            HashSet<Point> visited = new HashSet<Point>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                if (!WorldGen.InWorld(p.X, p.Y, 1)) continue;

                Tile tile = Framing.GetTileSafely(p.X, p.Y);
                if (tile.HasTile) continue;
                if (tile.LiquidAmount <= 0 || tile.LiquidType != LiquidID.Water) continue;

                waterCells.Add(p);
                if (waterCells.Count > 60) return false;

                Point n;
                n = new Point(p.X + 1, p.Y);
                if (!visited.Contains(n)) { visited.Add(n); queue.Enqueue(n); }
                n = new Point(p.X - 1, p.Y);
                if (!visited.Contains(n)) { visited.Add(n); queue.Enqueue(n); }
                n = new Point(p.X, p.Y + 1);
                if (!visited.Contains(n)) { visited.Add(n); queue.Enqueue(n); }
                n = new Point(p.X, p.Y - 1);
                if (!visited.Contains(n)) { visited.Add(n); queue.Enqueue(n); }
            }

            return waterCells.Count > 0;
        }

        private static void PlaceSaltCrystals(int seaSaltTile, int saltCrystalTile)
        {
            int attempts = (int)(Main.maxTilesX / 4200f * 2200f);
            if (attempts < 1600) attempts = 1600;
            if (attempts > 3800) attempts = 3800;

            for (int k = 0; k < attempts; k++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.maxTilesY - 220);
                if (!WorldGen.InWorld(x, y, 2)) continue;

                Tile anchor = Framing.GetTileSafely(x, y);
                if (!anchor.HasTile || anchor.TileType != seaSaltTile) continue;
                if (anchor.IsHalfBlock || anchor.Slope != SlopeType.Solid) continue;

                TryPlaceSaltCrystalFromAnchor(x, y, saltCrystalTile);
            }
        }

        private static void TryPlaceSaltCrystalFromAnchor(int x, int y, int saltCrystalTile)
        {
            int first = WorldGen.genRand.Next(4);
            for (int step = 0; step < 4; step++)
            {
                int dir = (first + step) % 4;
                int tx = x;
                int ty = y;
                int alt = 0;

                switch (dir)
                {
                    case 0:
                        ty = y - 1;
                        alt = 0;
                        break;
                    case 1:
                        ty = y + 1;
                        alt = 1;
                        break;
                    case 2:
                        tx = x - 1;
                        alt = 2;
                        break;
                    default:
                        tx = x + 1;
                        alt = 3;
                        break;
                }

                if (!WorldGen.InWorld(tx, ty, 2)) continue;
                Tile target = Framing.GetTileSafely(tx, ty);
                if (target.HasTile) continue;
                if (target.LiquidAmount > 130) continue;

                bool placed = WorldGen.PlaceObject(
                    tx,
                    ty,
                    saltCrystalTile,
                    mute: true,
                    style: 0,
                    alternate: alt,
                    random: WorldGen.genRand.Next(3));

                if (placed)
                {
                    Tile placedTile = Framing.GetTileSafely(tx, ty);
                    placedTile.TileFrameY = (short)(alt * 18);
                    break;
                }
            }
        }


    }
}
