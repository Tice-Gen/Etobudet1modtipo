using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class PlanteraClaw : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damageLineIndex = tooltips.FindIndex(line => line.Name == "Damage" && line.Mod == "Terraria");
            TooltipLine goreLine = new TooltipLine(Mod, "PlanteraClawGore", "-Gore weapon-")
            {
                OverrideColor = GetGoreColor()
            };

            if (damageLineIndex >= 0)
            {
                tooltips.Insert(damageLineIndex, goreLine);
            }
            else
            {
                tooltips.Add(goreLine);
            }
        }

        private static Color GetGoreColor()
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
