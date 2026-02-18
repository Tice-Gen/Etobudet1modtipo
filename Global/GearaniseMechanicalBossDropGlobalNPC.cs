using Etobudet1modtipo.items;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Global
{
    public class GearaniseMechanicalBossDropGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gearanise>(), 3));

            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                npcLoot.Add(ItemDropRule.ByCondition(
                    new Conditions.MissingTwin(),
                    ModContent.ItemType<Gearanise>(),
                    3
                ));
            }
        }
    }
}
