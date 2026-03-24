using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Global
{
    public class PressureGlobalNPC : GlobalNPC
    {
        private const int PressureDps = 3000;

        public override bool InstancePerEntity => true;

        private float pressureDamageAccumulator;

        public void ApplyPressureDrain(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !npc.active || npc.life <= 0 || npc.dontTakeDamage)
            {
                return;
            }

            pressureDamageAccumulator += PressureDps / 60f;
            int pendingDamage = (int)pressureDamageAccumulator;
            if (pendingDamage <= 0)
            {
                return;
            }

            pressureDamageAccumulator -= pendingDamage;

            int executeThreshold = Math.Max(1, (int)(npc.lifeMax * 0.01f));
            if (npc.life <= executeThreshold)
            {
                int hitDir = npc.direction == 0 ? 1 : npc.direction;
                npc.SimpleStrikeNPC(Math.Max(1, npc.life), hitDir, crit: false, knockBack: 0f);
                return;
            }

            int nonLethalDamage = Math.Min(pendingDamage, Math.Max(0, npc.life - 1));
            if (nonLethalDamage <= 0)
            {
                return;
            }

            npc.life -= nonLethalDamage;
            npc.netUpdate = true;
        }
    }
}
