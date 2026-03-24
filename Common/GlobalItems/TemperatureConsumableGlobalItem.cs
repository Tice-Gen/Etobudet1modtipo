using System.Collections.Generic;
using Etobudet1modtipo.Common.Temperature;
using Etobudet1modtipo.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class TemperatureConsumableGlobalItem : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            if (!TemperatureRegistry.TryGetConsumableEffect(item, out TemperatureConsumableEffect effect))
            {
                return;
            }

            player.GetModPlayer<TemperaturePlayer>().ApplyFoodTemperatureEffect(effect.Degrees, effect.DurationTicks);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!TemperatureRegistry.TryGetConsumableEffect(item, out TemperatureConsumableEffect effect))
            {
                return;
            }

            string key = effect.Degrees >= 0f
                ? "Mods.Etobudet1modtipo.UI.Temperature.TooltipWarm"
                : "Mods.Etobudet1modtipo.UI.Temperature.TooltipCool";

            string text = Language.GetTextValue(
                key,
                System.Math.Abs(effect.Degrees).ToString("0.#"),
                TemperatureRegistry.FormatDuration(effect.DurationTicks));

            tooltips.Add(new TooltipLine(Mod, "TemperatureEffect", text));
        }
    }
}
