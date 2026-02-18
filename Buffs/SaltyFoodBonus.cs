using Etobudet1modtipo.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class SaltyFoodBonus : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.WellFed}";

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SaltyFoodPlayer>().saltyFoodBonus = true;
        }
    }
}
