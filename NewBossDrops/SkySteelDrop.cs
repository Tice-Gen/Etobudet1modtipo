using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NewBossDrop
{
    public class SkySteelDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.LunarTowerSolar)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkySteel>(), 1, 1));
    
            }

            if (npc.type == NPCID.LunarTowerVortex)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkySteel>(), 1, 1));
    
            }

            if (npc.type == NPCID.LunarTowerNebula)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkySteel>(), 1, 1));
    
            }

            if (npc.type == NPCID.LunarTowerStardust)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkySteel>(), 1, 1));
    
            }
        }
    }
}
