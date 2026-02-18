using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class TameAngryAnisePlayer : ModPlayer
    {
        public bool aniseActive;
        private int healTimer;

        public override void ResetEffects()
        {
            aniseActive = false;
        }

        public override void PostUpdate()
        {
            if (!aniseActive)
            {
                healTimer = 0;
                return;
            }

            healTimer++;


            if (healTimer >= 600)
            {
                healTimer = 0;

                if (Player.statLife < Player.statLifeMax2)
                {
                    Player.statLife += 1;
                    Player.HealEffect(1);
                }
            }
        }
    }
}
