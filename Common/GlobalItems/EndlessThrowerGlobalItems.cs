using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class EndlessThrowerGlobalItems : GlobalItem
    {
        public override void SetDefaults(Item item)
        {

            if (item.type == ItemID.VampireKnives
                || item.type == ItemID.ScourgeoftheCorruptor
                || item.type == ItemID.DayBreak
                || item.type == ItemID.ShadowFlameKnife)
            {
                item.DamageType = ModContent.GetInstance<EndlessThrower>();
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.damage <= 0)
            {
                return;
            }

            if (!item.DamageType.CountsAsClass(ModContent.GetInstance<EndlessThrower>()))
            {
                return;
            }

            for (int i = tooltips.Count - 1; i >= 0; i--)
            {
                TooltipLine line = tooltips[i];
                if (line.Text != null &&
                    line.Text.IndexOf("does not consume ammo", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    tooltips.RemoveAt(i);
                }
            }

            tooltips.Add(new TooltipLine(Mod, "EndlessThrowerNoAmmo", "Does not consume ammo"));
        }
    }
}
