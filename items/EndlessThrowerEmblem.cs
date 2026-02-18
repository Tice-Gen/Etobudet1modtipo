using Etobudet1modtipo.Classes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class EndlessThrowerEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetDamage<EndlessThrower>() += 0.15f;
        }
    }
}