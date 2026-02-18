using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class TrueTerraBlade : ModItem
    {
        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.knockBack = 4.5f;
            Item.width = 40;
            Item.height = 40;
			Item.shootSpeed = 12f;
            Item.scale = 1f;
            Item.UseSound = SoundID.Item50;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(gold: 50);
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<TrueTerraBladeSwordProjectile>();
            Item.noMelee = true; 
            Item.shootsEveryUse = true; 
            Item.autoReuse = true;
            Item.damage = 120;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {



            float adjustedItemScale = player.GetAdjustedItemScale(Item); 
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); 




            

            int снаряд = ProjectileID.RainbowRodBullet; 


            float angle = MathHelper.ToRadians(5f); 


            Projectile.NewProjectile(source, position, velocity, снаряд, damage, knockback, player.whoAmI);



            Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), снаряд, damage, knockback, player.whoAmI);


            Projectile.NewProjectile(source, position, velocity.RotatedBy(-angle), снаряд, damage, knockback, player.whoAmI);


            return false; 
        }

        public override void AddRecipes() {
            CreateRecipe()
            .AddIngredient(ItemID.TerraBlade)
            .AddIngredient(ModContent.ItemType<HeroSword>())
            .AddIngredient(ModContent.ItemType<ProphecyBlade>())
            .AddIngredient(ModContent.ItemType<LunarSpaceCleaver>())
            .AddIngredient(ItemID.Meowmere)
            .Register();
        }
    }
}