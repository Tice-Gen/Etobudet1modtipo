using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Projectiles;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class Runeosama : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 5000;
            Item.width = 28;
            Item.height = 28;

            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(silver: 0);
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = false;


            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<RuneosamaInvisibleProj>();
            Item.shootSpeed = 10f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            List<int> indexesToRemove = new List<int>();
            

            for (int i = 0; i < tooltips.Count; i++)
            {
                var line = tooltips[i];
                

                if (line.Name == "Damage" || 
                    line.Name == "Speed" || 
                    line.Name == "Knockback")
                {
                    indexesToRemove.Add(i);
                }

                else if (line.Text.Contains(" damage") ||
                         line.Text.Contains(" speed") ||
                         line.Text.Contains(" knockback"))
                {
                    indexesToRemove.Add(i);
                }
            }
            

            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                tooltips.RemoveAt(indexesToRemove[i]);
            }
            

            tooltips.Add(new TooltipLine(Mod, "AniseHeadDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.Runeosama.AniseHeadDesc")));
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shootSpeed = 50f;
            }
            else
            {
                Item.shootSpeed = 10f;
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SkySteel>(), 25)
                .AddIngredient(ItemID.FragmentSolar, 200)
                .AddIngredient(ItemID.FragmentVortex, 200)
                .AddIngredient(ItemID.FragmentNebula, 200)
                .AddIngredient(ItemID.FragmentStardust, 200)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
