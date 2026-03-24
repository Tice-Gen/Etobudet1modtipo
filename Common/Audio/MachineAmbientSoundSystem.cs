using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.Audio
{
    public class MachineAmbientSoundSystem : ModSystem
    {
        private const float SilenceStartDistance = 96f;
        private const float SilenceEndDistance = 900f;
        private const float FadeInStep = 0.08f;
        private const float FadeOutStep = 0.04f;
        private const float MinimumAudibleVolume = 0.01f;

        private static readonly Dictionary<(MachineSoundKind Kind, int X, int Y), MachineSoundState> ActiveSounds = new();

        public override void PostUpdateProjectiles()
        {
            if (Main.dedServ || Main.gameMenu)
            {
                StopAllSounds();
                return;
            }

            Player player = Main.LocalPlayer;
            if (player is null || !player.active)
            {
                StopAllSounds();
                return;
            }

            List<(MachineSoundKind Kind, int X, int Y)> keys = new(ActiveSounds.Keys);
            List<(MachineSoundKind Kind, int X, int Y)> toRemove = null;

            foreach ((MachineSoundKind Kind, int X, int Y) key in keys)
            {
                if (!ActiveSounds.TryGetValue(key, out MachineSoundState state))
                {
                    continue;
                }

                MachineSoundProfile profile = GetProfile(key.Kind);
                bool wasRefreshed = state.Refreshed;

                state.Fade = wasRefreshed
                    ? MathHelper.Clamp(state.Fade + FadeInStep, 0f, 1f)
                    : MathHelper.Clamp(state.Fade - FadeOutStep, 0f, 1f);

                float targetVolume = profile.BaseVolume * state.Fade * GetDistanceVolume(player.Center, state.Position);

                if (SoundEngine.TryGetActiveSound(state.SoundSlot, out ActiveSound activeSound))
                {
                    activeSound.Position = state.Position;
                    activeSound.Volume = targetVolume;

                    if (targetVolume <= MinimumAudibleVolume && !state.Refreshed)
                    {
                        activeSound.Stop();
                    }
                }
                else if (state.Refreshed && targetVolume > MinimumAudibleVolume)
                {
                    SoundStyle style = new SoundStyle(profile.Path)
                    {
                        IsLooped = true,
                        Volume = targetVolume,
                        PitchVariance = 0f,
                        MaxInstances = 12,
                        SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                    };

                    state.SoundSlot = SoundEngine.PlaySound(style, state.Position);
                }

                state.Refreshed = false;
                ActiveSounds[key] = state;

                if (state.Fade <= 0f && !wasRefreshed)
                {
                    toRemove ??= new List<(MachineSoundKind Kind, int X, int Y)>();
                    toRemove.Add(key);
                }
            }

            if (toRemove is null)
            {
                return;
            }

            foreach ((MachineSoundKind Kind, int X, int Y) key in toRemove)
            {
                ActiveSounds.Remove(key);
            }
        }

        public override void OnWorldUnload() => StopAllSounds();

        public override void Unload() => StopAllSounds();

        public static void RefreshMachineSound(MachineSoundKind kind, Point16 topLeft, Vector2 position)
        {
            if (Main.dedServ)
            {
                return;
            }

            (MachineSoundKind Kind, int X, int Y) key = (kind, topLeft.X, topLeft.Y);
            if (!ActiveSounds.TryGetValue(key, out MachineSoundState state))
            {
                state = new MachineSoundState
                {
                    SoundSlot = SlotId.Invalid,
                    Fade = 0f
                };
            }

            state.Position = position;
            state.Refreshed = true;
            state.Fade = MathHelper.Clamp(state.Fade + FadeInStep, 0f, 1f);

            if (Main.gameMenu || Main.LocalPlayer is null || !Main.LocalPlayer.active)
            {
                ActiveSounds[key] = state;
                return;
            }

            float targetVolume = GetProfile(kind).BaseVolume * state.Fade * GetDistanceVolume(Main.LocalPlayer.Center, position);
            if (targetVolume > MinimumAudibleVolume && !SoundEngine.TryGetActiveSound(state.SoundSlot, out _))
            {
                SoundStyle style = new SoundStyle(GetProfile(kind).Path)
                {
                    IsLooped = true,
                    Volume = targetVolume,
                    PitchVariance = 0f,
                    MaxInstances = 24,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                };

                state.SoundSlot = SoundEngine.PlaySound(style, position);
            }

            ActiveSounds[key] = state;
        }

        private static float GetDistanceVolume(Vector2 listenerCenter, Vector2 soundCenter)
        {
            float distance = Vector2.Distance(listenerCenter, soundCenter);
            if (distance <= SilenceStartDistance)
            {
                return 1f;
            }

            float volume = 1f - (distance - SilenceStartDistance) / (SilenceEndDistance - SilenceStartDistance);
            return MathHelper.Clamp(volume, 0f, 1f);
        }

        private static void StopAllSounds()
        {
            if (!Main.dedServ)
            {
                foreach (MachineSoundState state in ActiveSounds.Values)
                {
                    if (SoundEngine.TryGetActiveSound(state.SoundSlot, out ActiveSound activeSound))
                    {
                        activeSound.Stop();
                    }
                }
            }

            ActiveSounds.Clear();
        }

        private static MachineSoundProfile GetProfile(MachineSoundKind kind)
        {
            return kind switch
            {
                MachineSoundKind.IndustrialCooler => new MachineSoundProfile("Etobudet1modtipo/Sounds/IndastrilFanWorking", 0.85f),
                MachineSoundKind.Fan => new MachineSoundProfile("Etobudet1modtipo/Sounds/FanWorking", 0.75f),
                _ => new MachineSoundProfile("Etobudet1modtipo/Sounds/FanWorking", 0.7f)
            };
        }

        public enum MachineSoundKind
        {
            IndustrialCooler = 0,
            Fan = 1
        }

        private struct MachineSoundState
        {
            public SlotId SoundSlot;
            public Vector2 Position;
            public float Fade;
            public bool Refreshed;
        }

        private readonly struct MachineSoundProfile
        {
            public MachineSoundProfile(string path, float baseVolume)
            {
                Path = path;
                BaseVolume = baseVolume;
            }

            public string Path { get; }

            public float BaseVolume { get; }
        }
    }
}
