using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class WeekPlasmaSword : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 54;
            Item.useTime = 20;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;


            Item.shoot = ProjectileID.FairyQueenMagicItemShot;
            Item.shootSpeed = 8f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.FieryGreatsword)
                .AddIngredient(ItemID.MeteoriteBar, 20)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 45)
                .AddIngredient(ItemID.HellstoneBar, 25)
                .AddCondition(Condition.NearShimmer)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.WeekPlasmaSword.Description"))
            {
                OverrideColor = Color.MediumPurple
            });
        }
    }
}