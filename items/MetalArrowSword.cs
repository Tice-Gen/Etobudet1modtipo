using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class MetalArrowSword : ModItem
    {


        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;


            Item.shoot = ModContent.ProjectileType<IronArrowProj>();
            Item.shootSpeed = 12f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }


        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            ArrowSwordPlayer mp = player.GetModPlayer<ArrowSwordPlayer>();

            mp.pending = true;
            mp.queuedOwner = player.whoAmI;
            mp.queuedItemType = Item.type;
            mp.queuedType = type;
            mp.queuedDamage = damage;
            mp.queuedKnockback = knockback;
            mp.queuedBaseSpeed = velocity.Length();


            mp.queuedMouseWorld = Main.MouseWorld;

            return false;
        }
    }
}
