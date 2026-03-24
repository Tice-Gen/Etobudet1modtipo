using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Global;

namespace Etobudet1modtipo.Buffs
{
    public class Pressure : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_69";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PressureGlobalNPC pressureGlobal = npc.GetGlobalNPC<PressureGlobalNPC>();
            pressureGlobal.ApplyPressureDrain(npc);
        }
    }
}
