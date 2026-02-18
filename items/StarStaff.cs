using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class StarStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.width = 38;
            Item.height = 37;
            Item.useTime = 4;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(gold: 4);
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.mana = 13;
            Item.noMelee = true;
            
            Item.reuseDelay = 22;

            Item.shoot = ProjectileID.BeeCloakStar;
            Item.shootSpeed = 50f;
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


            int projType = ProjectileID.BeeCloakStar;


            Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawnPos, projVelocity, projType, damage, knockback, player.whoAmI);


            return false;
        }



       public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 30)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 10)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
        
    }
    
}
