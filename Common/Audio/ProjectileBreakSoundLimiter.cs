using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace Etobudet1modtipo.Common.Audio
{
    internal static class ProjectileBreakSoundLimiter
    {
        private const uint RepeatDelayTicks = 6;
        private static uint lastBrokenStoneTick;
        private static uint lastBrokenKunaiTick;
        private static readonly SlotId[] BrokenStoneSlots = new SlotId[2];
        private static readonly SlotId[] BrokenKunaiSlots = new SlotId[2];

        public static void TryPlayBrokenStone(SoundStyle style, Vector2 position)
        {
            TryPlay(style, position, BrokenStoneSlots, ref lastBrokenStoneTick);
        }

        public static void TryPlayBrokenKunai(SoundStyle style, Vector2 position)
        {
            TryPlay(style, position, BrokenKunaiSlots, ref lastBrokenKunaiTick);
        }

        private static void TryPlay(SoundStyle style, Vector2 position, SlotId[] trackedSlots, ref uint lastTick)
        {
            if (Main.dedServ)
                return;

            uint now = Main.GameUpdateCount;
            if (lastTick != 0 && now - lastTick < RepeatDelayTicks)
                return;

            int activeCount = 0;
            int freeSlotIndex = -1;
            for (int i = 0; i < trackedSlots.Length; i++)
            {
                if (SoundEngine.TryGetActiveSound(trackedSlots[i], out _))
                {
                    activeCount++;
                }
                else if (freeSlotIndex == -1)
                {
                    freeSlotIndex = i;
                }
            }

            if (activeCount >= trackedSlots.Length)
                return;

            lastTick = now;
            SlotId slot = SoundEngine.PlaySound(style, position);
            trackedSlots[freeSlotIndex == -1 ? 0 : freeSlotIndex] = slot;
        }
    }
}
