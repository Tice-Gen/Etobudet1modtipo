using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Etobudet1modtipo.Achievements;
using Etobudet1modtipo;

namespace Etobudet1modtipo.Players
{
    public class AnimalsSaverAchievementPlayer : ModPlayer
    {
        public bool AnimalsSaverUnlocked;
        public int KindnessPoints;

        private const int RequiredKindnessPoints = 5;
        private const float LuckPerKindnessPoint = 0.0001f; // 0.01%

        public override void SaveData(TagCompound tag)
        {
            // Achievement persistence is handled by tModLoader achievement system.
        }

        public override void LoadData(TagCompound tag)
        {
            AnimalsSaverUnlocked = false;
            KindnessPoints = 0;
        }

        public override void Initialize()
        {
            AnimalsSaverUnlocked = false;
            KindnessPoints = 0;
        }

        public override void ModifyLuck(ref float luck)
        {
            luck += KindnessPoints * LuckPerKindnessPoint;
            if (luck > 1f)
            {
                luck = 1f;
            }
            else if (luck < -1f)
            {
                luck = -1f;
            }
        }

        public void AddKindnessPoint()
        {
            if (KindnessPoints >= RequiredKindnessPoints)
            {
                KindnessPoints = RequiredKindnessPoints;
                TryUnlockAnimalsSaver();

                if (Main.netMode == NetmodeID.Server)
                {
                    SyncKindnessToOwnerClient();
                }

                return;
            }

            KindnessPoints++;

            if (Main.netMode == NetmodeID.Server)
            {
                SyncKindnessToOwnerClient();
            }

            if (KindnessPoints == RequiredKindnessPoints)
            {
                TryUnlockAnimalsSaver();
            }
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            AnimalsSaverUnlocked = IsAnimalsSaverAchievementCompleted();

            if (KindnessPoints == RequiredKindnessPoints)
            {
                TryUnlockAnimalsSaver();
            }
        }

        public void TryUnlockAnimalsSaver()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            if (IsAnimalsSaverAchievementCompleted())
            {
                AnimalsSaverUnlocked = true;
                return;
            }

            if (KindnessPoints != RequiredKindnessPoints)
            {
                return;
            }

            AnimalsSaverUnlocked = true;

            if (Player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<AnimalsSaver>().Complete();
            }

            if (Main.netMode == NetmodeID.Server)
            {
                SendAnimalsSaverUnlockToOwnerClient();
            }
        }

        internal void ApplyKindnessFromNetwork(int points)
        {
            if (points < 0)
            {
                KindnessPoints = 0;
                return;
            }

            KindnessPoints = points > RequiredKindnessPoints ? RequiredKindnessPoints : points;
        }

        internal void ReceiveAnimalsSaverUnlockFromNetwork()
        {
            if (AnimalsSaverUnlocked)
            {
                return;
            }

            if (KindnessPoints < RequiredKindnessPoints)
            {
                KindnessPoints = RequiredKindnessPoints;
            }

            ModContent.GetInstance<AnimalsSaver>().Complete();
            AnimalsSaverUnlocked = IsAnimalsSaverAchievementCompleted();
        }

        public bool IsAnimalsSaverCompletedForUi()
        {
            return IsAnimalsSaverAchievementCompleted();
        }

        private void SyncKindnessToOwnerClient()
        {
            if (Player.whoAmI < 0 || Player.whoAmI >= Main.maxPlayers)
            {
                return;
            }

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)Etobudet1modtipo.PacketType.SyncKindnessPoints);
            packet.Write((byte)Player.whoAmI);
            packet.Write(KindnessPoints);
            packet.Send(toClient: Player.whoAmI);
        }

        private void SendAnimalsSaverUnlockToOwnerClient()
        {
            if (Player.whoAmI < 0 || Player.whoAmI >= Main.maxPlayers)
            {
                return;
            }

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)Etobudet1modtipo.PacketType.UnlockAnimalsSaver);
            packet.Write((byte)Player.whoAmI);
            packet.Send(toClient: Player.whoAmI);

        }

        private static bool IsAnimalsSaverAchievementCompleted()
        {
            AnimalsSaver achievementContent = ModContent.GetInstance<AnimalsSaver>();
            object achievementObj = achievementContent?.Achievement;
            if (achievementObj == null)
            {
                return false;
            }

            Type type = achievementObj.GetType();
            PropertyInfo isCompletedProperty = type.GetProperty("IsCompleted", BindingFlags.Instance | BindingFlags.Public);
            if (isCompletedProperty != null && isCompletedProperty.PropertyType == typeof(bool))
            {
                return (bool)isCompletedProperty.GetValue(achievementObj);
            }

            PropertyInfo completedProperty = type.GetProperty("Completed", BindingFlags.Instance | BindingFlags.Public);
            if (completedProperty != null && completedProperty.PropertyType == typeof(bool))
            {
                return (bool)completedProperty.GetValue(achievementObj);
            }

            return false;
        }
    }
}
