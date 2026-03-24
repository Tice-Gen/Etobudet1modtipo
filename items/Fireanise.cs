using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class Fireanise : ModItem
    {
        private const int SpawnDelayTicks = 24;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 3000;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<Cuborare>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Noisolpxe>();
            Item.shootSpeed = 0f;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return false;
            }

            int projectileIndex = Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                ModContent.ProjectileType<Noisolpxe>(),
                damage,
                knockback,
                player.whoAmI,
                SpawnDelayTicks,
                0f);

            if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
            {
                Main.projectile[projectileIndex].originalDamage = damage;
            }

            return false;
        }
    }
}
