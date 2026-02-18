using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using System;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class AniseGrassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = 1;

            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(40, 90, 55));

            RegisterItemDrop(ItemID.DirtBlock, ItemID.MudBlock);

            var saplingData = TileObjectData.GetTileData(TileID.Saplings, 0);
            if (saplingData != null && saplingData.AnchorValidTiles != null && Array.IndexOf(saplingData.AnchorValidTiles, Type) < 0)
            {
                var anchors = saplingData.AnchorValidTiles;
                Array.Resize(ref anchors, anchors.Length + 1);
                anchors[anchors.Length - 1] = Type;
                saplingData.AnchorValidTiles = anchors;
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            int x = i;
            int y = j - 1;

            if (y <= 0 || y >= Main.maxTilesY - 1)
                return;
            if (Main.tile[x, y].HasTile)
                return;

            var rnd = WorldGen.genRand;


            if (rnd.NextBool(200))
            {
                bool clearForTree = true;
                for (int yy = y; yy > y - 10; yy--)
                {
                    if (yy <= 0 || yy >= Main.maxTilesY - 1)
                    {
                        clearForTree = false;
                        break;
                    }
                    if (Main.tile[x, yy].HasTile)
                    {
                        clearForTree = false;
                        break;
                    }
                }

                if (clearForTree)
                {
                    bool placedSapling = WorldGen.PlaceObject(x, y, TileID.Saplings, mute: true);
                    if (placedSapling)
                    {
                        WorldGen.SquareTileFrame(x, y, true);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, x, y, 3);
                    }
                    return;
                }
            }


            if (rnd.NextBool(50))
            {
                int plantType = rnd.NextBool(2) ? TileID.Plants : TileID.Plants2;
                bool placedPlant = WorldGen.PlaceTile(x, y, plantType, mute: true, forced: true);
                if (placedPlant)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 3);
                }
                return;
            }


            if (!rnd.NextBool(10))
                return;

            int tFlower1 = ModContent.TileType<AniseFlower1>();
            int tFlower2 = ModContent.TileType<AniseFlower2>();
            int tFlowerBig = ModContent.TileType<AniseFlowerBig>();

            int choice = rnd.Next(3);
            int flowerTile = (choice == 0) ? tFlower1 : (choice == 1) ? tFlower2 : tFlowerBig;

            bool isBig = (flowerTile == tFlowerBig);
            int flowerHeight = isBig ? 1 : 2;

            for (int yy = y; yy > y - flowerHeight; yy--)
            {
                if (yy <= 0 || yy >= Main.maxTilesY - 1)
                    return;
                if (Main.tile[x, yy].HasTile)
                    return;
            }

            bool placed = false;
            if (isBig)
            {
                placed = WorldGen.PlaceTile(x, y, flowerTile, mute: true, forced: true);
            }
            else
            {
                int topY = y - (flowerHeight - 1);
                placed = WorldGen.PlaceObject(x, topY, flowerTile, mute: true);
            }

            if (!placed)
                return;

            WorldGen.SquareTileFrame(x, y, true);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, x, y, 3);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly || noItem)
                return;

            if (Main.rand.NextBool(5))
            {
                var src = new EntitySource_TileBreak(i, j);
                Item.NewItem(src, i * 16, j * 16, 16, 16, ModContent.ItemType<AniseForestSeeds>());
            }
        }
    }
}
