using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Legs)]
    public class DeepSeaLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 30;
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.value = Item.buyPrice(gold: 35);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.10f;
            player.maxMinions += 3;
            player.moveSpeed += 0.20f;
            player.waterWalk = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeepSeaBar>(), 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
