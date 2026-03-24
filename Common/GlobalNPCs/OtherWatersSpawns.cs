using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;
using Etobudet1modtipo.Subworlds;
using Etobudet1modtipo.Tiles;

namespace Etobudet1modtipo.Common.GlobalNPCs
{
    public class OtherWatersSpawns : GlobalNPC
    {
        private const float FishMobClusterAvoidRadius = 28f * 16f;
        private const int UrchinSoftCapInOtherWaters = 10;
        private const float UrchinClusterAvoidRadius = 22f * 16f;
        private const float UrchinSpawnWeight = 0.22f;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive())
            {
                return;
            }

            // Full override for OtherWaters: no vanilla mobs.
            pool.Clear();

            int physicalDepth = spawnInfo.SpawnTileY - OtherWatersSubworld.OceanSurfaceTileY;
            if (physicalDepth < 0)
            {
                return;
            }

            int logicalDepth = OtherWatersSubworld.ToLogicalDepth(physicalDepth);
            bool onDeepSeaFloor = IsOnDeepSeaStoneFloor(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY);
            int fishMobType = ModContent.NPCType<FishMob>();
            int fishMobCount = NPC.CountNPCS(fishMobType);
            bool fishNearby = IsFishMobNearby(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY, FishMobClusterAvoidRadius);
            bool fishMobSpawnWindow = Main.GameUpdateCount % 60 == 0;
            bool fishMobRollSuccess = Main.rand.NextBool(100);
            int urchinType = ModContent.NPCType<DeepSeaUrchin>();
            int urchinCount = NPC.CountNPCS(urchinType);
            bool urchinNearby = IsNpcTypeNearby(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY, UrchinClusterAvoidRadius, urchinType);

            if (!spawnInfo.Water)
            {
                return;
            }

            if (logicalDepth <= 1000)
            {
                if (fishMobCount == 0 && !fishNearby && fishMobSpawnWindow && fishMobRollSuccess)
                {
                    pool[fishMobType] = 1f;
                }
            }
            else if (logicalDepth <= OtherWatersSubworld.LogicalMaxOceanDepthTiles)
            {
                if (fishMobCount == 0 && !fishNearby && fishMobSpawnWindow && fishMobRollSuccess)
                {
                    pool[fishMobType] = 1f;
                }

                if (onDeepSeaFloor && urchinCount < UrchinSoftCapInOtherWaters && !urchinNearby)
                {
                    pool[urchinType] = UrchinSpawnWeight;
                }
            }
        }

        private static bool IsOnDeepSeaStoneFloor(int tileX, int tileY)
        {
            int deepSeaStoneTileType = ModContent.TileType<DeepSeaStoneTile>();

            for (int y = tileY + 1; y <= tileY + 4; y++)
            {
                if (!WorldGen.InWorld(tileX, y, 1))
                {
                    continue;
                }

                Tile tile = Framing.GetTileSafely(tileX, y);
                if (tile.HasTile && tile.TileType == deepSeaStoneTileType)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFishMobNearby(int spawnTileX, int spawnTileY, float radiusPixels)
        {
            int fishMobType = ModContent.NPCType<FishMob>();
            Vector2 spawnWorld = new Vector2(spawnTileX * 16f + 8f, spawnTileY * 16f + 8f);
            float radiusSq = radiusPixels * radiusPixels;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.type != fishMobType)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(npc.Center, spawnWorld) <= radiusSq)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNpcTypeNearby(int spawnTileX, int spawnTileY, float radiusPixels, int npcType)
        {
            Vector2 spawnWorld = new Vector2(spawnTileX * 16f + 8f, spawnTileY * 16f + 8f);
            float radiusSq = radiusPixels * radiusPixels;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.type != npcType)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(npc.Center, spawnWorld) <= radiusSq)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
