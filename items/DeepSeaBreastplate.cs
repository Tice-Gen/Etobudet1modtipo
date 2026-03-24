using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Body)]
    public class DeepSeaBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 60;
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.value = Item.buyPrice(gold: 45);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetDamage(DamageClass.Ranged) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeepSeaBar>(), 40)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
