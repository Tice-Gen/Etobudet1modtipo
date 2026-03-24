using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Rarities
{
    public class VisualRate : ModRarity
    {
        private const float CycleDurationSeconds = 5f;

        private static readonly Color[] Gradient =
        {
            new Color(173, 216, 230), // soft blue
            new Color(64, 224, 208),  // turquoise
            new Color(25, 25, 112),   // dark blue
            new Color(30, 144, 255)   // bright blue
        };

        public override Color RarityColor => GetGradientColor();

        public static Color GetGradientColor(float timeOffsetSeconds = 0f)
        {
            float t = (Main.GlobalTimeWrappedHourly + timeOffsetSeconds) % CycleDurationSeconds / CycleDurationSeconds;
            float scaled = t * Gradient.Length;
            int index = (int)scaled;
            float localT = scaled - index;

            Color from = Gradient[index % Gradient.Length];
            Color to = Gradient[(index + 1) % Gradient.Length];
            return Color.Lerp(from, to, localT);
        }

        public static Color GetOutlineColor(float timeOffsetSeconds = 0f)
        {
            Color c = GetGradientColor(timeOffsetSeconds);
            return new Color(
                (int)(c.R * 0.35f),
                (int)(c.G * 0.35f),
                (int)(c.B * 0.35f),
                c.A);
        }
    }
}
