using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Rarities
{
    public class SupremeRare : ModRarity
    {
        private const float CycleSeconds = 2f;

        private static readonly Color[] RarityGradient =
        {
            new Color(4, 10, 38),     // very dark navy
            new Color(6, 18, 62),     // deep night blue
            new Color(10, 30, 96)     // dark cobalt blue
        };

        private static readonly Color[] OutlineGradient =
        {
            new Color(34, 211, 238),  // neon cyan
            new Color(59, 130, 246),  // electric blue
            new Color(186, 230, 253)  // icy highlight
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
