using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class PumpkinPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 4;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 99;
            Item.useAnimation = 99;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.pick = 65;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pumpkin, 15)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
