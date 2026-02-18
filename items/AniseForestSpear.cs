using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.items
{
    public class AniseForestSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 94;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<AniseForestSpearProj>();
            Item.shootSpeed = 12f;

            Item.consumable = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestSpearDescription", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AniseForestSpear.AniseForestSpearDescription")));
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, 
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AniseForestSpearProj>(), damage, knockback, player.whoAmI);


            float angle = MathHelper.ToRadians(15);
            Vector2 velocityUp = velocity.RotatedBy(-angle);
            Vector2 velocityDown = velocity.RotatedBy(angle);


            int leafDamage = damage / 3;

            Projectile.NewProjectile(source, position, velocityUp, ProjectileID.Leaf, leafDamage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocityDown, ProjectileID.Leaf, leafDamage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteOre, 35)
                .AddIngredient(ItemID.ChlorophyteBar, 15)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 10)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
