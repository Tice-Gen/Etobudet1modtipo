using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class AniseInfusion : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(silver: 60);
            Item.UseSound = SoundID.Item3;
        }

        public override bool CanUseItem(Player player)
        {

            return !player.HasBuff(BuffID.PotionSickness);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.Bottle, 2)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 5)
                .AddIngredient(ItemID.StarAnise, 15)
                .Register();
        }

        public override bool? UseItem(Player player)
        {

            player.AddBuff(ModContent.BuffType<HealingBuff>(), 10 * 60);


            player.AddBuff(BuffID.PotionSickness, 45 * 60);

            return true;
        }
    }
}
