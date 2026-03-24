using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class OceanNostalgiaPlayer : ModPlayer
    {
        private static readonly int[] ScheduledPlayTicks = { 600, 1260, 1920, 4380 }; // 10s, 21s, 32s, 73s
        private const int RandomRollIntervalTicks = 5;
        private const int RandomRollChance = 1000;
        private const float DeepOceanDepthOffset = 420f;

        private SlotId nostalgiaSlot;
        private bool nostalgiaActive;
        private bool sequenceStarted;
        private int sequenceElapsedTicks;
        private int sequenceStep;
        private int randomRollTimer;

        public override void PostUpdate()
        {
            if (Main.dedServ || Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            UpdateNostalgiaAmbient();
        }

        public override void UpdateDead()
        {
            nostalgiaActive = false;
        }

        public override void OnEnterWorld()
        {
            nostalgiaActive = false;
            sequenceStarted = false;
            sequenceElapsedTicks = 0;
            sequenceStep = 0;
            randomRollTimer = 0;
        }

        private void UpdateNostalgiaAmbient()
        {
            if (nostalgiaActive)
            {
                if (SoundEngine.TryGetActiveSound(nostalgiaSlot, out ActiveSound activeSound))
                {
                    activeSound.Position = Player.Center;
                }
                else
                {
                    nostalgiaActive = false;
                }
            }

            if (!IsInDeepOcean())
            {
                return;
            }

            if (!sequenceStarted)
            {
                sequenceStarted = true;
                sequenceElapsedTicks = 0;
                sequenceStep = 0;
                randomRollTimer = 0;
            }

            sequenceElapsedTicks++;

            if (sequenceStep < ScheduledPlayTicks.Length)
            {
                if (sequenceElapsedTicks >= ScheduledPlayTicks[sequenceStep])
                {
                    PlayNostalgia();
                    sequenceStep++;
                }

                return;
            }

            randomRollTimer++;
            if (randomRollTimer >= RandomRollIntervalTicks)
            {
                randomRollTimer = 0;
                if (Main.rand.NextBool(RandomRollChance))
                {
                    PlayNostalgia();
                }
            }
        }

        private void PlayNostalgia()
        {
            SoundStyle nostalgiaStyle = new SoundStyle("Etobudet1modtipo/Sounds/nostalgia")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 1
            };

            nostalgiaSlot = SoundEngine.PlaySound(nostalgiaStyle, Player.Center);
            nostalgiaActive = true;
        }

        private bool IsInDeepOcean()
        {
            return Player.ZoneBeach
                && Player.wet
                && Player.Center.Y > Main.worldSurface * 16f + DeepOceanDepthOffset;
        }
    }
}
