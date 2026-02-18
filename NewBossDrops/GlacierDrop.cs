using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NewBossDrop
{
    public class GlacierDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            if (npc.type == NPCID.IceQueen)
            {

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Glacier>(), 5, 1, 1));
            }
        }
    }
}
