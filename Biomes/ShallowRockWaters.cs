using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Biomes
{
    public class ShallowRockWaters : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicID.OceanNight;
        public override string BackgroundPath => "Etobudet1modtipo/Backgrounds/ShallowRockWaters";

        public override bool IsBiomeActive(Player player)
        {
            return OtherWatersDepthHelper.TryGetLogicalDepth(player, out int logicalDepth) && logicalDepth <= 100;
        }
    }
}
