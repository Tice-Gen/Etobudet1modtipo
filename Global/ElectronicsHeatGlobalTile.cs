using Terraria.ModLoader;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.Global
{
    public class ElectronicsHeatGlobalTile : GlobalTile
    {
        public override bool PreHitWire(int i, int j, int type)
        {
            return !ElectronicsTemperatureSystem.IsSignalBlocked(i, j);
        }

        public override void HitWire(int i, int j, int type)
        {
            ElectronicsTemperatureSystem.RegisterActivation(i, j);
        }
    }
}
