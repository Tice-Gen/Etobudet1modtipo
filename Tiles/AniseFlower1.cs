using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace Etobudet1modtipo.Tiles
{
    public class AniseFlower1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(100, 140, 100));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int amount = Main.rand.Next(5, 16);
            var src = new EntitySource_TileBreak(i, j);
            Item.NewItem(src, i * 16, j * 16, 16, 32, ItemID.StarAnise, amount);
        }
    }
}
