using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.Players
{
    public class AshDiseasePlayer : ModPlayer
    {
        public override void ResetEffects()
        {

            LossOfConsciousness.Enabled = false;
        }
    }
}
