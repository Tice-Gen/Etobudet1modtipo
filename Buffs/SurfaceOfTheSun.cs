using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Buffs
{
    public class SurfaceOfTheSun : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

            var globalNPC = npc.GetGlobalNPC<SurfaceOfTheSunGlobal>();
            globalNPC.timer++;

            if (globalNPC.timer >= 1) 
            {
                globalNPC.timer = 0;

                int damage = 5000;
                npc.life -= damage;
                if (npc.life < 0)
                    npc.life = 0;


                CombatText.NewText(npc.Hitbox, Color.Orange, damage, dramatic: false, dot: true);
                if (npc.life <= 0)
                {
                    Sun.TrySpawnSunDeathEffects(npc);
                }
                npc.checkDead();
            }
        }
    }


    public class SurfaceOfTheSunGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int timer = 0;
    }
}
