using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Etobudet1modtipo.Systems
{
    public class SpytheTrailShaderSystem : ModSystem
    {
        public static Asset<Texture2D> NoiseTexture { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            NoiseTexture = ModContent.Request<Texture2D>("Etobudet1modtipo/Extra/WaterNoise", AssetRequestMode.ImmediateLoad);
        }

        public override void Unload()
        {
            NoiseTexture = null;
        }
    }
}
