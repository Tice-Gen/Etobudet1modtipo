using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class OtherWatersPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (!global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive())
            {
                return;
            }

            Player.ZoneBeach = true;
        }
    }
}
