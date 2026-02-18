using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.items
{
    public class SealedCoreOfLife : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
        }

        public override void AddRecipes()
        {

            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<HeartOfCalamity>())
                .AddIngredient(ModContent.ItemType<SteelOfLife>(), 15)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
