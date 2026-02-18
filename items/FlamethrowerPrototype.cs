using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class FlamethrowerPrototype : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 28;
            Item.damage = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 11;
            Item.useTime = 12;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 250);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.FlamethrowerPrototypeFlame>();
            Item.shootSpeed = 15f;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SlimeGun)
                .AddIngredient(ItemID.Torch)
                .AddIngredient(ItemID.IronBar, 10)
                .AddTile(TileID.Anvils)
                .Register();

                CreateRecipe()
                .AddIngredient(ItemID.SlimeGun)
                .AddIngredient(ItemID.Torch)
                .AddIngredient(ItemID.LeadBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "VenomGunDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FlamethrowerPrototype.VenomGunDesc")));
        }


        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-4f, 0f);
        }
    }
}