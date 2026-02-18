using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class HeartOfAniseForest : ModItem
    {
        private int healTimer = 0;

        public override string Texture => "Etobudet1modtipo/items/HeartOfAniseForest";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.accessory = true;
            Item.noMelee = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "HeartOfAniseForest", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.HeartOfAniseForest.HeartOfAniseForest")));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            healTimer++;
            if (healTimer >= 30)
            {
                int healAmount = 2;
                player.statLife += healAmount;

                player.HealEffect(healAmount, true);
                healTimer = 0;
            }


            player.GetDamage<EndlessThrower>() += 0.05f;
        }
    }
}