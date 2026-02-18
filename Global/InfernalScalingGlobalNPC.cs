using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Systems.InfernalAwakening;

namespace Etobudet1modtipo.Global
{
    public class InfernalScalingGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private bool scaled = false;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            scaled = false;
            TryScaleIfUnderworld(npc);
        }

        private void TryScaleIfUnderworld(NPC npc)
        {
            var infernal = InfernalAwakeningSystem.Instance;
            if (infernal == null) return;
            if (!infernal.InfernalActive) return;

            if (npc.friendly || npc.townNPC || npc.dontTakeDamage) return;
            if (npc.boss) return;

            int tileY = (int)(npc.Center.Y / 16f);
            int underworldThreshold = Main.maxTilesY - 200;


            if (tileY < underworldThreshold) return;

            if (!scaled)
            {
                scaled = true;
                npc.lifeMax = Math.Max(1, (int)(npc.lifeMax * 1.5f));
                npc.life = npc.lifeMax;
                npc.damage = (int)(npc.damage * 1.5f);
                npc.netUpdate = true;
            }
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
            if (!scaled) TryScaleIfUnderworld(npc);
        }
    }
}
