using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Projectiles;


namespace Etobudet1modtipo.items
{
    public class ProtectingAniseStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 0, 70, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<ProtectingAnise>();
            Item.buffType = ModContent.BuffType<ProtectingAniseBuff>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            position = Main.MouseWorld;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.Daybloom, 2)
                .AddIngredient(ItemID.StarAnise, 15)
                .Register();
        }

    }
}
