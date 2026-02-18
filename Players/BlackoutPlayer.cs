using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.Players
{
    public class BlackoutPlayer : ModPlayer
    {
        public bool BlackoutActive;

        public override void OnRespawn()
        {
            BlackoutActive = false;
        }

        public override void OnEnterWorld()
        {
            BlackoutActive = false;
        }

        public override void UpdateDead()
        {
            if (BlackoutActive)
            {
                BlackoutScreen.Enabled = true;
            }
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                BlackoutScreen.Enabled = BlackoutActive;
            }
        }
    }
}
