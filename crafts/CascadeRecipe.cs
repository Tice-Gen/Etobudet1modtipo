using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class CascadeRecipe : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.Cascade);
            recipe.AddIngredient(ItemID.HellstoneBar, 13);
            recipe.AddIngredient(ItemID.Obsidian, 5);
            recipe.AddIngredient(ItemID.Cobweb, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
