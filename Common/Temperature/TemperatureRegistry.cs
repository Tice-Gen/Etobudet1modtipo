using System;
using System.Collections.Generic;
using Etobudet1modtipo.Biomes;
using Etobudet1modtipo.Common.GlobalItems;
using Etobudet1modtipo.items;
using Etobudet1modtipo.NPCs;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.Temperature
{
    public readonly struct TemperatureAura
    {
        public TemperatureAura(float degrees, float rangePixels)
        {
            Degrees = degrees;
            RangePixels = rangePixels;
        }

        public float Degrees { get; }
        public float RangePixels { get; }
    }

    public readonly struct TemperatureConsumableEffect
    {
        public TemperatureConsumableEffect(float degrees, int durationTicks)
        {
            Degrees = degrees;
            DurationTicks = durationTicks;
        }

        public float Degrees { get; }
        public int DurationTicks { get; }
    }

    public static class TemperatureRegistry
    {
        public const float BaseTemperature = 18f;
        public const float DisplayMinTemperature = -20f;
        public const float DisplayMaxTemperature = 50f;
        public const float SafeMinTemperature = 5f;
        public const float SafeMaxTemperature = 30f;
        public const float DangerousMinTemperature = 0f;
        public const float DangerousMaxTemperature = 35f;
        public const float ElectronicsSafeMinTemperature = -10f;
        public const float ElectronicsSafeMaxTemperature = 40f;

        private static readonly Dictionary<int, float> ArmorTemperatureOffsets = new()
        {
            [ModContent.ItemType<AniseForestHelmet>()] = 0.8f,
            [ModContent.ItemType<AniseForestBreastplate>()] = 1.2f,
            [ModContent.ItemType<AniseForestLeggings>()] = 0.8f,
            [ModContent.ItemType<AniseHead>()] = 8f,
            [ModContent.ItemType<CalamityHelmet>()] = 1.5f,
            [ModContent.ItemType<CalamityBreastplate>()] = 2f,
            [ModContent.ItemType<CalamityLeggings>()] = 1.5f,
            [ModContent.ItemType<CobaltMask>()] = -1.5f,
            [ModContent.ItemType<DeepSeaHelmet>()] = -3f,
            [ModContent.ItemType<DeepSeaBreastplate>()] = -4f,
            [ModContent.ItemType<DeepSeaLeggings>()] = -3f,
            [ModContent.ItemType<FogStoneHelmet>()] = -2f,
            [ModContent.ItemType<FogStoneBreastplate>()] = -2.75f,
            [ModContent.ItemType<FogStoneLeggings>()] = -2f,
            [ModContent.ItemType<FloweringChlorophyteHelmet>()] = 0.5f,
            [ModContent.ItemType<FloweringChlorophyteBreastplate>()] = 0.8f,
            [ModContent.ItemType<FloweringChlorophyteLeggings>()] = 0.5f,
            [ModContent.ItemType<MythrilHelmet>()] = 0.4f,
            [ModContent.ItemType<OrichalcumFossilHelmet>()] = 0.8f,
            [ModContent.ItemType<PalladiumHelmet>()] = 1.2f,
            [ModContent.ItemType<AdamantiteHelmet>()] = 2.1f,
            [ModContent.ItemType<TitaniumMask>()] = 1.3f,
            [ModContent.ItemType<UshankaHat>()] = 8f,
            [ItemID.CobaltBreastplate] = -1.6f,
            [ItemID.CobaltLeggings] = -1.4f,
            [ItemID.PalladiumBreastplate] = 1.4f,
            [ItemID.PalladiumLeggings] = 1.2f,
            [ItemID.MythrilChainmail] = 0.5f,
            [ItemID.MythrilGreaves] = 0.3f,
            [ItemID.OrichalcumBreastplate] = 0.9f,
            [ItemID.OrichalcumLeggings] = 0.8f,
            [ItemID.AdamantiteBreastplate] = 2.6f,
            [ItemID.AdamantiteLeggings] = 2.2f,
            [ItemID.TitaniumBreastplate] = 1.4f,
            [ItemID.TitaniumLeggings] = 1.2f,
            [ItemID.EskimoHood] = 2f,
            [ItemID.EskimoCoat] = 2.5f,
            [ItemID.EskimoPants] = 2f,
            [ItemID.SnowHat] = 3f,
            [ItemID.PinkEskimoHood] = 2f,
            [ItemID.PinkEskimoCoat] = 2.5f,
            [ItemID.PinkEskimoPants] = 2f,
            [ItemID.SolarFlareHelmet] = 3.5f,
            [ItemID.SolarFlareBreastplate] = 4.5f,
            [ItemID.SolarFlareLeggings] = 3.5f,
            [ItemID.MoltenHelmet] = 2.5f,
            [ItemID.MoltenBreastplate] = 3.5f,
            [ItemID.MoltenGreaves] = 2.5f,
            [ItemID.ObsidianHelm] = 1.5f,
            [ItemID.ObsidianShirt] = 2f,
            [ItemID.ObsidianPants] = 1.5f,
            [ItemID.FrostHelmet] = -1.75f,
            [ItemID.FrostBreastplate] = -2.25f,
            [ItemID.FrostLeggings] = -1.75f
        };

        private static readonly Dictionary<int, float> AccessoryTemperatureOffsets = new()
        {
            [ModContent.ItemType<BandOfWarm>()] = 5f,
            [ModContent.ItemType<BandOfCold>()] = -5f,
            [ItemID.HandWarmer] = 2f,
            [ItemID.FlameWings] = 2f,
            [ItemID.WingsSolar] = 2f,
            [ItemID.MagmaStone] = 2f,
            [ItemID.FireGauntlet] = 2f
        };

        private static readonly Dictionary<int, TemperatureConsumableEffect> ConsumableEffects = new()
        {
            [ItemID.BottledWater] = new TemperatureConsumableEffect(-6f, 60 * 90),
            [ModContent.ItemType<AniseSoda>()] = new TemperatureConsumableEffect(-4f, 60 * 120),
            [ModContent.ItemType<HighlyConcentratedAniseSoda>()] = new TemperatureConsumableEffect(-3f, 60 * 150),
            [ModContent.ItemType<SaltWater>()] = new TemperatureConsumableEffect(2f, 60 * 180),
            [ItemID.BowlofSoup] = new TemperatureConsumableEffect(3f, 60 * 180),
            [ItemID.Pho] = new TemperatureConsumableEffect(4f, 60 * 180),
            [ItemID.BunnyStew] = new TemperatureConsumableEffect(3f, 60 * 180)
        };

        private static readonly int[] FoodBuffTypes =
        {
            BuffID.WellFed,
            BuffID.WellFed2,
            BuffID.WellFed3
        };

        public static readonly Dictionary<int, TemperatureAura> ProjectileAuras = new()
        {
            [ModContent.ProjectileType<FanCooler>()] = new TemperatureAura(-5f, 180f),
            [ModContent.ProjectileType<Cooler>()] = new TemperatureAura(-10f, 240f),
            [ModContent.ProjectileType<UnquenchableLava>()] = new TemperatureAura(12f, 200f),
            [ModContent.ProjectileType<Sun>()] = new TemperatureAura(18f, 240f),
            [ModContent.ProjectileType<CosmolFire>()] = new TemperatureAura(10f, 180f),
            [ModContent.ProjectileType<GlacierWave>()] = new TemperatureAura(-10f, 190f),
            [ModContent.ProjectileType<DeepSeaPressure>()] = new TemperatureAura(-6f, 160f),
            [ModContent.ProjectileType<DeepSeaPressureParticle>()] = new TemperatureAura(-4f, 110f),
            [ProjectileID.Flames] = new TemperatureAura(7f, 120f),
            [ProjectileID.Fireball] = new TemperatureAura(9f, 150f),
            [ProjectileID.FrostBoltStaff] = new TemperatureAura(-6f, 140f)
        };

        public static readonly Dictionary<int, TemperatureAura> NpcAuras = new()
        {
            [ModContent.NPCType<DeepSeaUrchin>()] = new TemperatureAura(-6f, 170f),
            [ModContent.NPCType<JellyFish>()] = new TemperatureAura(-4f, 180f),
            [ModContent.NPCType<Astraclysm>()] = new TemperatureAura(10f, 260f),
            [NPCID.FireImp] = new TemperatureAura(7f, 180f),
            [NPCID.LavaSlime] = new TemperatureAura(5f, 160f),
            [NPCID.RedDevil] = new TemperatureAura(12f, 240f),
            [NPCID.WallofFlesh] = new TemperatureAura(15f, 280f),
            [NPCID.IceBat] = new TemperatureAura(-5f, 160f),
            [NPCID.IceGolem] = new TemperatureAura(-10f, 220f),
            [NPCID.IcyMerman] = new TemperatureAura(-6f, 180f)
        };

        public static readonly Dictionary<ushort, float> TileTemperatureOffsets = new()
        {
            [(ushort)ModContent.TileType<ScorchedEarthT>()] = 2.2f,
            [(ushort)ModContent.TileType<DeepSeaStoneTile>()] = -1.7f,
            [(ushort)ModContent.TileType<SeaSaltTile>()] = -0.9f,
            [TileID.Fireplace] = 2.6f,
            [TileID.Furnaces] = 2.2f,
            [TileID.Hellforge] = 2.8f,
            [TileID.Hellstone] = 1.2f,
            [TileID.IceBlock] = -1.2f,
            [TileID.BreakableIce] = -1f,
            [TileID.SnowBlock] = -0.8f
        };

        public static float GetArmorContribution(Player player)
        {
            float total = 0f;
            for (int slot = 0; slot < 3; slot++)
            {
                Item equipped = player.armor[slot];
                total += GetArmorTemperatureOffset(equipped);
            }

            total += GetArmorSetContribution(player);
            return MathHelper.Clamp(total, -18f, 18f);
        }

        public static float GetAccessoryContribution(Player player)
        {
            float total = 0f;
            for (int slot = 3; slot <= 9 && slot < player.armor.Length; slot++)
            {
                total += GetAccessoryTemperatureOffset(player.armor[slot]);
            }

            return MathHelper.Clamp(total, -14f, 14f);
        }

        public static bool TryGetArmorTemperatureOffset(Item item, out float offset)
        {
            offset = GetArmorTemperatureOffset(item);
            return offset != 0f;
        }

        public static bool TryGetAccessoryTemperatureOffset(Item item, out float offset)
        {
            offset = GetAccessoryTemperatureOffset(item);
            return offset != 0f;
        }

        public static float GetBiomeContribution(Player player)
        {
            float ambientTemperature = GetAmbientBiomeTemperature(player);
            ambientTemperature += GetBiomeWeatherModifier(player);
            return MathHelper.Clamp(ambientTemperature - BaseTemperature, -26f, 26f);
        }

        public static float GetAmbientBiomeTemperature(Player player)
        {
            if (player.InModBiome<TheBottomOfTheAbyssOfRockyWaterOfTheGreatOcean>())
            {
                return -6f;
            }

            if (player.InModBiome<DeepRockyWatersOfOtherWaters>())
            {
                return 3f;
            }

            if (player.InModBiome<ShallowRockWaters>())
            {
                return 10f;
            }

            if (player.InModBiome<MistyCaveBiome>())
            {
                return 9f;
            }

            if (player.InModBiome<AniseForestBiome>())
            {
                return 19f;
            }

            if (player.ZoneUnderworldHeight)
            {
                return 42f;
            }

            if (player.ZoneUndergroundDesert)
            {
                return 36f;
            }

            if (player.ZoneDesert)
            {
                return 33f;
            }

            if (player.ZoneSnow)
            {
                return 2f;
            }

            if (player.ZoneGlowshroom)
            {
                return 20f;
            }

            if (player.ZoneJungle)
            {
                return player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight ? 25f : 24f;
            }

            if (player.ZoneBeach)
            {
                return 17f;
            }

            if (player.ZoneSkyHeight)
            {
                return 8f;
            }

            if (player.ZoneDungeon)
            {
                return 12f;
            }

            if (player.ZoneMeteor)
            {
                return 32f;
            }

            if (player.ZoneCorrupt || player.ZoneCrimson)
            {
                return 16f;
            }

            if (player.ZoneHallow)
            {
                return 18f;
            }

            if (player.ZoneRockLayerHeight)
            {
                return 14f;
            }

            if (player.ZoneDirtLayerHeight)
            {
                return 16f;
            }

            return 18f;
        }

        private static float GetBiomeWeatherModifier(Player player)
        {
            float total = 0f;

            total += GetSurfaceDaylightModifier(player);
            total += GetWindChillModifier(player);

            if (Main.raining && !player.ZoneUnderworldHeight)
            {
                total -= 1.5f;
            }

            if (player.ZoneSnow && Main.dayTime)
            {
                total -= 1f;
            }

            return total;
        }

        private static float GetSurfaceDaylightModifier(Player player)
        {
            if (!player.ZoneOverworldHeight || !Main.dayTime)
            {
                return 0f;
            }

            float dayProgress = (float)(Main.time / Main.dayLength);
            float middayStrength = 1f - Math.Abs(dayProgress - 0.5f) / 0.5f;
            return MathHelper.Lerp(2f, 7f, MathHelper.Clamp(middayStrength, 0f, 1f));
        }

        private static float GetWindChillModifier(Player player)
        {
            if (!player.ZoneOverworldHeight && !player.ZoneSkyHeight)
            {
                return 0f;
            }

            float windStrength = MathHelper.Clamp(Math.Abs(Main.windSpeedCurrent), 0f, 1f);
            return -5f * windStrength;
        }

        private static float GetArmorSetContribution(Player player)
        {
            float total = 0f;
            bool matchedExplicitSet = false;

            if (HasArmorSet(
                player,
                ModContent.ItemType<DeepSeaHelmet>(),
                ModContent.ItemType<DeepSeaBreastplate>(),
                ModContent.ItemType<DeepSeaLeggings>()))
            {
                total -= 5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(
                player,
                ModContent.ItemType<FogStoneHelmet>(),
                ModContent.ItemType<FogStoneBreastplate>(),
                ModContent.ItemType<FogStoneLeggings>()))
            {
                total -= 3f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(
                player,
                ModContent.ItemType<AniseForestHelmet>(),
                ModContent.ItemType<AniseForestBreastplate>(),
                ModContent.ItemType<AniseForestLeggings>()))
            {
                total += 1.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(
                player,
                ModContent.ItemType<CalamityHelmet>(),
                ModContent.ItemType<CalamityBreastplate>(),
                ModContent.ItemType<CalamityLeggings>()))
            {
                total += 2.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(
                player,
                ModContent.ItemType<FloweringChlorophyteHelmet>(),
                ModContent.ItemType<FloweringChlorophyteBreastplate>(),
                ModContent.ItemType<FloweringChlorophyteLeggings>()))
            {
                total += 1f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<CobaltMask>(), ItemID.CobaltBreastplate, ItemID.CobaltLeggings))
            {
                total -= 1.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<PalladiumHelmet>(), ItemID.PalladiumBreastplate, ItemID.PalladiumLeggings))
            {
                total += 1.25f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<MythrilHelmet>(), ItemID.MythrilChainmail, ItemID.MythrilGreaves))
            {
                total += 0.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<OrichalcumFossilHelmet>(), ItemID.OrichalcumBreastplate, ItemID.OrichalcumLeggings))
            {
                total += 1f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<AdamantiteHelmet>(), ItemID.AdamantiteBreastplate, ItemID.AdamantiteLeggings))
            {
                total += 2.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ModContent.ItemType<TitaniumMask>(), ItemID.TitaniumBreastplate, ItemID.TitaniumLeggings))
            {
                total += 1.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ItemID.EskimoHood, ItemID.EskimoCoat, ItemID.EskimoPants))
            {
                total += 3f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ItemID.PinkEskimoHood, ItemID.PinkEskimoCoat, ItemID.PinkEskimoPants))
            {
                total += 3f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ItemID.SolarFlareHelmet, ItemID.SolarFlareBreastplate, ItemID.SolarFlareLeggings))
            {
                total += 4.5f;
                matchedExplicitSet = true;
            }

            if (HasArmorSet(player, ItemID.MoltenHelmet, ItemID.MoltenBreastplate, ItemID.MoltenGreaves))
            {
                total += 4.5f;
                matchedExplicitSet = true;
            }

            if (!matchedExplicitSet)
            {
                total += GetGenericArmorSetContribution(player);
            }

            return total;
        }

        public static bool TryGetConsumableEffect(Item item, out TemperatureConsumableEffect effect)
        {
            float degrees = 0f;
            int durationTicks = 0;
            bool hasEffect = false;

            if (ConsumableEffects.TryGetValue(item.type, out TemperatureConsumableEffect explicitEffect))
            {
                degrees = explicitEffect.Degrees;
                durationTicks = explicitEffect.DurationTicks;
                hasEffect = true;
            }
            else if (TryGetFallbackConsumableEffect(item, out TemperatureConsumableEffect fallbackEffect))
            {
                degrees = fallbackEffect.Degrees;
                durationTicks = fallbackEffect.DurationTicks;
                hasEffect = true;
            }

            if (item.TryGetGlobalItem(out SaltyFoodGlobalItem saltyFood) && saltyFood.isSaltyFood)
            {
                degrees += 2f;
                durationTicks = Math.Max(durationTicks, GetConsumableEffectDuration(item));
                hasEffect = true;
            }

            if (!hasEffect || Math.Abs(degrees) < 0.01f)
            {
                effect = default;
                return false;
            }

            effect = new TemperatureConsumableEffect(
                MathHelper.Clamp(degrees, -8f, 8f),
                durationTicks > 0 ? durationTicks : GetConsumableEffectDuration(item));

            return true;
        }

        private static bool TryGetFallbackConsumableEffect(Item item, out TemperatureConsumableEffect effect)
        {
            if (!IsTemperatureConsumable(item))
            {
                effect = default;
                return false;
            }

            float degrees = GetGenericFoodTemperatureOffset(item);
            if (Math.Abs(degrees) < 0.01f)
            {
                effect = default;
                return false;
            }

            effect = new TemperatureConsumableEffect(degrees, GetConsumableEffectDuration(item));
            return true;
        }

        private static bool IsTemperatureConsumable(Item item)
        {
            if (item == null || item.IsAir || !item.consumable)
            {
                return false;
            }

            bool isFoodSetItem = item.type >= 0
                && item.type < ItemID.Sets.IsFood.Length
                && ItemID.Sets.IsFood[item.type];

            return isFoodSetItem || Array.IndexOf(FoodBuffTypes, item.buffType) >= 0;
        }

        private static float GetGenericFoodTemperatureOffset(Item item)
        {
            string lookupName = GetConsumableLookupName(item);
            float baseStrength = item.buffType switch
            {
                BuffID.WellFed3 => 1.5f,
                BuffID.WellFed2 => 1.15f,
                BuffID.WellFed => 0.85f,
                _ => 0.7f
            };

            float total = baseStrength;

            if (ContainsAny(
                lookupName,
                "IceCream", "Ice", "Frozen", "Gelato", "Sherbet", "Sorbet", "Sundae", "Milkshake", "Smoothie", "Slush", "Iced", "Soda"))
            {
                total -= 2.25f;
            }
            else if (ContainsAny(
                lookupName,
                "Soup", "Stew", "Pho", "Spaghetti", "Pasta", "BBQ", "Ribs", "Steak", "Burger", "Pizza", "Cooked", "Grilled", "Roasted", "Fried", "Baked", "Pie", "Coffee", "Tea", "Chocolate", "Cocoa"))
            {
                total += 1.5f;
            }

            if (ContainsAny(
                lookupName,
                "Seafood", "Fish", "Shrimp", "Bird", "Duck", "Bunny", "Oyster", "Egg", "Hotdog", "Bacon", "Ham", "Sausage", "Chicken", "Turkey"))
            {
                total += 0.7f;
            }

            if (ContainsAny(
                lookupName,
                "Salad", "Fruit", "Apple", "Apricot", "Banana", "Cherry", "Coconut", "Grape", "Lemon", "Lime", "Mango", "Orange", "Peach", "Pear", "Pineapple", "Plum", "Berry", "Melon"))
            {
                total -= 0.9f;
            }

            if (ContainsAny(lookupName, "Water", "Juice", "Lemonade"))
            {
                total -= 1.25f;
            }

            if (ContainsAny(lookupName, "Ale", "Sake", "Wine", "Beer", "Rum", "Whiskey", "Brandy"))
            {
                total += 0.5f;
            }

            if (ContainsAny(lookupName, "Cold", "Chilled"))
            {
                total -= 0.75f;
            }

            if (ContainsAny(lookupName, "Hot", "Spicy", "Inferno", "Hellfire", "Lava"))
            {
                total += 1f;
            }

            return MathHelper.Clamp(total, -4.5f, 4.5f);
        }

        private static int GetConsumableEffectDuration(Item item)
        {
            int durationTicks = item.buffTime > 0 ? item.buffTime : 60 * 150;
            string lookupName = GetConsumableLookupName(item);

            if (ContainsAny(lookupName, "Water", "Soda", "Juice", "Coffee", "Tea", "Ale", "Sake", "Wine", "Beer", "Milkshake", "Smoothie"))
            {
                durationTicks = (int)(durationTicks * 0.7f);
            }
            else if (ContainsAny(lookupName, "Soup", "Stew", "Pho", "Dinner", "Steak", "Ribs", "Pizza", "Spaghetti", "Burger", "Pie"))
            {
                durationTicks = (int)(durationTicks * 0.85f);
            }

            return Math.Clamp(durationTicks, 60 * 75, 60 * 360);
        }

        private static string GetConsumableLookupName(Item item)
        {
            string vanillaIdName = ItemID.Search.GetName(item.type);
            if (!string.IsNullOrWhiteSpace(vanillaIdName))
            {
                return vanillaIdName;
            }

            if (item.ModItem != null && !string.IsNullOrWhiteSpace(item.ModItem.Name))
            {
                return item.ModItem.Name;
            }

            return item.Name?.Replace(" ", string.Empty) ?? string.Empty;
        }

        public static Color GetTemperatureColor(float temperature)
        {
            float clamped = MathHelper.Clamp(temperature, DisplayMinTemperature, DisplayMaxTemperature);

            if (clamped < SafeMinTemperature)
            {
                float progress = (clamped - DisplayMinTemperature) / (SafeMinTemperature - DisplayMinTemperature);
                return Color.Lerp(new Color(68, 124, 255), new Color(144, 220, 255), progress);
            }

            if (clamped <= SafeMaxTemperature)
            {
                float progress = (clamped - SafeMinTemperature) / (SafeMaxTemperature - SafeMinTemperature);
                return Color.Lerp(new Color(154, 224, 137), new Color(255, 208, 84), progress);
            }

            float hotProgress = (clamped - SafeMaxTemperature) / (DisplayMaxTemperature - SafeMaxTemperature);
            return Color.Lerp(new Color(255, 174, 72), new Color(255, 84, 64), hotProgress);
        }

        public static string FormatDuration(int durationTicks)
        {
            int totalSeconds = Math.Max(1, durationTicks / 60);
            if (totalSeconds >= 60)
            {
                return $"{totalSeconds / 60}m";
            }

            return $"{totalSeconds}s";
        }

        private static bool HasArmorSet(Player player, int headType, int bodyType, int legType)
        {
            return player.armor[0].type == headType
                && player.armor[1].type == bodyType
                && player.armor[2].type == legType;
        }

        private static float GetArmorTemperatureOffset(Item item)
        {
            if (!IsArmorItem(item))
            {
                return 0f;
            }

            if (ArmorTemperatureOffsets.TryGetValue(item.type, out float explicitOffset))
            {
                return explicitOffset;
            }

            return GetFallbackArmorTemperatureOffset(item);
        }

        private static float GetAccessoryTemperatureOffset(Item item)
        {
            if (!IsAccessoryItem(item))
            {
                return 0f;
            }

            return AccessoryTemperatureOffsets.TryGetValue(item.type, out float explicitOffset)
                ? explicitOffset
                : 0f;
        }

        private static bool IsArmorItem(Item item)
        {
            return item != null
                && !item.IsAir
                && (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0);
        }

        private static bool IsAccessoryItem(Item item)
        {
            return item != null
                && !item.IsAir
                && item.accessory;
        }

        private static float GetFallbackArmorTemperatureOffset(Item item)
        {
            float baseOffset = item.vanity
                ? GetVanityArmorBaseOffset(item)
                : GetProtectiveArmorBaseOffset(item);

            float themeBias = GetArmorThemeBias(item);
            return MathHelper.Clamp(baseOffset + themeBias, -4.5f, 5.5f);
        }

        private static float GetProtectiveArmorBaseOffset(Item item)
        {
            float slotOffset = item.bodySlot >= 0 ? 0.75f : item.legSlot >= 0 ? 0.45f : 0.35f;
            return slotOffset + (item.defense * 0.11f);
        }

        private static float GetVanityArmorBaseOffset(Item item)
        {
            return item.bodySlot >= 0 ? 0.35f : item.legSlot >= 0 ? 0.25f : 0.2f;
        }

        private static float GetArmorThemeBias(Item item)
        {
            string lookupName = GetArmorLookupName(item);
            if (string.IsNullOrWhiteSpace(lookupName))
            {
                return 0f;
            }

            if (ContainsAny(lookupName, "SolarFlare", "Molten", "Meteor", "Hellstone", "Hell", "Lava", "Inferno", "Sun", "Phoenix"))
            {
                return 2.25f;
            }

            if (ContainsAny(lookupName, "Frost", "Frozen", "Ice", "Icy", "Glacier", "Snow", "Boreal", "Arctic", "Blizzard", "Vortex", "Viking"))
            {
                return -2.25f;
            }

            if (ContainsAny(lookupName, "Cobalt"))
            {
                return -0.9f;
            }

            if (ContainsAny(lookupName, "Stardust"))
            {
                return -0.45f;
            }

            if (ContainsAny(lookupName, "Obsidian", "Forbidden", "Squire", "ValhallaKnight", "Beetle", "Turtle", "Chlorophyte", "Jungle", "Bee", "Pearlwood", "RichMahogany", "Wood", "Bamboo", "Cactus", "Pumpkin", "Spooky", "Shroom", "Mushroom"))
            {
                return 0.85f;
            }

            if (ContainsAny(lookupName, "Titanium", "Adamantite", "Palladium", "Mythril", "Orichalcum", "Hallowed", "Nebula", "Spectre", "Crystal"))
            {
                return 0.45f;
            }

            if (ContainsAny(lookupName, "Angler", "Fisher", "Pirate", "Sail", "Rain"))
            {
                return 0.5f;
            }

            if (ContainsAny(lookupName, "Mining", "NightVision", "Gi", "Ninja", "Wizard", "Magic", "Apprentice", "DarkArtist", "Monk", "RedRiding", "Familiar", "Tuxedo", "Plumber", "Hero"))
            {
                return 0.25f;
            }

            return 0f;
        }

        private static float GetGenericArmorSetContribution(Player player)
        {
            Item head = player.armor[0];
            Item body = player.armor[1];
            Item legs = player.armor[2];

            if (!IsArmorItem(head) || !IsArmorItem(body) || !IsArmorItem(legs))
            {
                return 0f;
            }

            string headFamily = GetArmorFamilyKey(head);
            string bodyFamily = GetArmorFamilyKey(body);
            string legFamily = GetArmorFamilyKey(legs);
            if (string.IsNullOrWhiteSpace(headFamily)
                || !headFamily.Equals(bodyFamily, StringComparison.OrdinalIgnoreCase)
                || !headFamily.Equals(legFamily, StringComparison.OrdinalIgnoreCase))
            {
                return 0f;
            }

            float pieceSum = GetArmorTemperatureOffset(head) + GetArmorTemperatureOffset(body) + GetArmorTemperatureOffset(legs);
            if (Math.Abs(pieceSum) < 0.01f)
            {
                return 0f;
            }

            return MathHelper.Clamp(pieceSum * 0.2f, -3f, 3f);
        }

        private static string GetArmorFamilyKey(Item item)
        {
            string lookupName = GetArmorLookupName(item);
            if (string.IsNullOrWhiteSpace(lookupName))
            {
                return string.Empty;
            }

            string[] suffixes =
            {
                "Helmet", "Mask", "Hat", "Headgear", "Hood", "Cap", "Crown", "Helm", "Visor", "Headpiece", "Head", "Hairpin", "Wig",
                "Breastplate", "Chainmail", "Platemail", "PlateMail", "Scalemail", "Shirt", "Robe", "Coat", "Tunic", "Jacket", "Doublet", "Jerkin", "Chestguard", "Chestplate", "Cuirass", "Garment",
                "Greaves", "Leggings", "Pants", "Cuisses", "Trousers", "Kilt", "Skirt", "Boots", "Slacks"
            };

            for (int i = 0; i < suffixes.Length; i++)
            {
                string suffix = suffixes[i];
                if (lookupName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) && lookupName.Length > suffix.Length)
                {
                    return lookupName[..^suffix.Length];
                }
            }

            return string.Empty;
        }

        private static string GetArmorLookupName(Item item)
        {
            string vanillaIdName = ItemID.Search.GetName(item.type);
            if (!string.IsNullOrWhiteSpace(vanillaIdName))
            {
                return vanillaIdName;
            }

            if (item.ModItem != null && !string.IsNullOrWhiteSpace(item.ModItem.Name))
            {
                return item.ModItem.Name;
            }

            return item.Name?.Replace(" ", string.Empty) ?? string.Empty;
        }

        private static bool ContainsAny(string source, params string[] keywords)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if (source.Contains(keywords[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
