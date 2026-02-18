using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Systems
{
    public class SkyIslandChestLootSystem : ModSystem
    {
        private const int SkywareChestStyle = 13;

        public override void PostWorldGen()
        {
            int itemType = ModContent.ItemType<HarpyRazorFeathers>();

            List<int> skyChestIndices = FindSkywareChests();
            if (skyChestIndices.Count == 0)
                return;

            Shuffle(skyChestIndices, WorldGen.genRand);

            int cursor = 0;


            bool placedAny = TryPlaceInNextChest(skyChestIndices, ref cursor, itemType, 1);
            if (!placedAny)
                return;


            if (WorldGen.genRand.NextFloat() < 0.20f)
                TryPlaceInNextChest(skyChestIndices, ref cursor, itemType, 1);


            if (WorldGen.genRand.NextFloat() < 0.10f)
                TryPlaceInNextChest(skyChestIndices, ref cursor, itemType, 1);
        }

        private static List<int> FindSkywareChests()
        {
            List<int> indices = new();

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);
                if (!tile.HasTile)
                    continue;

                if (tile.TileType != TileID.Containers)
                    continue;

                int style = tile.TileFrameX / 36;
                if (style == SkywareChestStyle)
                    indices.Add(i);
            }

            return indices;
        }

        private static bool TryPlaceInNextChest(List<int> chestIndices, ref int cursor, int itemType, int stack)
        {
            while (cursor < chestIndices.Count)
            {
                Chest chest = Main.chest[chestIndices[cursor++]];
                if (chest == null)
                    continue;


                if (ForcePlaceAsMainLoot(chest, itemType, stack))
                    return true;
            }

            return false;
        }






        private static bool ForcePlaceAsMainLoot(Chest chest, int itemType, int stack)
        {
            int mainSlot = 0;


            if (!chest.item[mainSlot].IsAir)
            {
                int free = FindFirstEmptySlot(chest);
                if (free != -1)
                {
                    chest.item[free] = chest.item[mainSlot].Clone();
                    chest.item[mainSlot].TurnToAir();
                }
                else
                {


                    chest.item[mainSlot].TurnToAir();
                }
            }

            chest.item[mainSlot].SetDefaults(itemType);
            chest.item[mainSlot].stack = stack;
            return true;
        }

        private static int FindFirstEmptySlot(Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].IsAir)
                    return i;
            }
            return -1;
        }

        private static void Shuffle(List<int> list, UnifiedRandom rand)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
