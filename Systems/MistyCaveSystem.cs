using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Common.Systems
{
    public class MistyCaveSystem : ModSystem
    {

        public static Vector2 MistyCaveCenter = Vector2.Zero;

        public const float BiomeRadius = 150f;


        public override void OnWorldLoad()
        {
            MistyCaveCenter = Vector2.Zero;
        }

        public override void OnWorldUnload()
        {
            MistyCaveCenter = Vector2.Zero;
        }


        public override void SaveWorldData(TagCompound tag)
        {
            if (MistyCaveCenter != Vector2.Zero)
            {
                tag["MistyCaveCenterX"] = MistyCaveCenter.X;
                tag["MistyCaveCenterY"] = MistyCaveCenter.Y;
            }
        }


        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("MistyCaveCenterX"))
            {
                float x = tag.GetFloat("MistyCaveCenterX");
                float y = tag.GetFloat("MistyCaveCenterY");
                MistyCaveCenter = new Vector2(x, y);
            }
        }
    }
}