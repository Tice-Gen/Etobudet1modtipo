using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class DeepSeaPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.value = Item.buyPrice(platinum: 5);
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.pick = 1000;
            Item.useTurn = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeepSeaStone>(), 900)
                .Register();
        }
    }
}
