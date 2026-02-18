using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NewBossDrop
{
    public class PlanteraDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            if (npc.type == NPCID.Plantera)
            {

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestSpear>(), 1, 1, 1));


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloweringChlorophyteBar>(), 1, 20, 30));
            }
        }
    }
}
