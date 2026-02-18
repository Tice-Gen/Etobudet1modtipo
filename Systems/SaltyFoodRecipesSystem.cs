using Etobudet1modtipo.Common.GlobalItems;
using Etobudet1modtipo.items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Systems
{
    public class SaltyFoodRecipesSystem : ModSystem
    {
        private static readonly int[] TargetFoodTypes =
        {
            ItemID.SeafoodDinner,
            ItemID.CookedFish,
            ItemID.CookedShrimp,
            ItemID.GrilledSquirrel,
            ItemID.RoastedBird,
            ItemID.RoastedDuck,
            ItemID.BowlofSoup,
            ItemID.ShuckedOyster,
            ItemID.FriedEgg,
            ItemID.BBQRibs,
            ItemID.Steak,
            ItemID.Spaghetti
        };

        public override void AddRecipes()
        {
            int saltType = ModContent.ItemType<Salt>();

            for (int i = 0; i < TargetFoodTypes.Length; i++)
            {
                int foodType = TargetFoodTypes[i];
                Recipe recipe = Recipe.Create(foodType)
                    .AddIngredient(foodType)
                    .AddIngredient(saltType, 5);

                recipe.AddOnCraftCallback(static (_, item, _, destinationStack) =>
                {
                    if (System.Array.IndexOf(TargetFoodTypes, item.type) < 0)
                        return;

                    item.GetGlobalItem<SaltyFoodGlobalItem>().isSaltyFood = true;

                    if (destinationStack is not null && !destinationStack.IsAir && destinationStack.type == item.type)
                        destinationStack.GetGlobalItem<SaltyFoodGlobalItem>().isSaltyFood = true;
                });

                recipe.Register();
            }
        }
    }
}
