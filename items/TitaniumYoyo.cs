using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Players;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Etobudet1modtipo.items
{
    public class TitaniumYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<TitaniumYoyoProj>();
            Item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 16)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil);

            recipe.AddOnCraftCallback(static (_, item, _, _) =>
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    return;
                }

                Player localPlayer = Main.LocalPlayer;
                if (localPlayer == null || !localPlayer.active)
                {
                    return;
                }

                localPlayer.GetModPlayer<OrbitAchievementPlayer>().RegisterOrbitalYoyoCraft(item.type);
            });

            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Deadly shards spin around the yoyo.\nPierces armor extremely well.")));
            int damageIndex = tooltips.FindIndex(t => t.Name == "Damage");
            TooltipLine orbitalLine = new TooltipLine(Mod, "OrbitalYoyoTag", "-Orbital Yoyo-");
            float wave = (float)((Math.Sin(Main.GlobalTimeWrappedHourly * 2f) + 1.0) * 0.5);
            orbitalLine.OverrideColor = Color.Lerp(new Color(0, 120, 180), Color.LimeGreen, wave);

            if (damageIndex >= 0)
                tooltips.Insert(damageIndex, orbitalLine);
            else
                tooltips.Add(orbitalLine);
        }
    }
}

