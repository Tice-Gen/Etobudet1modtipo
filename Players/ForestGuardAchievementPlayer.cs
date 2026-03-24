using System;
using System.Reflection;
using Etobudet1modtipo.Achievements;
using Etobudet1modtipo.Systems.InfernalAwakening;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class ForestGuardAchievementPlayer : ModPlayer
    {
        public bool ForestGuardUnlocked;

        public override void Initialize()
        {
            ForestGuardUnlocked = false;
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            ForestGuardUnlocked = IsForestGuardAchievementCompleted();
            if (ForestGuardUnlocked)
            {
                return;
            }

            InfernalAwakeningSystem infernal = InfernalAwakeningSystem.Instance;
            if (infernal?.AniseKingSlimeDefeated != true)
            {
                return;
            }

            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            ModContent.GetInstance<ForestGuard>().Complete();
            ForestGuardUnlocked = IsForestGuardAchievementCompleted();
        }

        public bool IsForestGuardCompletedForUi()
        {
            return IsForestGuardAchievementCompleted();
        }

        private static bool IsForestGuardAchievementCompleted()
        {
            ForestGuard achievementContent = ModContent.GetInstance<ForestGuard>();
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
