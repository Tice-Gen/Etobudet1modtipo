using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class PoisonGun : ModItem
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
            Item.useAnimation = 6;
            Item.useTime = 6;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.PoisonProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<EmptySlimeGun>())
                .AddIngredient(ItemID.Stinger, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "PoisonGunDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.PoisonGun.PoisonGunDesc")));
        }


        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-4f, 0f);
        }
    }
}