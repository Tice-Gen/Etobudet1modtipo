using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class AngryAnise : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip", Terraria.Localization.Language.GetTextValue("Not consumable")));
        }
        public override void AddRecipes()
    {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.EbonstoneBlock, 10);
            recipe1.AddIngredient(ItemID.StarAnise);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimstoneBlock, 10);
            recipe2.AddIngredient(ItemID.StarAnise);
            recipe2.Register();
        }

        
}
}

