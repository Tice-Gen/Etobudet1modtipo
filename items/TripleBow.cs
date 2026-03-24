using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class TripleBow : ModItem
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

            Item.value = Item.buyPrice(silver: 60);
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
            if (!TryGetPriorityArrowSlot(player, -1, -1, -1, out int firstSlot))
            {
                return false;
            }

            Vector2 sidePos = velocity.RotatedBy(MathHelper.ToRadians(10f));
            Vector2 sideNeg = velocity.RotatedBy(MathHelper.ToRadians(-10f));
            bool firstSidePositive = Main.rand.NextBool();
            Vector2 sideA = firstSidePositive ? sidePos : sideNeg;
            Vector2 sideB = firstSidePositive ? sideNeg : sidePos;

            Item firstAmmo = player.inventory[firstSlot];
            int firstProj = AmmoProjectileType(firstAmmo);
            int firstCountBefore = firstAmmo.stack;

            ConsumeAmmoFromSlot(player, firstSlot, 1);
            Projectile.NewProjectile(source, position, velocity, firstProj, damage, knockback, player.whoAmI);

            bool hasSecond = TryGetPriorityArrowSlot(player, firstSlot, -1, -1, out int secondSlot);
            if (!hasSecond)
            {
                if (firstCountBefore >= 2)
                {
                    ConsumeAmmoFromSlot(player, firstSlot, 1);
                    Projectile.NewProjectile(source, position, sideA, firstProj, damage, knockback, player.whoAmI);
                }

                if (firstCountBefore >= 3)
                {
                    ConsumeAmmoFromSlot(player, firstSlot, 1);
                    Projectile.NewProjectile(source, position, sideB, firstProj, damage, knockback, player.whoAmI);
                }

                return false;
            }

            Item secondAmmo = player.inventory[secondSlot];
            int secondProj = AmmoProjectileType(secondAmmo);
            int secondCountBefore = secondAmmo.stack;

            bool hasThird = TryGetPriorityArrowSlot(player, firstSlot, secondSlot, -1, out int thirdSlot);
            if (hasThird)
            {
                Item thirdAmmo = player.inventory[thirdSlot];
                int thirdProj = AmmoProjectileType(thirdAmmo);

                bool secondGoesFirst = Main.rand.NextBool();
                int projA = secondGoesFirst ? secondProj : thirdProj;
                int projB = secondGoesFirst ? thirdProj : secondProj;

                ConsumeAmmoFromSlot(player, secondSlot, 1);
                ConsumeAmmoFromSlot(player, thirdSlot, 1);
                Projectile.NewProjectile(source, position, sideA, projA, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, sideB, projB, damage, knockback, player.whoAmI);
                return false;
            }

            ConsumeAmmoFromSlot(player, secondSlot, 1);
            Projectile.NewProjectile(source, position, sideA, secondProj, damage, knockback, player.whoAmI);

            if (secondCountBefore >= 2)
            {
                ConsumeAmmoFromSlot(player, secondSlot, 1);
                Projectile.NewProjectile(source, position, sideB, secondProj, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Doubow>())
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient(ItemID.IronBar, 2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Doubow>())
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient(ItemID.LeadBar, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        private static int AmmoProjectileType(Item ammoItem)
        {
            return ammoItem.shoot > ProjectileID.None ? ammoItem.shoot : ProjectileID.WoodenArrowFriendly;
        }

        private static bool TryGetPriorityArrowSlot(Player player, int excludedA, int excludedB, int excludedC, out int slotIndex)
        {
            for (int i = 54; i <= 57; i++)
            {
                if (i >= player.inventory.Length || i == excludedA || i == excludedB || i == excludedC)
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
                if (i == excludedA || i == excludedB || i == excludedC)
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
            if (item.IsAir || item.stack <= 0 || !item.consumable)
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
