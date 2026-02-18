using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class StarAniseGrassDrop : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {

            if (type == TileID.Plants || type == TileID.Plants2)
            {
                if (!fail && !noItem && Main.rand.NextFloat() < 0.1f)
                {
                    int amount = Main.rand.Next(1, 4);
                    Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.StarAnise, amount);
                }
            }
        }
    }
}