using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Players
{
    public class VisualStudioCheatPlayer : ModPlayer
    {
        public bool GodModeEnabled;

        public override void PostUpdate()
        {
            if (!Player.HasBuff(ModContent.BuffType<VisualStudioBuff>()))
            {
                GodModeEnabled = false;
            }

            if (!GodModeEnabled)
            {
                return;
            }

            Player.dead = false;
            Player.ghost = false;
            Player.statLife = Player.statLifeMax2;
            Player.statMana = Player.statManaMax2;
            Player.immune = true;
            Player.immuneTime = 2;
            Player.breath = Player.breathMax;
        }

        public override bool PreKill(
            double damage,
            int hitDirection,
            bool pvp,
            ref bool playSound,
            ref bool genDust,
            ref PlayerDeathReason damageSource)
        {
            return !GodModeEnabled;
        }
    }
}
