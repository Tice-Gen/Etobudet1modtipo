using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Buffs
{
    public class HealthyAniseBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {

            player.GetDamage<EndlessThrower>() += 0.20f;
        }
    }
}
