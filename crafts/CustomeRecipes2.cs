using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class CustomRecipes2 : ModSystem
    {
        public override void AddRecipes()
        {

            Recipe recipe = Recipe.Create(ItemID.Leather);
            recipe.AddIngredient(ItemID.Vertebrae, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}