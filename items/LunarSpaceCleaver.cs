using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class LunarSpaceCleaver : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 115;
            Item.DamageType = DamageClass.Melee;

            Item.width = 80;
            Item.height = 90;

            Item.useTime = 55;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 6f;
            Item.autoReuse = true;

            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Cyan;

            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<LunarZigzag>();
            Item.shootSpeed = 1f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++) 
            {
                int dustIndex = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Vortex);
                
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
                dust.scale = Main.rand.NextFloat(1.3f, 1.8f);
            }
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
            if (player.whoAmI != Main.myPlayer)
                return false;

            Vector2 cursor = Main.MouseWorld;

            Vector2 spawnPos = new Vector2(
                cursor.X,
                cursor.Y - 600f
            );

            Vector2 dir = cursor - spawnPos;
            Vector2 vel = dir.SafeNormalize(Vector2.UnitY) * 24f;

            int projectileDamage = damage * 10;

            Projectile.NewProjectile(
                source,
                spawnPos,
                vel,
                ModContent.ProjectileType<LunarZigzag>(),
                projectileDamage,
                knockback,
                player.whoAmI
            );
            Lighting.AddLight(Item.Center, 0.18f, 0.36f, 0.65f);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SkySteel>())
                .AddIngredient(ItemID.FragmentVortex, 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}