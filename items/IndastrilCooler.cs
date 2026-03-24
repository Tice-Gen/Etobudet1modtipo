using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class IndastrilCooler : ModItem
    {
        public override string Texture => "Etobudet1modtipo/Tiles/IndustrialCooler";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 80);
            Item.UseSound = SoundID.Dig;
            Item.createTile = ModContent.TileType<Tiles.IndastrilCoolerTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 20)
                .AddIngredient(ItemID.Wire, 8)
                .AddIngredient(ItemID.IronBar, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
