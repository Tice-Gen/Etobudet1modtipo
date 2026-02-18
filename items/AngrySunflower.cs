using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.items
{
    public class AngrySunflower : ModItem
    {
        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityMaterials[Type] = 58;
        }

        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            

            Item.consumable = true;
            

            Item.createTile = ModContent.TileType<AngrySunflowerTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AngryAnise>())
                .AddIngredient(ItemID.Sunflower)
                .AddIngredient(ItemID.GrassSeeds, 5)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}