using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class Salt : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.White;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ModContent.ItemType<SeaSalt>())
                .Register();
        }
    }
}
