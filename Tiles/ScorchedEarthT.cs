using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class ScorchedEarthT : ModTile
    {
        private const int DepletionRadius = 1;
        private const int GoreSpawnChance = 650;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;

            AddMapEntry(new Color(70, 55, 45));
            DustType = DustID.Ash;
            HitSound = SoundID.Dig;

            RegisterItemDrop(ModContent.ItemType<ScorchedEarth>());
        }

        public override void RandomUpdate(int i, int j)
        {
            TrySpawnRareGore(i, j);
            RemovePlantAbove(i, j);

            for (int dx = -DepletionRadius; dx <= DepletionRadius; dx++)
            {
                for (int dy = -DepletionRadius; dy <= DepletionRadius; dy++)
                {
                    int tx = i + dx;
                    int ty = j + dy;

                    if (!WorldGen.InWorld(tx, ty, 1) || (tx == i && ty == j))
                    {
                        continue;
                    }

                    if (TryDepleteSoil(tx, ty))
                    {
                        RemovePlantAbove(tx, ty);
                    }
                }
            }
        }

        private static void TrySpawnRareGore(int i, int j)
        {
            if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(GoreSpawnChance))
            {
                return;
            }

            Vector2 spawnPos = new Vector2(i * 16f + 8f, j * 16f + 8f);
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-1.2f, -0.3f));

            Gore.NewGore(null, spawnPos, velocity, 99, Main.rand.NextFloat(0.8f, 1.1f));
        }

        private static bool TryDepleteSoil(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            if (!tile.HasTile)
            {
                return false;
            }

            int aniseGrass = ModContent.TileType<AniseGrassTile>();
            ushort currentType = tile.TileType;

            bool canDeplete =
                currentType == TileID.Grass ||
                currentType == TileID.CorruptGrass ||
                currentType == TileID.CrimsonGrass ||
                currentType == TileID.HallowedGrass ||
                currentType == TileID.JungleGrass ||
                currentType == TileID.MushroomGrass ||
                currentType == aniseGrass;

            if (!canDeplete)
            {
                return false;
            }

            tile.TileType = TileID.Dirt;
            WorldGen.SquareTileFrame(x, y, true);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, x, y, 3);
            }

            return true;
        }

        private static void RemovePlantAbove(int x, int y)
        {
            int aboveY = y - 1;
            if (!WorldGen.InWorld(x, aboveY, 1))
            {
                return;
            }

            Tile above = Framing.GetTileSafely(x, aboveY);
            if (!above.HasTile)
            {
                return;
            }

            if (!Main.tileCut[above.TileType])
            {
                return;
            }

            WorldGen.KillTile(x, aboveY, fail: false, effectOnly: false, noItem: true);
            WorldGen.SquareTileFrame(x, aboveY, true);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, x, aboveY, 3);
            }
        }
    }
}
