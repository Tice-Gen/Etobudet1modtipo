using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Biomes
{
    public class DeepRockyWatersOfOtherWaters : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicID.Ocean;
        public override string BackgroundPath => "Etobudet1modtipo/Backgrounds/DeepRockyWatersOfOtherWaters";

        public override bool IsBiomeActive(Player player)
        {
            if (!OtherWatersDepthHelper.TryGetLogicalDepth(player, out int logicalDepth))
            {
                return false;
            }

            return logicalDepth >= 101 && logicalDepth <= 1000;
        }
    }
}
