using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class Gearanise : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.crit = 50;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 11;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.reuseDelay = 30;

            Item.shoot = ModContent.ProjectileType<Gearwheel>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.None;
            Item.consumable = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damageLineIndex = tooltips.FindIndex(line => line.Name == "Damage" && line.Mod == "Terraria");
            TooltipLine massiveLine = new TooltipLine(Mod, "GearaniseMassive", "-MASSIVE-")
            {
                OverrideColor = GetMassiveColor()
            };

            if (damageLineIndex >= 0)
            {
                tooltips.Insert(damageLineIndex, massiveLine);
            }
            else
            {
                tooltips.Add(massiveLine);
            }

            tooltips.Add(new TooltipLine(Mod, "GearaniseDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.Gearanise.GearaniseDesc")));
        }

        private static Color GetMassiveColor()
        {
            Color darkRed = new Color(85, 0, 0);
            Color duskyCrimson = new Color(120, 24, 38);
            Color duskyRed = new Color(145, 40, 40);

            float t = (Main.GlobalTimeWrappedHourly * 1.9f) % 3f;
            if (t < 1f)
            {
                return Color.Lerp(darkRed, duskyCrimson, t);
            }

            if (t < 2f)
            {
                return Color.Lerp(duskyCrimson, duskyRed, t - 1f);
            }

            return Color.Lerp(duskyRed, darkRed, t - 2f);
        }
    }
}
