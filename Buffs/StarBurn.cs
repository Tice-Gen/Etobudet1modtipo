using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class StarBurn : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = (int)(npc.defense * 0.93f);

            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }


            npc.lifeRegen -= 200;
        }
    }
}
