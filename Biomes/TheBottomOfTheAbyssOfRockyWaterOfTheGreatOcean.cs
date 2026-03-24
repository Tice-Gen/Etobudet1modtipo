using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Subworlds;

namespace Etobudet1modtipo.Biomes
{
    public class TheBottomOfTheAbyssOfRockyWaterOfTheGreatOcean : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicID.Space;
        public override string BackgroundPath => "Etobudet1modtipo/Backgrounds/TheBottomOfTheAbyssOfRockyWaterOfTheGreatOcean";

        public override bool IsBiomeActive(Player player)
        {
            if (!OtherWatersDepthHelper.TryGetLogicalDepth(player, out int logicalDepth))
            {
                return false;
            }

            return logicalDepth >= 1001 && logicalDepth <= OtherWatersSubworld.LogicalMaxOceanDepthTiles;
        }
    }
}
