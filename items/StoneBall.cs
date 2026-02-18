using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using System.Collections.Generic;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.items
{
    public class StoneBall : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.crit = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(copper: 10);
            Item.rare = ItemRarityID.White;
            Item.consumable = true;

            Item.shoot = ModContent.ProjectileType<StoneBallProj>();
            Item.shootSpeed = 0f;


            Item.ammo = CustomAmmoID.Stone;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "StoneBallDescription", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.StoneBall.StoneBallDescription")));
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
}
