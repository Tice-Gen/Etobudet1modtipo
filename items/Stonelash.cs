using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class Stonelash : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<Rock>();
            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int rockCount = 1;
            const float rockSpeed = 5f;
            const int rockDamage = 10;

            for (int i = 0; i < rockCount; i++)
            {
                Vector2 dir = Main.rand.NextVector2CircularEdge(1f, 1f);
                Vector2 rockVelocity = dir * rockSpeed;
                Projectile.NewProjectile(source, position, rockVelocity, ModContent.ProjectileType<Rock>(), rockDamage, knockback, player.whoAmI);

                Dust.NewDust(position, 2, 2, DustID.Stone, rockVelocity.X * 0.2f, rockVelocity.Y * 0.2f);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
}
