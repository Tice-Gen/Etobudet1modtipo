using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class slimeGunRecipe : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe slimeGunRecipe = Recipe.Create(ItemID.SlimeGun);
            slimeGunRecipe.AddIngredient(ModContent.ItemType<items.EmptySlimeGun>());
            slimeGunRecipe.AddIngredient(ItemID.Gel, 100);
            slimeGunRecipe.AddTile(TileID.WorkBenches);
            slimeGunRecipe.Register();
        }
    }
}