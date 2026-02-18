using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class FogStone_Gem : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;

            Main.tileShine[Type] = 1100;
            Main.tileShine2[Type] = true;


            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);


            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleWrapLimit = 12;
            TileObjectData.newTile.CoordinateWidth = 18;
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.newTile.CoordinatePadding = 0;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(150, 150, 200));

            DustType = DustID.GemDiamond;
            MineResist = 1f;

            RegisterItemDrop(ModContent.ItemType<FogStone>());
        }
    }
}
