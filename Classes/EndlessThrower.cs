using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Classes
{
    public class EndlessThrower : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<EndlessThrower>() += 4;
        }

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            return true;
        }


        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return StatInheritanceData.None;
        }


        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Ranged)
                return true;




            return false;
        }


        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return false;
        }

        public override bool UseStandardCritCalcs => true;
    }
}
