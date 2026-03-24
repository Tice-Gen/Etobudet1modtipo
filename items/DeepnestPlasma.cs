using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class DeepnestPlasma : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 50;
            Item.damage = 0;
            Item.useTime = 60;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 0);
            Item.rare = ModContent.RarityType<SupremeRare>();
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<BlueExpl>();
            Item.shootSpeed = 0f;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI
            );

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name == "Damage");

            TooltipLine customDamageLine = new TooltipLine(Mod, "DeepnestPlasmaDamage", "100% any damage");
            int nameIndex = tooltips.FindIndex(line => line.Name == "ItemName");

            if (nameIndex != -1)
            {
                tooltips.Insert(nameIndex + 1, customDamageLine);
            }
            else
            {
                tooltips.Insert(0, customDamageLine);
            }
        }
    }
}
