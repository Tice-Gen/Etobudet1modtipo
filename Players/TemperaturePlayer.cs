using System;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Common.Temperature;
using Etobudet1modtipo.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class TemperaturePlayer : ModPlayer
    {
        private const int TileScanRadius = 6;
        private const int LavaOverheatRadiusTiles = 5;
        private const int TorchHeatRadiusTiles = 5;
        private const float SunOverheatRadiusPixels = 500f;
        private const float CampfireHeatMaxBonus = 10f;
        private const float CampfireFullHeatRadiusPixels = 56f;
        private const float TorchHeatBonus = 5f;

        private float foodTemperatureOffset;
        private int foodTemperatureTime;

        public float CurrentTemperature { get; private set; }
        public float TargetTemperature { get; private set; }
        public float ColdPenaltyStrength { get; private set; }
        public float HeatPenaltyStrength { get; private set; }
        public float DamagePerSecond { get; private set; }

        public float FillRatio
            => MathHelper.Clamp(
                (CurrentTemperature - TemperatureRegistry.DisplayMinTemperature)
                / (TemperatureRegistry.DisplayMaxTemperature - TemperatureRegistry.DisplayMinTemperature),
                0f,
                1f);

        public override void Initialize()
        {
            ResetTemperature(clearFood: true);
        }

        public override void OnEnterWorld()
        {
            ResetTemperature(clearFood: true);
        }

        public override void UpdateDead()
        {
            ResetTemperature(clearFood: true);
        }

        public override void PostUpdate()
        {
            if (!Player.active || Player.dead)
            {
                return;
            }

            UpdateFoodEffect();
            UpdateTemperatureState();
            ApplyTemperatureBuffs();
        }

        public override void UpdateBadLifeRegen()
        {
            if (DamagePerSecond <= 0f)
            {
                return;
            }

            Player.lifeRegenTime = 0;
            if (Player.lifeRegen > 0)
            {
                Player.lifeRegen = 0;
            }

            Player.lifeRegen -= (int)Math.Round(DamagePerSecond * 2f);
        }

        public void ApplyFoodTemperatureEffect(float degrees, int durationTicks)
        {
            if (durationTicks <= 0)
            {
                return;
            }

            foodTemperatureOffset = degrees;
            foodTemperatureTime = durationTicks;
        }

        public string GetStatusText()
        {
            return Language.GetTextValue(GetStatusKey());
        }

        public Color GetStatusColor()
        {
            return TemperatureRegistry.GetTemperatureColor(CurrentTemperature);
        }

        private void UpdateFoodEffect()
        {
            if (foodTemperatureTime <= 0)
            {
                foodTemperatureOffset = 0f;
                foodTemperatureTime = 0;
                return;
            }

            foodTemperatureTime--;
            if (foodTemperatureTime <= 0)
            {
                foodTemperatureOffset = 0f;
                foodTemperatureTime = 0;
            }
        }

        private void UpdateTemperatureState()
        {
            if (IsExtremeHeatSourceNearby())
            {
                TargetTemperature = TemperatureRegistry.DisplayMaxTemperature;
                CurrentTemperature = TemperatureRegistry.DisplayMaxTemperature;
                UpdateThresholdEffects();
                return;
            }

            float target = TemperatureRegistry.BaseTemperature;
            target += TemperatureRegistry.GetBiomeContribution(Player);
            target += TemperatureRegistry.GetArmorContribution(Player);
            target += TemperatureRegistry.GetAccessoryContribution(Player);
            target += foodTemperatureOffset;
            target += GetTorchContribution();
            target += GetTileContribution();
            target += GetNpcContribution();
            target += GetProjectileContribution();

            if (Player.lavaWet)
            {
                target += 16f;
            }
            else if (Player.wet)
            {
                target -= 1.75f;
            }

            if (Player.honeyWet)
            {
                target += 0.75f;
            }

            if (Player.HasBuff(BuffID.OnFire) || Player.HasBuff(BuffID.OnFire3) || Player.HasBuff(BuffID.Daybreak))
            {
                target += 3.5f;
            }

            if (Player.HasBuff(BuffID.Frostburn) || Player.HasBuff(BuffID.Frostburn2) || Player.HasBuff(BuffID.Chilled))
            {
                target -= 2.5f;
            }

            TargetTemperature = MathHelper.Clamp(
                target,
                TemperatureRegistry.DisplayMinTemperature,
                TemperatureRegistry.DisplayMaxTemperature);

            float maxTemperatureStep = 0.08f;
            if (Player.velocity.LengthSquared() > 16f)
            {
                maxTemperatureStep += 0.03f;
            }

            if (Player.wet || Player.lavaWet)
            {
                maxTemperatureStep += 0.03f;
            }

            CurrentTemperature = MoveTowards(CurrentTemperature, TargetTemperature, maxTemperatureStep);
            CurrentTemperature = MathHelper.Clamp(
                CurrentTemperature,
                TemperatureRegistry.DisplayMinTemperature,
                TemperatureRegistry.DisplayMaxTemperature);

            UpdateThresholdEffects();
        }

        private void UpdateThresholdEffects()
        {
            ColdPenaltyStrength = 0f;
            HeatPenaltyStrength = 0f;
            DamagePerSecond = 0f;

            if (CurrentTemperature < TemperatureRegistry.SafeMinTemperature)
            {
                float coldDepth = TemperatureRegistry.SafeMinTemperature - CurrentTemperature;
                ColdPenaltyStrength = MathHelper.Clamp(coldDepth / 10f, 0.08f, 2f);

                if (CurrentTemperature < TemperatureRegistry.DangerousMinTemperature)
                {
                    DamagePerSecond = MathHelper.Clamp(
                        6f + (TemperatureRegistry.DangerousMinTemperature - CurrentTemperature) * 1.4f,
                        0f,
                        28f);
                }

                return;
            }

            if (CurrentTemperature > TemperatureRegistry.SafeMaxTemperature)
            {
                float heatDepth = CurrentTemperature - TemperatureRegistry.SafeMaxTemperature;
                HeatPenaltyStrength = MathHelper.Clamp(heatDepth / 10f, 0.08f, 2f);

                if (CurrentTemperature > TemperatureRegistry.DangerousMaxTemperature)
                {
                    DamagePerSecond = MathHelper.Clamp(
                        6f + (CurrentTemperature - TemperatureRegistry.DangerousMaxTemperature) * 1.4f,
                        0f,
                        28f);
                }
            }
        }

        private void ApplyTemperatureBuffs()
        {
            if (ColdPenaltyStrength > 0f)
            {
                Player.AddBuff(ModContent.BuffType<ColdExposure>(), 2);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ColdExposure>());
            }

            if (HeatPenaltyStrength > 0f)
            {
                Player.AddBuff(ModContent.BuffType<HeatExposure>(), 2);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<HeatExposure>());
            }
        }

        private float GetTileContribution()
        {
            int playerTileX = (int)(Player.Center.X / 16f);
            int playerTileY = (int)(Player.Center.Y / 16f);
            float total = 0f;
            float campfireBonus = 0f;
            float maxDistance = TileScanRadius * 16f;

            for (int x = playerTileX - TileScanRadius; x <= playerTileX + TileScanRadius; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = playerTileY - TileScanRadius; y <= playerTileY + TileScanRadius; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    Vector2 tileCenter = new Vector2((x + 0.5f) * 16f, (y + 0.5f) * 16f);
                    float distance = Vector2.Distance(Player.Center, tileCenter);
                    if (distance > maxDistance)
                    {
                        continue;
                    }

                    float weight = 1f - distance / maxDistance;

                    if (tile.HasTile && tile.TileType == TileID.Campfire)
                    {
                        campfireBonus = Math.Max(campfireBonus, GetCampfireHeatBonus(distance, maxDistance));
                    }

                    if (tile.HasTile && TemperatureRegistry.TileTemperatureOffsets.TryGetValue(tile.TileType, out float tileOffset))
                    {
                        total += tileOffset * weight;
                    }

                    total += ElectronicsTemperatureSystem.GetSignalHeatAtTile(x, y) * weight;

                    if (tile.LiquidAmount > 0)
                    {
                        switch (tile.LiquidType)
                        {
                            case LiquidID.Lava:
                                total += 2.6f * weight;
                                break;
                            case LiquidID.Water:
                                total -= 1.1f * weight;
                                break;
                            case LiquidID.Honey:
                                total += 0.6f * weight;
                                break;
                        }
                    }
                }
            }

            return MathHelper.Clamp(total, -9f, 9f) + campfireBonus;
        }

        private float GetTorchContribution()
        {
            return IsTorchItem(Player.HeldItem) || HasNearbyTorch(TorchHeatRadiusTiles)
                ? TorchHeatBonus
                : 0f;
        }

        private float GetCampfireHeatBonus(float distance, float maxDistance)
        {
            if (distance <= CampfireFullHeatRadiusPixels)
            {
                return CampfireHeatMaxBonus;
            }

            if (distance >= maxDistance)
            {
                return 0f;
            }

            float progress = 1f - (distance - CampfireFullHeatRadiusPixels) / (maxDistance - CampfireFullHeatRadiusPixels);
            return CampfireHeatMaxBonus * MathHelper.Clamp(progress, 0f, 1f);
        }

        private bool HasNearbyTorch(int radiusTiles)
        {
            int playerTileX = (int)(Player.Center.X / 16f);
            int playerTileY = (int)(Player.Center.Y / 16f);
            float radiusPixels = radiusTiles * 16f;
            float radiusSq = radiusPixels * radiusPixels;

            for (int x = playerTileX - radiusTiles; x <= playerTileX + radiusTiles; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = playerTileY - radiusTiles; y <= playerTileY + radiusTiles; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile || !IsTorchTile(tile.TileType))
                    {
                        continue;
                    }

                    Vector2 tileCenter = new Vector2((x + 0.5f) * 16f, (y + 0.5f) * 16f);
                    if (Vector2.DistanceSquared(Player.Center, tileCenter) <= radiusSq)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsTorchItem(Item item)
        {
            if (item == null || item.IsAir)
            {
                return false;
            }

            if (item.createTile >= 0 && item.createTile < TileID.Sets.Torch.Length && TileID.Sets.Torch[item.createTile])
            {
                return true;
            }

            string vanillaIdName = ItemID.Search.GetName(item.type);
            if (!string.IsNullOrWhiteSpace(vanillaIdName)
                && vanillaIdName.Contains("Torch", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(item.Name)
                && item.Name.Contains("Torch", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsTorchTile(ushort tileType)
        {
            return tileType < TileID.Sets.Torch.Length && TileID.Sets.Torch[tileType];
        }

        private float GetNpcContribution()
        {
            float total = 0f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !TemperatureRegistry.NpcAuras.TryGetValue(npc.type, out TemperatureAura aura))
                {
                    continue;
                }

                total += GetWeightedAuraContribution(npc.Center, aura);
            }

            return MathHelper.Clamp(total, -12f, 12f);
        }

        private float GetProjectileContribution()
        {
            int coolerProjectileType = ModContent.ProjectileType<global::Etobudet1modtipo.Projectiles.Cooler>();
            float total = 0f;
            float coolerTotal = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || !TemperatureRegistry.ProjectileAuras.TryGetValue(projectile.type, out TemperatureAura aura))
                {
                    continue;
                }

                float contribution = GetWeightedAuraContribution(projectile.Center, aura);
                if (projectile.type == coolerProjectileType)
                {
                    coolerTotal += contribution;
                    continue;
                }

                total += contribution;
            }

            return MathHelper.Clamp(total, -12f, 12f) + coolerTotal;
        }

        private float GetWeightedAuraContribution(Vector2 sourceCenter, TemperatureAura aura)
        {
            float distance = Vector2.Distance(Player.Center, sourceCenter);
            if (distance >= aura.RangePixels)
            {
                return 0f;
            }

            float intensity = 1f - distance / aura.RangePixels;
            return aura.Degrees * intensity;
        }

        private string GetStatusKey()
        {
            if (CurrentTemperature < TemperatureRegistry.DangerousMinTemperature)
            {
                return "Mods.Etobudet1modtipo.UI.Temperature.Status.Freezing";
            }

            if (CurrentTemperature < TemperatureRegistry.SafeMinTemperature)
            {
                return "Mods.Etobudet1modtipo.UI.Temperature.Status.ColdWarning";
            }

            if (CurrentTemperature > TemperatureRegistry.DangerousMaxTemperature)
            {
                return "Mods.Etobudet1modtipo.UI.Temperature.Status.Overheated";
            }

            if (CurrentTemperature > TemperatureRegistry.SafeMaxTemperature)
            {
                return "Mods.Etobudet1modtipo.UI.Temperature.Status.HotWarning";
            }

            return "Mods.Etobudet1modtipo.UI.Temperature.Status.Safe";
        }

        private bool IsExtremeHeatSourceNearby()
        {
            return Player.lavaWet || HasNearbyLava(LavaOverheatRadiusTiles) || HasNearbySunProjectile();
        }

        private bool HasNearbyLava(int radiusTiles)
        {
            int playerTileX = (int)(Player.Center.X / 16f);
            int playerTileY = (int)(Player.Center.Y / 16f);
            float radiusPixels = radiusTiles * 16f;
            float radiusSq = radiusPixels * radiusPixels;

            for (int x = playerTileX - radiusTiles; x <= playerTileX + radiusTiles; x++)
            {
                if (x < 0 || x >= Main.maxTilesX)
                {
                    continue;
                }

                for (int y = playerTileY - radiusTiles; y <= playerTileY + radiusTiles; y++)
                {
                    if (y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.LiquidAmount <= 0 || tile.LiquidType != LiquidID.Lava)
                    {
                        continue;
                    }

                    Vector2 tileCenter = new Vector2((x + 0.5f) * 16f, (y + 0.5f) * 16f);
                    if (Vector2.DistanceSquared(Player.Center, tileCenter) <= radiusSq)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool HasNearbySunProjectile()
        {
            int sunProjectileType = ModContent.ProjectileType<global::Etobudet1modtipo.Projectiles.Sun>();
            float radiusSq = SunOverheatRadiusPixels * SunOverheatRadiusPixels;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.type != sunProjectileType)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(Player.Center, projectile.Center) <= radiusSq)
                {
                    return true;
                }
            }

            return false;
        }

        private float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Math.Sign(target - current) * maxDelta;
        }

        private void ResetTemperature(bool clearFood)
        {
            CurrentTemperature = TemperatureRegistry.BaseTemperature;
            TargetTemperature = TemperatureRegistry.BaseTemperature;
            ColdPenaltyStrength = 0f;
            HeatPenaltyStrength = 0f;
            DamagePerSecond = 0f;

            if (clearFood)
            {
                foodTemperatureOffset = 0f;
                foodTemperatureTime = 0;
            }
        }
    }
}
