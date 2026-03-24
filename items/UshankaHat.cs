using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class UshankaHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SnowHat)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
