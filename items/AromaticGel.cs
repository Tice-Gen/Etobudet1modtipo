using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class AromaticGel : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(copper: 20);
            Item.rare = ItemRarityID.White;


            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AromaticGel.Tooltip")));
        }
    }
}