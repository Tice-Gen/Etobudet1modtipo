using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Rarities
{
    public class Cuborare : ModRarity
    {
        private const float CycleSeconds = 2.4f;

        private static readonly Color[] RarityGradient =
        {
            new Color(255, 95, 24),   // hot orange
            new Color(255, 59, 28),   // vivid red-orange
            new Color(210, 24, 24),   // strong red
            new Color(255, 138, 38)   // bright orange
        };

        private static readonly Color[] OutlineGradient =
        {
            new Color(255, 201, 120), // warm highlight
            new Color(255, 114, 76),  // orange-red edge
            new Color(255, 78, 44)    // deep orange-red
        };

        public override Color RarityColor => GetRarityColor();

        public static Color GetRarityColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(RarityGradient, timeOffsetSeconds);
        }

        public static Color GetOutlineColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(OutlineGradient, timeOffsetSeconds);
        }

        private static Color SampleGradient(Color[] gradient, float timeOffsetSeconds)
        {
            float t = (Main.GlobalTimeWrappedHourly + timeOffsetSeconds) % CycleSeconds / CycleSeconds;
            float scaled = t * gradient.Length;
            int index = (int)scaled;
            float localT = scaled - index;

            Color from = gradient[index % gradient.Length];
            Color to = gradient[(index + 1) % gradient.Length];
            return Color.Lerp(from, to, localT);
        }
    }
}
