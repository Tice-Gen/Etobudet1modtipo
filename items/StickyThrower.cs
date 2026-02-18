using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.items
{
    public class StickyThrower : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 10;
            Item.height = 24;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(silver: 5);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.StickyKnife>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.None;
            Item.consumable = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "StickyThrowerDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.StickyThrower.StickyThrowerDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ExplosivePowder, 20)
                .AddIngredient(ItemID.Gel, 25)
                .AddIngredient(ModContent.ItemType<KnifeStorm>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}