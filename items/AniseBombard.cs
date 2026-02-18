using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class AniseBombard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 29;
            Item.crit = 21;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<BurstingStarAnise>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.None;
            Item.consumable = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseBombardDesc", Terraria.Localization.Language.GetTextValue("Throws explosive star anise that bursts into fragments.\nDoes not consume ammo.")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Grenade, 5)
                .AddIngredient(ItemID.StarAnise, 50)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 10)
                .AddIngredient(ModContent.ItemType<HighlyConcentratedAniseSoda>())
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
