using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class Kaluchka : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (type == TileID.Plants || type == TileID.CorruptThorns)
            {
                if (!fail && !noItem && Main.rand.NextFloat() < 0.025f)
                {
                    int amount = 1;
                    Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.Vilethorn, amount);
                }
            }
        }
    }
}