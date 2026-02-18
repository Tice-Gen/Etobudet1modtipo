using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class ParticleOfCalamity : ModItem
    {


        public override void SetStaticDefaults()
        {

            Item.ResearchUnlockCount = 10000;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Purple;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            foreach (var line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    line.OverrideColor = new Microsoft.Xna.Framework.Color(120, 0, 150);
                }
            }


            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ParticleOfCalamity.Description")));
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe(5);
            recipe1.AddIngredient(ItemID.ShadowScale, 5);
            recipe1.AddIngredient(ItemID.DemoniteBar, 5);
            recipe1.AddIngredient(ItemID.DemoniteOre, 15);
            recipe1.Register();

            Recipe recipe2 = CreateRecipe(5);
            recipe2.AddIngredient(ItemID.TissueSample, 5);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe2.AddIngredient(ItemID.CrimtaneOre, 15);
            recipe2.Register();
        }
    }
}