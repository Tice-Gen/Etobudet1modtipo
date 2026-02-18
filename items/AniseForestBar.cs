using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class AniseForestBar : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ModContent.RarityType<AniseRarity>();


            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;


            Item.createTile = ModContent.TileType<AniseForestBarTile>();
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip1", Terraria.Localization.Language.GetTextValue("A bar infused with the power of the Anise Forest")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarAnise, 3)
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 5)
                .AddIngredient(ModContent.ItemType<AniseForestSeeds>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

