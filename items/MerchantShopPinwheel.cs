using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class MerchantShopPinwheel : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
            {
                shop.Add(ModContent.ItemType<items.PinwheelShuriken>());
            }
        }
    }
}
