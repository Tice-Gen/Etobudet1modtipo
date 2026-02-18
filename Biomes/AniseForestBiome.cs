using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.Biomes
{
    public class AniseForestBiome : ModBiome
    {
        const int radiusTiles = 90;
        const int requiredCount = 10;

        public override bool IsBiomeActive(Player player)
        {
            int tileX = (int)(player.Center.X / 16f);
            int tileY = (int)(player.Center.Y / 16f);
            int count = 0;

            for (int x = tileX - radiusTiles; x <= tileX + radiusTiles; x++)
            {
                if (x < 0 || x >= Main.maxTilesX) continue;
                for (int y = tileY - radiusTiles; y <= tileY + radiusTiles; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY) continue;
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == ModContent.TileType<AniseGrassTile>())
                    {
                        count++;
                        if (count >= requiredCount) return true;
                    }
                }
            }

            return false;
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/AniseForestTheme");
            
    }
}
