using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class TooltipCleanupGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(ShouldRemoveTooltipLine);
        }

        private static bool ShouldRemoveTooltipLine(TooltipLine line)
        {
            if (string.IsNullOrWhiteSpace(line.Text))
            {
                return true;
            }

            string trimmedText = line.Text.Trim();
            return trimmedText.StartsWith("Mods.Etobudet1modtipo.", StringComparison.Ordinal);
        }
    }
}
