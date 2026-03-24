using System.Collections.Generic;
using Etobudet1modtipo.Common.Temperature;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Systems
{
    public class ElectronicsTemperatureSystem : ModSystem
    {
        private const int UpdateIntervalTicks = 20;
        private const int CandidateScanRadiusTiles = 42;
        private const int ThermalTileScanRadius = 5;
        private const int ElectronicsHeatAuraRadiusTiles = 2;
        private const int FailureStressThreshold = 180;
        private const int BlastRadiusTiles = 2;
        private const int WireBlastPixelRadius = 34;
        private const float ActiveSignalHeatDegrees = 1f;
        private const float InternalHeatPerSecondNormal = 1f / 100f;
        private const float InternalHeatPerSecondCooled = 1f / 1000f;
        private const float MaxInternalHeat = 60f;
        private const int ActivationPulseTicks = 60;
        private const int MaxActivationPropagationTiles = 2000;

        private sealed class ElectronicsState
        {
            public float Temperature;
            public float InternalHeat;
            public int ActiveTimeLeft;
            public int Stress;
            public ulong LastSeenAt;
        }

        private readonly struct ActiveProjectileAura
        {
            public ActiveProjectileAura(int type, Vector2 center, TemperatureAura aura)
            {
                Type = type;
                Center = center;
                Aura = aura;
            }

            public int Type { get; }
            public Vector2 Center { get; }
            public TemperatureAura Aura { get; }
        }

        private static readonly Dictionary<Point16, ElectronicsState> States = new();
        private static readonly HashSet<ushort> ExplicitElectronicTiles = new()
        {
            (ushort)ModContent.TileType<Tiles.IndastrilCoolerTile>(),
            (ushort)ModContent.TileType<Tiles.MegaTrapTile>()
        };

        public override void OnWorldUnload()
        {
            States.Clear();
        }

        public static bool IsSignalBlocked(int i, int j)
        {
            for (int x = i - 1; x <= i + 1; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Point16 point = new Point16(x, y);
                    if (!States.TryGetValue(point, out ElectronicsState state))
                    {
                        continue;
                    }

                    if (state.Temperature < TemperatureRegistry.ElectronicsSafeMinTemperature)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static float GetSignalHeatAtTile(int i, int j)
        {
            Point16 point = new Point16(i, j);
            if (!States.TryGetValue(point, out ElectronicsState state) || state.ActiveTimeLeft <= 0)
            {
                return 0f;
            }

            Tile tile = Framing.GetTileSafely(i, j);
            return HasConnection(tile) ? ActiveSignalHeatDegrees : 0f;
        }

        public static void RegisterActivation(int i, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Queue<Point16> pending = new();
            HashSet<Point16> visited = new();

            for (int x = i - 1; x <= i + 1; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Point16 seed = new Point16(x, y);
                    if (visited.Add(seed))
                    {
                        pending.Enqueue(seed);
                    }
                }
            }

            int processed = 0;
            while (pending.Count > 0 && processed < MaxActivationPropagationTiles)
            {
                Point16 point = pending.Dequeue();
                processed++;

                Tile tile = Framing.GetTileSafely(point.X, point.Y);
                bool hasConnection = HasConnection(tile);
                bool isDevice = IsElectronicDeviceTile(tile);
                if (!hasConnection && !isDevice)
                {
                    continue;
                }

                TouchActivation(point);

                if (!hasConnection)
                {
                    continue;
                }

                EnqueueNeighbor(point.X - 1, point.Y, pending, visited);
                EnqueueNeighbor(point.X + 1, point.Y, pending, visited);
                EnqueueNeighbor(point.X, point.Y - 1, pending, visited);
                EnqueueNeighbor(point.X, point.Y + 1, pending, visited);
            }
        }

        public override void PostUpdateWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (Main.GameUpdateCount % UpdateIntervalTicks != 0)
            {
                return;
            }

            Dictionary<Point16, Player> candidates = GatherCandidateTiles();
            if (candidates.Count == 0)
            {
                States.Clear();
                return;
            }

            List<ActiveProjectileAura> projectileAuras = GatherProjectileAuras();
            Dictionary<Point16, float> internalHeatSnapshot = CaptureInternalHeatSnapshot();
            ulong tickStamp = Main.GameUpdateCount;

            foreach ((Point16 point, Player playerContext) in candidates)
            {
                Tile tile = Framing.GetTileSafely(point.X, point.Y);
                if (!IsElectronicsRelevantTile(tile))
                {
                    States.Remove(point);
                    continue;
                }

                bool cooledByCooler = IsAffectedByCooler(GetTileCenter(point), projectileAuras);
                float temperature = ComputeElectronicsTemperature(point, playerContext, projectileAuras, internalHeatSnapshot);
                UpdateElectronicsState(point, tile, temperature, cooledByCooler, tickStamp);
            }

            RemoveStaleStates(tickStamp);
        }

        private static Dictionary<Point16, Player> GatherCandidateTiles()
        {
            Dictionary<Point16, Player> result = new();

            foreach (Player player in Main.ActivePlayers)
            {
                int centerX = (int)(player.Center.X / 16f);
                int centerY = (int)(player.Center.Y / 16f);

                for (int x = centerX - CandidateScanRadiusTiles; x <= centerX + CandidateScanRadiusTiles; x++)
                {
                    if (x < 0 || x >= Main.maxTilesX)
                    {
                        continue;
                    }

                    for (int y = centerY - CandidateScanRadiusTiles; y <= centerY + CandidateScanRadiusTiles; y++)
                    {
                        if (y < 0 || y >= Main.maxTilesY)
                        {
                            continue;
                        }

                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!IsElectronicsRelevantTile(tile))
                        {
                            continue;
                        }

                        Point16 point = new Point16(x, y);
                        if (!result.TryGetValue(point, out Player existingPlayer))
                        {
                            result[point] = player;
                            continue;
                        }

                        Vector2 tileCenter = GetTileCenter(point);
                        if (Vector2.DistanceSquared(player.Center, tileCenter) < Vector2.DistanceSquared(existingPlayer.Center, tileCenter))
                        {
                            result[point] = player;
                        }
                    }
                }
            }

            return result;
        }

        private static List<ActiveProjectileAura> GatherProjectileAuras()
        {
            List<ActiveProjectileAura> result = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || !TemperatureRegistry.ProjectileAuras.TryGetValue(projectile.type, out TemperatureAura aura))
                {
                    continue;
                }

                result.Add(new ActiveProjectileAura(projectile.type, projectile.Center, aura));
            }

            return result;
        }

        private static Dictionary<Point16, float> CaptureInternalHeatSnapshot()
        {
            Dictionary<Point16, float> snapshot = new(States.Count);
            foreach ((Point16 point, ElectronicsState state) in States)
            {
                if (state.InternalHeat > 0f)
                {
                    snapshot[point] = state.InternalHeat;
                }
            }

            return snapshot;
        }

        private static float ComputeElectronicsTemperature(
            Point16 point,
            Player playerContext,
            List<ActiveProjectileAura> projectileAuras,
            Dictionary<Point16, float> internalHeatSnapshot)
        {
            Vector2 center = GetTileCenter(point);
            float total = TemperatureRegistry.BaseTemperature;
            total += TemperatureRegistry.GetBiomeContribution(playerContext);
            total += GetLocalTileContribution(center);
            total += GetLocalProjectileContribution(center, projectileAuras);
            total += GetNearbyElectronicsHeatContribution(point, internalHeatSnapshot);
            return MathHelper.Clamp(total, -30f, 70f);
        }

        private static float GetLocalTileContribution(Vector2 center)
        {
            int tileX = (int)(center.X / 16f);
            int tileY = (int)(center.Y / 16f);
            float total = 0f;
            float maxDistance = ThermalTileScanRadius * 16f;

            for (int x = tileX - ThermalTileScanRadius; x <= tileX + ThermalTileScanRadius; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = tileY - ThermalTileScanRadius; y <= tileY + ThermalTileScanRadius; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    Vector2 tileCenter = new Vector2((x + 0.5f) * 16f, (y + 0.5f) * 16f);
                    float distance = Vector2.Distance(center, tileCenter);
                    if (distance > maxDistance)
                    {
                        continue;
                    }

                    float weight = 1f - distance / maxDistance;

                    if (tile.HasTile && TemperatureRegistry.TileTemperatureOffsets.TryGetValue(tile.TileType, out float tileOffset))
                    {
                        total += tileOffset * weight;
                    }

                    total += GetSignalHeatAtTile(x, y) * weight;

                    if (tile.LiquidAmount <= 0)
                    {
                        continue;
                    }

                    switch (tile.LiquidType)
                    {
                        case LiquidID.Lava:
                            total += 2.8f * weight;
                            break;
                        case LiquidID.Water:
                            total -= 1.2f * weight;
                            break;
                        case LiquidID.Honey:
                            total += 0.6f * weight;
                            break;
                    }
                }
            }

            return MathHelper.Clamp(total, -12f, 12f);
        }

        private static float GetLocalProjectileContribution(Vector2 center, List<ActiveProjectileAura> projectileAuras)
        {
            int coolerProjectileType = ModContent.ProjectileType<Projectiles.Cooler>();
            float total = 0f;
            float coolerTotal = 0f;
            foreach (ActiveProjectileAura projectileAura in projectileAuras)
            {
                float distance = Vector2.Distance(center, projectileAura.Center);
                if (distance >= projectileAura.Aura.RangePixels)
                {
                    continue;
                }

                float weight = 1f - distance / projectileAura.Aura.RangePixels;
                float contribution = projectileAura.Aura.Degrees * weight;
                if (projectileAura.Type == coolerProjectileType)
                {
                    coolerTotal += contribution;
                    continue;
                }

                total += contribution;
            }

            return MathHelper.Clamp(total, -16f, 16f) + coolerTotal;
        }

        private static float GetNearbyElectronicsHeatContribution(Point16 point, Dictionary<Point16, float> internalHeatSnapshot)
        {
            float total = 0f;
            float maxDistance = ElectronicsHeatAuraRadiusTiles * 16f;
            Vector2 center = GetTileCenter(point);

            for (int x = point.X - ElectronicsHeatAuraRadiusTiles; x <= point.X + ElectronicsHeatAuraRadiusTiles; x++)
            {
                for (int y = point.Y - ElectronicsHeatAuraRadiusTiles; y <= point.Y + ElectronicsHeatAuraRadiusTiles; y++)
                {
                    Point16 nearbyPoint = new Point16(x, y);
                    if (!internalHeatSnapshot.TryGetValue(nearbyPoint, out float internalHeat) || internalHeat <= 0f)
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(center, GetTileCenter(nearbyPoint));
                    if (distance > maxDistance)
                    {
                        continue;
                    }

                    float weight = 1f - distance / maxDistance;
                    total += internalHeat * weight;
                }
            }

            return MathHelper.Clamp(total, 0f, MaxInternalHeat);
        }

        private static bool IsAffectedByCooler(Vector2 center, List<ActiveProjectileAura> projectileAuras)
        {
            int coolerType = ModContent.ProjectileType<Projectiles.Cooler>();
            foreach (ActiveProjectileAura projectileAura in projectileAuras)
            {
                if (projectileAura.Type != coolerType)
                {
                    continue;
                }

                if (Vector2.Distance(center, projectileAura.Center) < projectileAura.Aura.RangePixels)
                {
                    return true;
                }
            }

            return false;
        }

        private static void UpdateElectronicsState(Point16 point, Tile tile, float temperature, bool cooledByCooler, ulong tickStamp)
        {
            if (!States.TryGetValue(point, out ElectronicsState state))
            {
                state = new ElectronicsState();
                States[point] = state;
            }

            state.Temperature = temperature;
            state.LastSeenAt = tickStamp;
            UpdateInternalHeat(state, tile, cooledByCooler);

            bool isOvercooled = temperature < TemperatureRegistry.ElectronicsSafeMinTemperature;
            bool isOverheated = temperature > TemperatureRegistry.ElectronicsSafeMaxTemperature;
            if (!isOvercooled && !isOverheated)
            {
                state.Stress = Utils.Clamp(state.Stress - UpdateIntervalTicks * 2, 0, FailureStressThreshold);
                return;
            }

            if (isOvercooled)
            {
                state.Stress = Utils.Clamp(state.Stress - UpdateIntervalTicks, 0, FailureStressThreshold);
                return;
            }

            float overflow = temperature - TemperatureRegistry.ElectronicsSafeMaxTemperature;

            state.Stress += UpdateIntervalTicks + (int)(overflow * 5f);
            if (state.Stress < FailureStressThreshold)
            {
                return;
            }

            float failureChance = MathHelper.Clamp(0.08f + overflow * 0.04f + (state.Stress - FailureStressThreshold) / 420f, 0.08f, 0.85f);
            if (state.Stress >= FailureStressThreshold * 2 || Main.rand.NextFloat() < failureChance)
            {
                TriggerFailure(point);
            }
        }

        private static void UpdateInternalHeat(ElectronicsState state, Tile tile, bool cooledByCooler)
        {
            if (!HasConnection(tile))
            {
                state.ActiveTimeLeft = 0;
                return;
            }

            if (state.ActiveTimeLeft <= 0)
            {
                return;
            }

            int heatedTicks = Math.Min(UpdateIntervalTicks, state.ActiveTimeLeft);
            float heatPerSecond = cooledByCooler ? InternalHeatPerSecondCooled : InternalHeatPerSecondNormal;
            state.InternalHeat = Math.Min(state.InternalHeat + heatPerSecond * heatedTicks / 60f, MaxInternalHeat);
            state.ActiveTimeLeft = Math.Max(0, state.ActiveTimeLeft - UpdateIntervalTicks);
        }

        private static void TriggerFailure(Point16 origin)
        {
            States.Remove(origin);
            Vector2 blastCenter = GetTileCenter(origin);
            SoundEngine.PlaySound(SoundID.Item14, blastCenter);

            if (Main.netMode != NetmodeID.Server)
            {
                SpawnFailureParticles(blastCenter);
            }

            for (int x = origin.X - BlastRadiusTiles; x <= origin.X + BlastRadiusTiles; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = origin.Y - BlastRadiusTiles; y <= origin.Y + BlastRadiusTiles; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    ClearConnections(tile);

                    if (IsElectronicDeviceTile(tile))
                    {
                        WorldGen.KillTile(x, y, false, false, true);
                    }
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, BlastRadiusTiles * 2 + 3);
            }
        }

        private static void SpawnFailureParticles(Vector2 center)
        {
            for (int i = 0; i < 28; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.8f, 5.8f);
                Dust spark = Dust.NewDustPerfect(center, DustID.Electric, velocity, 80, default, Main.rand.NextFloat(1.1f, 1.6f));
                spark.noGravity = true;
            }

            for (int i = 0; i < 18; i++)
            {
                Vector2 spawnPosition = center + Main.rand.NextVector2Circular(WireBlastPixelRadius, WireBlastPixelRadius);
                Vector2 velocity = Main.rand.NextVector2Circular(1.6f, 1.6f);
                Dust smoke = Dust.NewDustPerfect(spawnPosition, DustID.Smoke, velocity, 120, default, Main.rand.NextFloat(1f, 1.8f));
                smoke.fadeIn = 1.1f;
                smoke.velocity *= 1.5f;
            }

            for (int i = 0; i < 18; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.2f, 4.2f);
                Dust frost = Dust.NewDustPerfect(center, DustID.IceTorch, velocity, 70, default, Main.rand.NextFloat(0.9f, 1.35f));
                frost.noGravity = true;
            }
        }

        private static void ClearConnections(Tile tile)
        {
            tile.RedWire = false;
            tile.BlueWire = false;
            tile.GreenWire = false;
            tile.YellowWire = false;
            tile.HasActuator = false;
        }

        private static void RemoveStaleStates(ulong tickStamp)
        {
            List<Point16> toRemove = null;
            foreach ((Point16 point, ElectronicsState state) in States)
            {
                if (state.LastSeenAt == tickStamp)
                {
                    continue;
                }

                toRemove ??= new List<Point16>();
                toRemove.Add(point);
            }

            if (toRemove == null)
            {
                return;
            }

            foreach (Point16 point in toRemove)
            {
                States.Remove(point);
            }
        }

        private static bool IsElectronicsRelevantTile(Tile tile)
        {
            if (HasConnection(tile))
            {
                return true;
            }

            return tile.HasTile && ExplicitElectronicTiles.Contains(tile.TileType);
        }

        private static bool IsElectronicDeviceTile(Tile tile)
        {
            if (!tile.HasTile)
            {
                return false;
            }

            if (ExplicitElectronicTiles.Contains(tile.TileType))
            {
                return true;
            }

            return HasConnection(tile) && (Main.tileFrameImportant[tile.TileType] || !Main.tileSolid[tile.TileType]);
        }

        private static bool HasConnection(Tile tile)
        {
            return tile.RedWire || tile.BlueWire || tile.GreenWire || tile.YellowWire || tile.HasActuator;
        }

        private static void TouchActivation(Point16 point)
        {
            if (!States.TryGetValue(point, out ElectronicsState state))
            {
                state = new ElectronicsState();
                States[point] = state;
            }

            state.ActiveTimeLeft = Math.Max(state.ActiveTimeLeft, ActivationPulseTicks);
            state.LastSeenAt = Main.GameUpdateCount;
        }

        private static void EnqueueNeighbor(int x, int y, Queue<Point16> pending, HashSet<Point16> visited)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
            {
                return;
            }

            Point16 point = new Point16(x, y);
            if (visited.Add(point))
            {
                pending.Enqueue(point);
            }
        }

        private static Vector2 GetTileCenter(Point16 point)
        {
            return new Vector2((point.X + 0.5f) * 16f, (point.Y + 0.5f) * 16f);
        }
    }
}
