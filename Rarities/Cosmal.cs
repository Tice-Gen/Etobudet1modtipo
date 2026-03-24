using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Rarities
{
    public class Cosmal : ModRarity
    {
        private const float FillCycleSeconds = 7f;
        private const float OutlineCycleSeconds = 4.5f;
        private const float NebulaCycleSeconds = 6f;

        private static readonly Color[] FillGradient =
        {
            new Color(2, 5, 18),
            new Color(6, 12, 34),
            new Color(11, 18, 52),
            new Color(16, 11, 42)
        };

        private static readonly Color[] OutlineGradient =
        {
            new Color(207, 188, 255),
            new Color(182, 166, 244),
            new Color(227, 216, 255),
            new Color(196, 180, 255)
        };

        private static readonly Color[] NebulaGradient =
        {
            new Color(34, 40, 92),
            new Color(52, 42, 122),
            new Color(24, 62, 110)
        };

        public override Color RarityColor => GetRarityColor();

        public static Color GetRarityColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(FillGradient, FillCycleSeconds, timeOffsetSeconds);
        }

        public static Color GetOutlineColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(OutlineGradient, OutlineCycleSeconds, timeOffsetSeconds);
        }

        public static Color GetNebulaColor(float timeOffsetSeconds = 0f)
        {
            return SampleGradient(NebulaGradient, NebulaCycleSeconds, timeOffsetSeconds);
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
