using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class IronArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(copper: 50);
            Item.rare = ItemRarityID.Blue;

            Item.shoot = ModContent.ProjectileType<Projectiles.IronArrowProj>();
            Item.shootSpeed = 6.5f;

            Item.consumable = true;
            Item.ammo = AmmoID.Arrow;
            Item.maxStack = 9999;

            Item.width = 14;
            Item.height = 32;

            Item.crit = 4;
        }

        public override void AddRecipes()
        {

            Recipe recipe1 = CreateRecipe(25);
            recipe1.AddIngredient(ItemID.WoodenArrow, 25);
            recipe1.AddIngredient(ItemID.IronBar, 5);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe(25);
            recipe2.AddIngredient(ItemID.WoodenArrow, 25);
            recipe2.AddIngredient(ItemID.LeadBar, 5);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}