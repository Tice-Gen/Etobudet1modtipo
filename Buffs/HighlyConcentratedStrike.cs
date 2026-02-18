using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Buffs
{
    public class HighlyConcentratedStrike : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.active) return;


            npc.defense = (int)(npc.defense * 0.85f);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;


            var globalNPC = npc.GetGlobalNPC<HighlyConcentratedStrikeGlobal>();
            globalNPC.timer++;

            if (globalNPC.timer >= 30)
            {
                globalNPC.timer = 0;

                int damage = 3;
                npc.life -= damage;
                if (npc.life < 0)
                    npc.life = 0;


                CombatText.NewText(npc.Hitbox, Color.Orange, damage, dramatic: false, dot: true);
                npc.checkDead();
            }
        }
    }


    public class HighlyConcentratedStrikeGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int timer = 0;
    }
}
