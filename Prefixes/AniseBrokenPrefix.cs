using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Prefixes
{
    public class AniseBrokenPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override bool CanRoll(Item item)
        {

            return item.DamageType == ModContent.GetInstance<EndlessThrower>();
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = 0.9f;
            useTimeMult = 1.1f;
            critBonus = -10;
            knockbackMult = 1.1f;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 0.1f;
        }
    }
}