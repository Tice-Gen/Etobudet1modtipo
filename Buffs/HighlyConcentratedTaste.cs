using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Etobudet1modtipo.Buffs
{
    public class HighlyConcentratedTaste : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = (int)(npc.defense * 0.9f);
            npc.damage = (int)(npc.damage * 0.95f);

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;


            npc.lifeRegen -= 16;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) -= 0.05f;
            player.statDefense *= 0.9f;

            if (player.lifeRegen > 0)
                player.lifeRegen = 0;


            player.lifeRegen -= 16;
        }
    }
}
