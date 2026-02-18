using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.items
{
    public class SaltWater : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.value = Item.buyPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;

            Item.buffType = ModContent.BuffType<Dehydration>();
            Item.buffTime = 900;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ModContent.ItemType<SeaSalt>(), 2)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
