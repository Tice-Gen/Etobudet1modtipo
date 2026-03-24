using System.Collections.Generic;
using Etobudet1modtipo.Common.Temperature;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class TemperatureArmorTooltipGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            float offset = 0f;
            bool hasTemperatureEffect =
                TemperatureRegistry.TryGetArmorTemperatureOffset(item, out offset)
                || TemperatureRegistry.TryGetAccessoryTemperatureOffset(item, out offset);

            if (!hasTemperatureEffect || offset == 0f)
            {
                return;
            }

            string key = offset > 0f
                ? "Mods.Etobudet1modtipo.UI.Temperature.ArmorWarm"
                : "Mods.Etobudet1modtipo.UI.Temperature.ArmorCool";

            string text = Language.GetTextValue(key, System.Math.Abs(offset).ToString("0.#"));
            tooltips.Add(new TooltipLine(Mod, "TemperatureArmor", text));
        }
    }
}
