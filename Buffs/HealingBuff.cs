using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class HealingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {

            if (player.buffTime[buffIndex] % 60 == 0)
            {
                player.statLife += 6;
                player.HealEffect(6);
            }
        }
    }
}
