using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common
{
    public enum CalamityAbyssLayer : byte
    {
        None = 0,
        Layer1 = 1,
        Layer2 = 2,
        Layer3 = 3,
        Layer4 = 4
    }

    public static class CalamityAbyssHelper
    {
        private const string CalamityModName = "CalamityMod";

        private static bool resolved;
        private static bool calamityLoaded;

        private static ModBiome layer1Biome;
        private static ModBiome layer2Biome;
        private static ModBiome layer3Biome;
        private static ModBiome layer4Biome;

        public static bool IsCalamityLoaded()
        {
            ResolveIfNeeded();
            return calamityLoaded;
        }

        public static CalamityAbyssLayer GetAbyssLayer(Player player)
        {
            if (player == null)
            {
                return CalamityAbyssLayer.None;
            }

            ResolveIfNeeded();
            if (!calamityLoaded)
            {
                return CalamityAbyssLayer.None;
            }

            if (layer4Biome != null && player.InModBiome(layer4Biome))
            {
                return CalamityAbyssLayer.Layer4;
            }

            if (layer3Biome != null && player.InModBiome(layer3Biome))
            {
                return CalamityAbyssLayer.Layer3;
            }

            if (layer2Biome != null && player.InModBiome(layer2Biome))
            {
                return CalamityAbyssLayer.Layer2;
            }

            if (layer1Biome != null && player.InModBiome(layer1Biome))
            {
                return CalamityAbyssLayer.Layer1;
            }

            return CalamityAbyssLayer.None;
        }

        private static void ResolveIfNeeded()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;

            if (!ModLoader.TryGetMod(CalamityModName, out Mod calamityMod))
            {
                return;
            }

            calamityLoaded = true;
            layer1Biome = TryGetBiome(calamityMod, "AbyssLayer1Biome");
            layer2Biome = TryGetBiome(calamityMod, "AbyssLayer2Biome");
            layer3Biome = TryGetBiome(calamityMod, "AbyssLayer3Biome");
            layer4Biome = TryGetBiome(calamityMod, "AbyssLayer4Biome");
        }

        private static ModBiome TryGetBiome(Mod mod, string biomeInternalName)
        {
            try
            {
                return mod.Find<ModBiome>(biomeInternalName);
            }
            catch
            {
                return null;
            }
        }
    }
}
