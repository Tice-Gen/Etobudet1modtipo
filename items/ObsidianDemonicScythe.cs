using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class ObsidianDemonicScythe : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 3);


            Item.useStyle = ItemUseStyleID.None;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {

            return false;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Dormant", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ObsidianDemonicScythe.Dormant")));
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Obsidian, 25);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}
