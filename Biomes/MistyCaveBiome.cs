using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Etobudet1modtipo.Common.Systems;
using Etobudet1modtipo.Systems;
using System;

namespace Etobudet1modtipo.Biomes
{
    public class MistyCaveBiome : ModBiome
    {
        private const int FadeStartTicks = 180;
        private const int FadeDurationTicks = 120;
        private const int ParticlesPerTick = 5;

        private const int InitialAlpha = 100;
        private const float InitialScale = 3f;

        private const int GoreLifetime = 180;
        private const float GoreSpawnChance = 0.12f;


        private const float GoreMinScale = 1.8f;
        private const float GoreMaxScale = 2.6f;

        public override bool IsBiomeActive(Player player)
        {
            if (MistyCaveSystem.MistyCaveCenter == Vector2.Zero)
            {
                MistyBiomeVisuals.Enabled = false;
                return false;
            }

            float dist = Vector2.Distance(player.Center, MistyCaveSystem.MistyCaveCenter * 16f);
            bool active = dist < (MistyCaveSystem.BiomeRadius * 16f);

            MistyBiomeVisuals.Enabled = active;
            return active;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MistyCave");
        public override string BackgroundPath => "Terraria/Images/MapBG/Cave";
        public override string BestiaryIcon => "Terraria/Images/UI/Bestiary/Icon_Rank_Light";

        public override void OnInBiome(Player player)
        {
            MistyBiomeVisuals.Enabled = true;

            Vector2 center = MistyCaveSystem.MistyCaveCenter * 16f;
            float radius = MistyCaveSystem.BiomeRadius * 16f;


            for (int i = 0; i < ParticlesPerTick; i++)
            {
                Vector2 pos = center + Main.rand.NextVector2Circular(radius * 1.15f, radius * 1.15f);
                Dust d = Dust.NewDustDirect(pos, 1, 1, DustID.Smoke);

                d.scale = InitialScale;
                d.alpha = InitialAlpha;
                d.noGravity = true;
                d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.2f, 3.2f);
                d.velocity.Y -= Main.rand.NextFloat(0.1f, 0.6f);
            }


            if (Main.rand.NextFloat() < GoreSpawnChance)
            {
                Vector2 pos = center + Main.rand.NextVector2Circular(radius, radius);
                int goreType = Main.rand.Next(1087, 1094);

                float scale = Main.rand.NextFloat(GoreMinScale, GoreMaxScale);

                int id = Gore.NewGore(
                    null,
                    pos,
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.15f, 0.6f),
                    goreType,
                    scale
                );

                if (id >= 0 && id < Main.maxGore)
                {
                    Gore g = Main.gore[id];
                    g.timeLeft = GoreLifetime;
                    g.alpha = 40;
                }
            }


            for (int i = 0; i < Main.maxGore; i++)
            {
                Gore g = Main.gore[i];
                if (!g.active)
                    continue;


                if (g.timeLeft > GoreLifetime)
                    continue;

                float lifeProgress = 1f - (g.timeLeft / (float)GoreLifetime);


                g.alpha = (int)MathHelper.Lerp(40f, 255f, lifeProgress);


                g.scale *= 0.9975f;


                g.velocity *= 0.965f;
                g.velocity.Y -= 0.01f;
            }
        }
    }
}
