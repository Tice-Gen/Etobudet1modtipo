using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class DeepSeaBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            DustType = DustID.Ice;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            RegisterItemDrop(ModContent.ItemType<DeepSeaBar>());
            AddMapEntry(new Color(45, 132, 164));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (!WorldGen.SolidTileAllowBottomSlope(i, j + 1))
            {
                WorldGen.KillTile(i, j);
            }

            return true;
        }
    }
}
