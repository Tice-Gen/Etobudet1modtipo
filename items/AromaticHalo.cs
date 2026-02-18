using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class AromaticHalo : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.damage = 15;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(silver: 150);

            Item.defense = 5;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.Add(new TooltipLine(Mod, "AromaticHaloDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AromaticHalo.AromaticHaloDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Daybloom, 2)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 50)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 30)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AromaticHaloPlayer>().haloEquipped = true;

        }
    }

    public class AromaticHaloPlayer : ModPlayer
    {
        public bool haloEquipped;

        public override void ResetEffects()
        {
            haloEquipped = false;
        }

        public override void PostUpdate()
        {
            int projType = ModContent.ProjectileType<AniseAcs>();
            int orbitCount = 8;
            float distance = 150f;

            if (haloEquipped)
            {
                for (int i = 0; i < orbitCount; i++)
                {
                    bool exists = false;
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.owner == Player.whoAmI && proj.type == projType && proj.ai[0] == i)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists && Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            Player.Center,
                            Vector2.Zero,
                            projType,
                            15,
                            0f,
                            Player.whoAmI,
                            i,
                            distance
                        );
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Player.whoAmI && proj.type == projType)
                        proj.Kill();
                }
            }
        }
    }
}
