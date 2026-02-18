using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;
namespace Etobudet1modtipo
{
    public class CustomRecipes3 : ModSystem
    {
        public override void AddRecipes()
        {

            Recipe recipe = Recipe.Create(ItemID.AvengerEmblem);
            recipe.AddIngredient(ModContent.ItemType<EndlessThrowerEmblem>());
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}