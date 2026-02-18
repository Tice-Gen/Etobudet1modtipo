using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class HeroSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 77;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<HeroProj>();
            Item.shootSpeed = 8f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BrokenHeroSword)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
}
