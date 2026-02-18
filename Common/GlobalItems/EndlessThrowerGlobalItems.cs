using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class EndlessThrowerGlobalItems : GlobalItem
    {
        public override void SetDefaults(Item item)
        {

            if (item.type == ItemID.VampireKnives
                || item.type == ItemID.ScourgeoftheCorruptor
                || item.type == ItemID.DayBreak
                || item.type == ItemID.ShadowFlameKnife)
            {
                item.DamageType = ModContent.GetInstance<EndlessThrower>();
            }
        }
    }
}