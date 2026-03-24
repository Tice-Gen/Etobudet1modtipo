using Terraria;
using Etobudet1modtipo.Subworlds;

namespace Etobudet1modtipo.Biomes
{
    internal static class OtherWatersDepthHelper
    {
        public static bool TryGetLogicalDepth(Player player, out int logicalDepth)
        {
            logicalDepth = 0;

            if (player == null || !player.active || !IsInOtherWatersWorld())
            {
                return false;
            }

            int tileY = (int)(player.Center.Y / 16f);
            int physicalDepth = tileY - OtherWatersSubworld.OceanSurfaceTileY;
            if (physicalDepth < 0)
            {
                return false;
            }

            logicalDepth = OtherWatersSubworld.ToLogicalDepth(physicalDepth);
            return logicalDepth <= OtherWatersSubworld.LogicalMaxOceanDepthTiles;
        }

        private static bool IsInOtherWatersWorld()
        {
            if (global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive())
            {
                return true;
            }

            return Main.maxTilesX == OtherWatersSubworld.WorldWidthTiles &&
                   Main.maxTilesY == OtherWatersSubworld.WorldHeightTiles;
        }
    }
}
