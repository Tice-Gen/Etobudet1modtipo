using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class FogStoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(120, 120, 140));
            DustType = DustID.Stone;
            MineResist = 1.5f;
            MinPick = 0;


            RegisterItemDrop(ModContent.ItemType<FogStone>());
        }
    }
}