using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Etobudet1modtipo.NPCs;
using Etobudet1modtipo.Systems.InfernalAwakening;

namespace Etobudet1modtipo.Global
{
    public class BossDefeatGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);


            var infernal = InfernalAwakeningSystem.Instance;
            if (infernal == null) return;


            if (npc.type == NPCID.SkeletronHead)
            {
                if (!infernal.SkeletronDefeated)
                {
                    infernal.SkeletronDefeated = true;
                    infernal.TryActivateInfernal();
                }
            }


            int aniseType = ModContent.NPCType<AniseKingSlime>();
            if (npc.type == aniseType)
            {
                if (!infernal.AniseDefeated)
                {
                    infernal.AniseDefeated = true;
                    infernal.TryActivateInfernal();
                }
            }
        }
    }
}
