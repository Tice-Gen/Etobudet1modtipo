using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.Common.Systems
{

    public class MistyCaveClientSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {

            if (Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            bool inBiome = false;


            if (MistyCaveSystem.MistyCaveCenter != Vector2.Zero && player != null && player.active)
            {
                float dist = Vector2.Distance(player.Center, MistyCaveSystem.MistyCaveCenter * 16f);
                inBiome = dist < (MistyCaveSystem.BiomeRadius * 16f);
            }


            LossOfConsciousness.Enabled = inBiome;
        }
    }
}
