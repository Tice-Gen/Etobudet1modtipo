using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class Doubow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 40;
            Item.damage = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 5;
            Item.knockBack = 1.5f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item5;

            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 7f;

            Item.value = Item.buyPrice(silver: 40);
            Item.rare = ItemRarityID.White;
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback
        )
        {
            Vector2 velocityPos = velocity.RotatedBy(MathHelper.ToRadians(10f));
            Vector2 velocityNeg = velocity.RotatedBy(MathHelper.ToRadians(-10f));
            bool firstPositive = Main.rand.NextBool();
            Vector2 firstDirection = firstPositive ? velocityPos : velocityNeg;
            Vector2 secondDirection = firstPositive ? velocityNeg : velocityPos;

            if (!TryGetPriorityArrowSlot(player, -1, out int firstSlot))
            {
                return false;
            }

            if (TryGetPriorityArrowSlot(player, firstSlot, out int secondSlot))
            {
                int firstProjType = AmmoProjectileType(player.inventory[firstSlot]);
                int secondProjType = AmmoProjectileType(player.inventory[secondSlot]);
                ConsumeAmmoFromSlot(player, firstSlot, 1);
                ConsumeAmmoFromSlot(player, secondSlot, 1);

                Projectile.NewProjectile(source, position, firstDirection, firstProjType, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, secondDirection, secondProjType, damage, knockback, player.whoAmI);
                return false;
            }

            Item firstPriorityAmmo = player.inventory[firstSlot];
            int firstPriorityProjType = AmmoProjectileType(firstPriorityAmmo);
            int firstPriorityCount = firstPriorityAmmo.stack;

            if (firstPriorityCount <= 1)
            {
                ConsumeAmmoFromSlot(player, firstSlot, 1);
                Projectile.NewProjectile(source, position, firstDirection, firstPriorityProjType, damage, knockback, player.whoAmI);
                return false;
            }

            ConsumeAmmoFromSlot(player, firstSlot, 2);
            Projectile.NewProjectile(source, position, firstDirection, firstPriorityProjType, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, secondDirection, firstPriorityProjType, damage, knockback, player.whoAmI);

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenBow)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddIngredient(ItemID.IronBar, 1)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.WoodenBow)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddIngredient(ItemID.LeadBar, 1)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }

        private static int AmmoProjectileType(Item ammoItem)
        {
            return ammoItem.shoot > ProjectileID.None ? ammoItem.shoot : ProjectileID.WoodenArrowFriendly;
        }

        private static bool TryGetPriorityArrowSlot(Player player, int excludedSlot, out int slotIndex)
        {
            for (int i = 54; i <= 57; i++)
            {
                if (i == excludedSlot || i >= player.inventory.Length)
                {
                    continue;
                }

                Item item = player.inventory[i];
                if (!item.IsAir && item.ammo == AmmoID.Arrow && item.stack > 0)
                {
                    slotIndex = i;
                    return true;
                }
            }

            for (int i = 0; i <= 53 && i < player.inventory.Length; i++)
            {
                if (i == excludedSlot)
                {
                    continue;
                }

                Item item = player.inventory[i];
                if (!item.IsAir && item.ammo == AmmoID.Arrow && item.stack > 0)
                {
                    slotIndex = i;
                    return true;
                }
            }

            slotIndex = -1;
            return false;
        }

        private static void ConsumeAmmoFromSlot(Player player, int slotIndex, int amount)
        {
            if (slotIndex < 0 || slotIndex >= player.inventory.Length || amount <= 0)
            {
                return;
            }

            Item item = player.inventory[slotIndex];
            if (item.IsAir || item.stack <= 0)
            {
                return;
            }

            if (!item.consumable)
            {
                return;
            }

            item.stack -= amount;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }
        }
    }
}
