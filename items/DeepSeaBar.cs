using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.items
{
    public class DeepSeaBar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ModContent.RarityType<DeepnestRare>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<DeepSeaBarTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeepSeaStone>(), 10)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
