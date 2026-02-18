using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class FleshSplash : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense *= 1.25f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.05f);
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 10f;
        }
    }
}
