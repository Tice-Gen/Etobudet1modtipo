using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Projectiles;
namespace Etobudet1modtipo.items
{
    public class ReverseShotgun : ModItem
    {

        public override string Texture => "Etobudet1modtipo/items/ReverseShotgun";

        public override void SetDefaults()
        {
            Item.damage = 71;
            Item.crit = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<ReverseBullet>();
            Item.shootSpeed = 25f;

            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 7;
            float distance = 1100f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 direction = velocity.SafeNormalize(Vector2.UnitX);
                Vector2 spawnPos = player.Center + direction * distance;
                Vector2 toPlayer = (player.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                toPlayer = toPlayer.RotatedByRandom(MathHelper.ToRadians(10));
                float speed = 25f;
                Vector2 velocityToPlayer = toPlayer * speed;

                Projectile.NewProjectile(
                    source,
                    spawnPos,
                    velocityToPlayer,
                    ModContent.ProjectileType<ReverseBullet>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-4f, 0f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Shotgun, 1);
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ItemID.BlackLens, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}