using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.items
{
    public class AniseForestSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.value = 0;
            Item.rare = ItemRarityID.White;


            Item.createTile = -1;
        }

        public override bool? UseItem(Player player)
        {

            int tx = (int)(Main.MouseWorld.X / 16f);
            int ty = (int)(Main.MouseWorld.Y / 16f);



            Tile target = Framing.GetTileSafely(tx, ty);


            if (!target.HasTile)
            {
                target = Framing.GetTileSafely(tx, ty + 1);
                ty = ty + 1;
            }


            ushort[] allowed = new ushort[] { TileID.Dirt, TileID.Grass };

            bool canConvert = false;
            foreach (ushort t in allowed)
            {
                if (target.HasTile && target.TileType == t)
                {
                    canConvert = true;
                    break;
                }
            }

            if (!canConvert)
                return false;


            int newType = ModContent.TileType<AniseGrassTile>();
            target.TileType = (ushort)newType;
            target.HasTile = true;


            WorldGen.SquareTileFrame(tx, ty, true);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, tx, ty, 1);
            }


            return true;
        }
    }
}
