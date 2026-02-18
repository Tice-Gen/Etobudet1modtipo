using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class AniseForestDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Melee;
            Item.width = 14;
            Item.height = 14;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(silver: 50);
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.autoReuse = true;

            Item.shoot = ProjectileID.Leaf;
            Item.shootSpeed = 10f;
        }


        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Vector2 mouseWorld = Main.MouseWorld;


            float spawnDistance = 150f;


            float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);


            Vector2 spawnPos = mouseWorld + new Vector2(spawnDistance, 0f).RotatedBy(angle);


            Vector2 toCursor = mouseWorld - spawnPos;

            if (toCursor == Vector2.Zero)
                toCursor = new Vector2(0.01f, 0f);

            Vector2 projVelocity = Vector2.Normalize(toCursor) * Item.shootSpeed;


            int projType = ProjectileID.Leaf;


            Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawnPos, projVelocity, projType, damage, knockback, player.whoAmI);


            return false;
        }



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 10)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 5)
                .Register();
        }
    }
    
}
