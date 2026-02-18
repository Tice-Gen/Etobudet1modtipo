using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class StarBow : ModItem
    {
        private const int SpecialShotInterval = 10;
        private const int SpecialVolleyProjectiles = 10;
        private const float SpecialVolleyAngle = 60f;
        private int shotCounter;

        protected override bool CloneNewInstances => true;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 26;
            Item.height = 46;
            Item.useTime = 8;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ModContent.ProjectileType<Projectiles.StarArrow>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.None;
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
            shotCounter++;

            if (shotCounter >= SpecialShotInterval)
            {
                shotCounter = 0;

                float halfFanAngleRad = MathHelper.ToRadians(SpecialVolleyAngle * 0.5f);
                for (int i = 0; i < SpecialVolleyProjectiles; i++)
                {
                    float t = SpecialVolleyProjectiles > 1 ? (float)i / (SpecialVolleyProjectiles - 1) : 0.5f;
                    float angleOffset = MathHelper.Lerp(-halfFanAngleRad, halfFanAngleRad, t);
                    Vector2 perturbedSpeed = velocity.RotatedBy(angleOffset);
                    Projectile.NewProjectile(source, position, perturbedSpeed, Item.shoot, damage, knockback, player.whoAmI);
                }

                return false;
            }

            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.DemonBow);
            recipe1.AddIngredient(ItemID.FallenStar, 15);
            recipe1.AddIngredient(ItemID.GoldBar, 5);
            recipe1.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.TendonBow);
            recipe2.AddIngredient(ItemID.FallenStar, 15);
            recipe2.AddIngredient(ItemID.PlatinumBar, 5);
            recipe2.Register();

            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.TendonBow);
            recipe3.AddIngredient(ItemID.FallenStar, 15);
            recipe3.AddIngredient(ItemID.GoldBar, 5);
            recipe3.Register();

            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.DemonBow);
            recipe4.AddIngredient(ItemID.FallenStar, 15);
            recipe4.AddIngredient(ItemID.PlatinumBar, 5);
            recipe4.Register();
        }
    }
}
