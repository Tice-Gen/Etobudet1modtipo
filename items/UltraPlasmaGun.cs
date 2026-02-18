using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class UltraPlasmaGun : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 1900;
            Item.crit = 35;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 999);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<UltraPlasmaGunHold>();
            Item.shootSpeed = 15f;
            Item.mana = 300;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, 
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, player.Center, velocity, type, 
                damage, knockback, player.whoAmI);
            return false;
        }


    }
}