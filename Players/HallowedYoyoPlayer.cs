using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class HallowedYoyoPlayer : ModPlayer
    {
        private int auraRegenTimer;

        public override void UpdateDead()
        {
            auraRegenTimer = 0;
        }

        public override void PostUpdate()
        {
            if (auraRegenTimer <= 0)
                return;

            auraRegenTimer--;


            if (Player.lifeRegen < 16)
                Player.lifeRegen = 16;

            if (Player.lifeRegenTime < 60)
                Player.lifeRegenTime = 60;
        }

        public void ActivateAuraRegen(int ticks)
        {
            if (ticks > auraRegenTimer)
                auraRegenTimer = ticks;
        }
    }
}
