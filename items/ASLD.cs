using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class ASLD : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 20;
            Item.damage = 1;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.None;
            Item.shoot = ModContent.ProjectileType<Projectiles.FlareMarker>();
            Item.shootSpeed = 1f;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6f, 2f);
        }

        public override bool Shoot(
            Player player,
            Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {

            Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                Item.shoot,
                0,
                knockback,
                player.whoAmI
            );

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {

                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Damage")
                {
                    tooltips[i].Text = Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ASLD.Damage");
                }
            }

            tooltips.Add(new TooltipLine(Mod, "ASLDDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ASLD.ASLDDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlareGun)
                .AddIngredient(ItemID.HellstoneBar, 35)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
