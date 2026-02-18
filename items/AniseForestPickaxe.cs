using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class AniseForestPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 4;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.pick = 35;
            Item.useTurn = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 5)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 5)
                .Register();
        }
    }
}
