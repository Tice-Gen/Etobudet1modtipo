using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class CustomRecipes : ModSystem
    {
        public override void AddRecipes()
        {

            Recipe recipe = Recipe.Create(ItemID.FrostDaggerfish);
            recipe.AddIngredient(ItemID.IceBlock, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}