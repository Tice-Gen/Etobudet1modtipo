using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.items
{
    public class SteelOfLife : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;

            Item.maxStack = 9999;

            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;

            Item.consumable = true;


            Item.createTile = ModContent.TileType<SteelOfLifeTile>();
        }

        public override void AddRecipes()
        {

            CreateRecipe(5)
                .AddIngredient(ItemID.CobaltBar, 5)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.DemonAltar)
                .Register();


            CreateRecipe(5)
                .AddIngredient(ItemID.PalladiumBar, 5)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
