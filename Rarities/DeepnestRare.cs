using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Rarities
{
    public class DeepnestRare : ModRarity
    {
        private const float RarityCycleSeconds = 5f;
        private const float OutlineCycleSeconds = 4f;

        private static readonly Color[] RarityGradient =
        {
            new Color(0, 90, 92),   // dark turquoise
            new Color(20, 58, 98),  // dark bluish-cyan
            new Color(28, 96, 84)   // muted greenish-turquoise
        };

        private static readonly Color[] OutlineGradient =
        {
            new Color(140, 255, 240), // light turquoise
            new Color(150, 215, 255), // light blue
            new Color(135, 180, 255)  // light blue (deeper)
        };

        public override Color RarityColor => GetRarityColor();

        public static Color GetRarityColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(RarityGradient, RarityCycleSeconds, timeOffsetSeconds);
        }

        public static Color GetOutlineColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(OutlineGradient, OutlineCycleSeconds, timeOffsetSeconds);
        }

        private static Color SampleGradient(Color[] gradient, float cycleSeconds, float timeOffsetSeconds)
        {
            float t = (Main.GlobalTimeWrappedHourly + timeOffsetSeconds) % cycleSeconds / cycleSeconds;
            float scaled = t * gradient.Length;
            int index = (int)scaled;
            float localT = scaled - index;

            Color from = gradient[index % gradient.Length];
            Color to = gradient[(index + 1) % gradient.Length];
            return Color.Lerp(from, to, localT);
        }
    }
}
