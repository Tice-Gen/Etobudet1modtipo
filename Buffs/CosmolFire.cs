using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Global;

namespace Etobudet1modtipo.Buffs
{
    public class CosmolFire : ModBuff
    {
        public override string Texture => "Etobudet1modtipo/Buffs/StarBurn";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CosmolFireGlobalNPC>().cosmolFireActive = true;
        }
    }
}
