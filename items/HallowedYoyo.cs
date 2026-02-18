using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class HallowedYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 97;
            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 9);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<HallowedYoyoProj>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("The yoyo creates a strong aura that grants defense while hovering over the player, but becomes weaker.\nAt low health, it heals you for each enemy it hits.\nHits from the yoyo itself speed up the aura attack rate.\nPierces all armor.")));
            int damageIndex = tooltips.FindIndex(t => t.Name == "Damage");
            TooltipLine auraLine = new TooltipLine(Mod, "AuraYoyoTag", "-Aura Yoyo-");
            float wave = (float)((System.Math.Sin(Main.GlobalTimeWrappedHourly * 3.2f) + 1.0) * 0.5);
            Color brightGold = new Color(255, 220, 70);
            Color darkRed = new Color(135, 32, 38);
            auraLine.OverrideColor = Color.Lerp(darkRed, brightGold, wave);

            if (damageIndex >= 0)
                tooltips.Insert(damageIndex, auraLine);
            else
                tooltips.Add(auraLine);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 16)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

