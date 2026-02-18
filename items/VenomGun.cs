using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class VenomGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 28;
            Item.damage = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 250);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.VenomProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PoisonGun>())
                .AddIngredient(ItemID.VialofVenom, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "VenomGunDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.VenomGun.VenomGunDesc")));
        }


        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-4f, 0f);
        }
    }
}