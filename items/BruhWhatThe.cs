using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.items
{
    public class BruhWhatThe : ModItem
    {
        private int healTimer = 0;

        public override string Texture => "Terraria/Images/Mana";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.accessory = true;
            Item.noMelee = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "HeartOfAniseForest", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.BruhWhatThe.HeartOfAniseForest")));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            healTimer++;
            if (healTimer >= 1)
            {
                int healAmount = 676767;
                player.statLife += healAmount;

                player.HealEffect(healAmount, true);
                healTimer = 0;
            }
            player.GetDamage<EndlessThrower>() += 10000f;
        }
        
    }
}