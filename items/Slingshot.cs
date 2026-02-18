using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.items
{
    public class Slingshot : ModItem
    {
        public override string Texture => "Etobudet1modtipo/items/Slingshot";

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<StoneBallProj>();
            Item.shootSpeed = 11f;


            Item.useAmmo = CustomAmmoID.Stone;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
                .AddIngredient(ItemID.Gel, 5)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "SlingshotDescription", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.Slingshot.SlingshotDescription")));
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 3f);
        }
    }
}
