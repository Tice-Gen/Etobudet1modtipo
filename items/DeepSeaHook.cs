using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.items
{
    public class DeepSeaHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(silver: 15);
            Item.rare = ItemRarityID.Red;

            Item.shoot = ModContent.ProjectileType<DeepGarp>();
            Item.shootSpeed = 0f;
            Item.ammo = CustomAmmoID.DeepSeaHook;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ModContent.ItemType<DeepSeaStone>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
