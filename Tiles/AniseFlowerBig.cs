using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class AniseFlowerBig : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.addTile(Type);

            DustType = DustID.Grass;
            HitSound = SoundID.Grass;

            AddMapEntry(new Color(90, 120, 90));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly || noItem)
                return;

            var src = new EntitySource_TileBreak(i, j);


            if (Main.rand.NextFloat() < 0.50f)
            {
                int count = Main.rand.Next(1, 6);
                Item.NewItem(src, i * 16, j * 16, 16, 16, ModContent.ItemType<AniseForestSeeds>(), count);
            }


            Item.NewItem(src, i * 16, j * 16, 16, 16, ItemID.StarAnise, Main.rand.Next(5, 16));
        }
    }
}
