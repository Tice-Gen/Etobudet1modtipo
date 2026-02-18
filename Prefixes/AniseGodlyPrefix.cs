using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Prefixes
{
    public class AniseGodlyPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override bool CanRoll(Item item)
        {

            return item.DamageType == ModContent.GetInstance<EndlessThrower>();
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = 1.10f;
            critBonus = 4;
            useTimeMult = 0.9f;

        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 2.0f;
        }
    }
}