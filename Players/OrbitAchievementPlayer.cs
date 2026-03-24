using System;
using System.Reflection;
using Etobudet1modtipo.Achievements;
using Etobudet1modtipo.items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Etobudet1modtipo.Players
{
    public class OrbitAchievementPlayer : ModPlayer
    {
        private const string CraftMaskTagKey = "OrbitCraftMask";
        private const byte TitaniumYoyoCraftedMask = 1 << 0;
        private const byte StarAdamantiteYoyoCraftedMask = 1 << 1;
        private const byte ChlorophyteYoyoCraftedMask = 1 << 2;
        private const byte GoldenLineCraftedMask = 1 << 3;
        private const byte PlatinumLineCraftedMask = 1 << 4;
        private const byte RequiredCraftMask = TitaniumYoyoCraftedMask | StarAdamantiteYoyoCraftedMask | ChlorophyteYoyoCraftedMask | GoldenLineCraftedMask | PlatinumLineCraftedMask;

        private byte orbitalYoyoCraftMask;

        public bool OrbitUnlocked;

        public override void SaveData(TagCompound tag)
        {
            if (orbitalYoyoCraftMask != 0)
            {
                tag[CraftMaskTagKey] = orbitalYoyoCraftMask;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            orbitalYoyoCraftMask = tag.ContainsKey(CraftMaskTagKey) ? tag.GetByte(CraftMaskTagKey) : (byte)0;
            OrbitUnlocked = IsOrbitAchievementCompleted();

            if (OrbitUnlocked)
            {
                orbitalYoyoCraftMask = RequiredCraftMask;
            }
        }

        public override void Initialize()
        {
            orbitalYoyoCraftMask = 0;
            OrbitUnlocked = false;
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            OrbitUnlocked = IsOrbitAchievementCompleted();
            if (OrbitUnlocked)
            {
                orbitalYoyoCraftMask = RequiredCraftMask;
                return;
            }

            if (orbitalYoyoCraftMask == RequiredCraftMask)
            {
                TryUnlockOrbit();
            }
        }

        public void RegisterOrbitalYoyoCraft(int itemType)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            byte craftBit = GetCraftBitForItemType(itemType);
            if (craftBit == 0)
            {
                return;
            }

            orbitalYoyoCraftMask |= craftBit;

            if (orbitalYoyoCraftMask == RequiredCraftMask)
            {
                TryUnlockOrbit();
            }
        }

        public void TryUnlockOrbit()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            if (IsOrbitAchievementCompleted())
            {
                OrbitUnlocked = true;
                orbitalYoyoCraftMask = RequiredCraftMask;
                return;
            }

            if (orbitalYoyoCraftMask != RequiredCraftMask)
            {
                return;
            }

            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            ModContent.GetInstance<Orbit>().Complete();
            OrbitUnlocked = IsOrbitAchievementCompleted();

            if (OrbitUnlocked)
            {
                orbitalYoyoCraftMask = RequiredCraftMask;
            }
        }

        private static byte GetCraftBitForItemType(int itemType)
        {
            if (itemType == ModContent.ItemType<TitaniumYoyo>())
            {
                return TitaniumYoyoCraftedMask;
            }

            if (itemType == ModContent.ItemType<StarAdamantiteYoyo>())
            {
                return StarAdamantiteYoyoCraftedMask;
            }

            if (itemType == ModContent.ItemType<ChlorophyteYoyo>())
            {
                return ChlorophyteYoyoCraftedMask;
            }

            if (itemType == ModContent.ItemType<GoldenLine>())
            {
                return GoldenLineCraftedMask;
            }

            if (itemType == ModContent.ItemType<PlatinumLine>())
            {
                return PlatinumLineCraftedMask;
            }

            return 0;
        }

        private static bool IsOrbitAchievementCompleted()
        {
            Orbit achievementContent = ModContent.GetInstance<Orbit>();
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
