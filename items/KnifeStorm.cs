using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.items
{
    public class KnifeStorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ProjectileID.ThrowingKnife;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.None;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
            int type, int damage, float knockback)
        {
            int numberProjectiles = 6 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                float scale = 1f - Main.rand.NextFloat() * 0.2f;
                perturbedSpeed *= scale;

                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, 0f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "KnifeStormDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.KnifeStorm.KnifeStormDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 15)
                .AddIngredient(ItemID.ThrowingKnife, 500)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}