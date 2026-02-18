using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class NewZenithRecept : ModSystem
    {
        public override void AddRecipes()
        {

            Recipe recipe = Recipe.Create(ItemID.Zenith);
            recipe.AddIngredient(ItemID.CopperShortsword);
            recipe.AddIngredient(ItemID.IceBlade);
            recipe.AddIngredient(ItemID.BeeKeeper);
            recipe.AddIngredient(ItemID.Seedler);
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.InfluxWaver);
            recipe.AddIngredient(ItemID.StarWrath);
            recipe.AddIngredient(ItemID.Meowmere);
            recipe.AddIngredient(ItemID.TerraBlade);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}