using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NewBagDrops
{
    public class PlanteraBagGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {

            if (item.type == ItemID.PlanteraBossBag)
            {

                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloweringChlorophyteBar>(), 1, 50, 60));




            }
        }
    }
}
