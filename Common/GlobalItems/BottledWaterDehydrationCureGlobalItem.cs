using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class BottledWaterDehydrationCureGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type != ItemID.BottledWater)
                return;


            item.useStyle = ItemUseStyleID.DrinkLiquid;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (!IsDehydrationCureDrink(item))
                return;

            player.ClearBuff(ModContent.BuffType<Dehydration>());
        }

        private static bool IsDehydrationCureDrink(Item item)
        {
            if (item.type == ItemID.BottledWater ||
                item.type == ModContent.ItemType<AniseSoda>())
            {
                return true;
            }


            string name = (item.Name ?? string.Empty).ToLowerInvariant();
            return name.Contains("хоха-кола") ||
                   name.Contains("хоха кола") ||
                   name.Contains("крем сода") ||
                   name.Contains("крем-сода");
        }
    }
}
