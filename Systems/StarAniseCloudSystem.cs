using System.Collections.Generic;
using Etobudet1modtipo.Clouds;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Systems
{
    public class StarAniseCloudSystem : ModSystem
    {
        private const int CloudCheckInterval = 60;
        private int cloudCheckTimer;

        public override void OnWorldLoad()
        {
            cloudCheckTimer = 0;
        }

        public override void ClearWorld()
        {
            cloudCheckTimer = 0;
        }

        public override void PostUpdateWorld()
        {
            if (Main.dedServ || Main.gameMenu)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            cloudCheckTimer++;
            if (cloudCheckTimer < CloudCheckInterval)
                return;

            cloudCheckTimer = 0;

            if (!player.ZoneOverworldHeight && !player.ZoneSkyHeight)
                return;

            SpawnStarAniseCloud();
        }

        private static void SpawnStarAniseCloud()
        {
            HashSet<Cloud> existingClouds = [];
            bool[] wasActive = new bool[Main.maxClouds];
            int previousCloudCount = Main.numClouds;

            for (int i = 0; i < Main.maxClouds; i++)
            {
                Cloud existingCloud = Main.cloud[i];
                if (existingCloud != null)
                    existingClouds.Add(existingCloud);

                wasActive[i] = existingCloud?.active ?? false;
            }

            Cloud.addCloud();

            int slot = FindSpawnedCloudSlot(existingClouds, wasActive, previousCloudCount);
            if (slot < 0)
                return;

            Cloud cloud = Main.cloud[slot];
            if (cloud == null || !cloud.active)
                return;

            cloud.kill = false;
            cloud.type = ModContent.CloudType<StarAniseCloud>();
            cloud.ModCloud?.OnSpawn(cloud);
        }

        private static int FindSpawnedCloudSlot(HashSet<Cloud> existingClouds, bool[] wasActive, int previousCloudCount)
        {
            for (int i = 0; i < Main.maxClouds; i++)
            {
                Cloud cloud = Main.cloud[i];
                if (cloud != null && cloud.active && !existingClouds.Contains(cloud))
                    return i;
            }

            for (int i = 0; i < Main.maxClouds; i++)
            {
                Cloud cloud = Main.cloud[i];
                if (cloud != null && cloud.active && !wasActive[i])
                    return i;
            }

            for (int i = previousCloudCount; i < Main.numClouds && i < Main.maxClouds; i++)
            {
                Cloud cloud = Main.cloud[i];
                if (cloud != null && cloud.active)
                    return i;
            }

            return -1;
        }
    }
}
