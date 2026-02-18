using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.items;


namespace Etobudet1modtipo.items
{
    public class ImprovedStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 70;
            Item.knockBack = 4f;
            Item.mana = 15;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(0, 10, 0, 0);

            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<ImprovedStaffProjectile>();
            Item.shootSpeed = 12f;
            Item.noMelee = true;

            Item.noUseGraphic = false;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }


        public override void HoldItem(Player player)
        {

            player.itemRotation = MathHelper.ToRadians(-15f);


            player.itemLocation += new Vector2(2, -2);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DiamondStaff)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.RubyStaff)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.AmberStaff)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}