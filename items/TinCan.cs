using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class TinCan : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(copper: 50);
            Item.rare = ItemRarityID.White;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.TinCan.Tooltip")));
        }

        public override void AddRecipes()
    {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.IronBar, 2);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.LeadBar, 2);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
