using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Clouds
{
    public class StarAniseCloud : ModCloud
    {
        public override float SpawnChance() => 0f;

        public override void OnSpawn(Cloud cloud)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            cloud.width = texture.Width;
            cloud.height = texture.Height;
        }
    }
}
