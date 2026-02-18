using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class WeekZenith : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            
            Item.shoot = ModContent.ProjectileType<WeekZenithProj>();
            Item.shootSpeed = 15f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }

        public override void AddRecipes()
    {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.WoodenSword);
            recipe1.AddIngredient(ItemID.RichMahoganySword);
            recipe1.AddIngredient(ItemID.EbonwoodSword);
            recipe1.AddIngredient(ItemID.BorealWoodSword);
            recipe1.AddIngredient(ItemID.PalmWoodSword);
            recipe1.AddIngredient(ItemID.CactusSword);      
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.WoodenSword);
            recipe2.AddIngredient(ItemID.RichMahoganySword);
            recipe2.AddIngredient(ItemID.ShadewoodSword);
            recipe2.AddIngredient(ItemID.BorealWoodSword);
            recipe2.AddIngredient(ItemID.PalmWoodSword);
            recipe2.AddIngredient(ItemID.CactusSword);      
            recipe2.Register();
        }
    }
}