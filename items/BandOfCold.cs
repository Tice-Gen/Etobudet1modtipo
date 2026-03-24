using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class BandOfCold : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
