using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Biomes;

namespace Etobudet1modtipo.Common.GlobalNPCs
{
    public class MistyCaveSpawns : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.InModBiome<MistyCaveBiome>())
            {
                spawnRate = (int)(spawnRate * 3.33f);
                maxSpawns = (int)(maxSpawns * 0.3f);
            }
        }
    }
}