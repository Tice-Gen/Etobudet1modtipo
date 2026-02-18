using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class NecklaceOfEternalSatiety : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NecklaceOfEternalSatietyPlayer>().equipped = true;


            player.AddBuff(BuffID.WellFed, 60);
        }

        public override void AddRecipes()
        {
            AddRecipeVariant(ItemID.Burger, ItemID.GoldBar);
            AddRecipeVariant(ItemID.Burger, ItemID.PlatinumBar);
            AddRecipeVariant(ItemID.Bacon, ItemID.GoldBar);
            AddRecipeVariant(ItemID.Bacon, ItemID.PlatinumBar);
        }

        private void AddRecipeVariant(int burgerOrBacon, int barType)
        {
            CreateRecipe()
                .AddIngredient(ItemID.RoastedBird, 5)
                .AddIngredient(ItemID.RoastedDuck, 2)
                .AddIngredient(burgerOrBacon, 1)
                .AddIngredient(barType, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
