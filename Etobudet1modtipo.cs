using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Players;
using Etobudet1modtipo.Subworlds;

namespace Etobudet1modtipo
{
    public class Etobudet1modtipo : Mod
    {
        internal enum PacketType : byte
        {
            SyncKindnessPoints = 1,
            UnlockAnimalsSaver = 2
        }

        public static int AniseForestMusicSlot = -1;
        public static int MistyCaveMusicSlot = -1;
        public static string OtherWatersIdentifier = string.Empty;
        public static bool OtherWatersRegistered;
        public static ModKeybind DeepSeaDashKeybind;

        private static Mod subworldLibraryMod;

        public override void Load()
        {
            DeepSeaDashKeybind = KeybindLoader.RegisterKeybind(this, "Deep Sea Dash", "Mouse4");
            TryRegisterOtherWatersSubworld();

            if (!Main.dedServ)
            {
                string anisePath = "Sounds/Music/AniseForestTheme";
                string mistyPath = "Sounds/Music/MistyCave";

                if (MusicLoader.GetMusicSlot(this, anisePath) == -1)
                    MusicLoader.AddMusic(this, anisePath);

                if (MusicLoader.GetMusicSlot(this, mistyPath) == -1)
                    MusicLoader.AddMusic(this, mistyPath);

                AniseForestMusicSlot = MusicLoader.GetMusicSlot(this, anisePath);
                MistyCaveMusicSlot = MusicLoader.GetMusicSlot(this, mistyPath);
            }
        }

        public override void Unload()
        {
            AniseForestMusicSlot = -1;
            MistyCaveMusicSlot = -1;
            OtherWatersIdentifier = string.Empty;
            OtherWatersRegistered = false;
            subworldLibraryMod = null;
            DeepSeaDashKeybind = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType packetType = (PacketType)reader.ReadByte();

            switch (packetType)
            {
                case PacketType.SyncKindnessPoints:
                {
                    byte playerIndex = reader.ReadByte();
                    int points = reader.ReadInt32();
                    if (playerIndex >= Main.maxPlayers)
                    {
                        break;
                    }

                    Player player = Main.player[playerIndex];
                    if (player == null || !player.active)
                    {
                        break;
                    }

                    player.GetModPlayer<AnimalsSaverAchievementPlayer>().ApplyKindnessFromNetwork(points);
                    break;
                }

                case PacketType.UnlockAnimalsSaver:
                {
                    byte playerIndex = reader.ReadByte();
                    if (playerIndex >= Main.maxPlayers || playerIndex != Main.myPlayer)
                    {
                        break;
                    }

                    Player player = Main.player[playerIndex];
                    if (player == null || !player.active)
                    {
                        break;
                    }

                    player.GetModPlayer<AnimalsSaverAchievementPlayer>().ReceiveAnimalsSaverUnlockFromNetwork();
                    break;
                }

                default:
                    Logger.Warn($"Unknown packet type: {(byte)packetType}");
                    break;
            }
        }

        public static bool IsOtherWatersAvailable()
        {
            return subworldLibraryMod != null && OtherWatersRegistered;
        }

        public static bool IsOtherWatersActive()
        {
            if (!IsOtherWatersAvailable())
            {
                return false;
            }

            try
            {
                foreach (string id in GetOtherWatersIdCandidates())
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    if (subworldLibraryMod.Call("IsActive", id) is bool active && active)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool EnterOtherWaters()
        {
            if (!IsOtherWatersAvailable())
            {
                return false;
            }

            try
            {
                foreach (string id in GetOtherWatersIdCandidates())
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    try
                    {
                        object result = subworldLibraryMod.Call("Enter", id);
                        if (result is bool entered && entered)
                        {
                            return true;
                        }

                        ModContent.GetInstance<Etobudet1modtipo>().Logger.Warn(
                            $"OtherWaters enter attempt returned '{result ?? "null"}' for id '{id}'.");
                    }
                    catch (Exception ex)
                    {
                        ModContent.GetInstance<Etobudet1modtipo>().Logger.Warn(
                            $"OtherWaters enter attempt threw for id '{id}': {ex.Message}");
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool ExitSubworld()
        {
            if (subworldLibraryMod == null)
            {
                return false;
            }

            try
            {
                object result = subworldLibraryMod.Call("Exit");
                return result is bool exited && exited;
            }
            catch
            {
                return false;
            }
        }

        private void TryRegisterOtherWatersSubworld()
        {
            OtherWatersIdentifier = string.Empty;
            OtherWatersRegistered = false;
            subworldLibraryMod = null;

            if (!ModLoader.TryGetMod("SubworldLibrary", out Mod subworldLibrary))
            {
                Logger.Info("OtherWaters: SubworldLibrary not found, subworld disabled.");
                return;
            }

            subworldLibraryMod = subworldLibrary;

            if (TryRegisterWithSignature(
                () => subworldLibrary.Call(
                    "Register",
                    this,
                    OtherWatersSubworld.Name,
                    OtherWatersSubworld.WorldWidthTiles,
                    OtherWatersSubworld.WorldHeightTiles,
                    OtherWatersSubworld.BuildTasks()),
                id => OtherWatersIdentifier = id))
            {
                FinalizeOtherWatersRegistration();
                return;
            }

            if (TryRegisterWithSignature(
                () => subworldLibrary.Call(
                    "Register",
                    OtherWatersSubworld.Name,
                    OtherWatersSubworld.WorldWidthTiles,
                    OtherWatersSubworld.WorldHeightTiles,
                    OtherWatersSubworld.BuildTasks()),
                id => OtherWatersIdentifier = id))
            {
                FinalizeOtherWatersRegistration();
                return;
            }

            if (TryRegisterWithSignature(
                () => subworldLibrary.Call(
                    "Register",
                    OtherWatersSubworld.Name,
                    OtherWatersSubworld.WorldWidthTiles,
                    OtherWatersSubworld.WorldHeightTiles,
                    OtherWatersSubworld.BuildTasks(),
                    null,
                    null,
                    null,
                    true,
                    false,
                    true),
                id => OtherWatersIdentifier = id))
            {
                FinalizeOtherWatersRegistration();
                return;
            }

            Logger.Error("OtherWaters registration failed for all known SubworldLibrary signatures.");
        }

        private bool TryRegisterWithSignature(Func<object> registerCall, Action<string> setIdentifier)
        {
            try
            {
                object result = registerCall();
                if (result is string identifier && !string.IsNullOrWhiteSpace(identifier))
                {
                    setIdentifier(identifier);
                    return true;
                }

                Logger.Warn($"OtherWaters registration returned invalid id '{result ?? "null"}'.");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn($"OtherWaters registration signature failed: {ex.Message}");
                return false;
            }
        }

        private void FinalizeOtherWatersRegistration()
        {
            OtherWatersRegistered = true;

            if (string.IsNullOrWhiteSpace(OtherWatersIdentifier))
            {
                OtherWatersIdentifier = $"{Name}:{OtherWatersSubworld.Name}";
            }

            Logger.Info($"OtherWaters registered. Identifier='{OtherWatersIdentifier}'.");
        }

        public static string[] GetOtherWatersIdCandidates()
        {
            string modName = ModContent.GetInstance<Etobudet1modtipo>().Name;

            return new[]
            {
                OtherWatersIdentifier,
                OtherWatersSubworld.Name,
                $"{modName}:{OtherWatersSubworld.Name}",
                $"{modName}/{OtherWatersSubworld.Name}",
                $"SubworldLibrary/{OtherWatersSubworld.Name}"
            };
        }
    }
}
