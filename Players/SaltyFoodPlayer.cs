using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class SaltyFoodPlayer : ModPlayer
    {
        public bool saltyFoodBonus;

        public override void ResetEffects()
        {
            saltyFoodBonus = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (!saltyFoodBonus)
                return;

            Player.GetDamage(DamageClass.Generic) += 0.03f;
            Player.GetAttackSpeed(DamageClass.Generic) += 0.03f;
            Player.GetCritChance(DamageClass.Generic) += 3f;
            Player.moveSpeed += 0.03f;
            Player.pickSpeed -= 0.03f;
        }
    }
}
