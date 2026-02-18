using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class NecklaceOfEternalSatietyPlayer : ModPlayer
    {
        public bool equipped;
        private bool equippedLastTick;

        public override void ResetEffects()
        {
            equipped = false;
        }

        public override void PostUpdateEquips()
        {
            if (equipped && !equippedLastTick && Player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(SoundID.Item2, Player.Center);

            equippedLastTick = equipped;
        }
    }
}
