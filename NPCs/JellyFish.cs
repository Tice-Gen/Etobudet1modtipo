using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Etobudet1modtipo.Players;
using Etobudet1modtipo.Common;
using Etobudet1modtipo.Global;

namespace Etobudet1modtipo.NPCs
{
    public class JellyFish : ModNPC
    {
        private static readonly int[] AnimationSequence = { 0, 1, 0, 1, 2, 2, 2 };

        private const int FrameCount = 3;
        private const int FrameWidth = 34;
        private const int FrameHeight = 38;
        private const int FrameTickDelay = 8;

        private const float DashSpeed = 5.6f;
        private const float WaterSlowdown = 0.86f;
        private const float MaxWaterSpeed = 6.0f;
        private const float WallProbeDistance = 32f; // 2 tiles
        private const float VerticalDashStrength = 0.85f;
        private const float SmallSwimForce = 0.12f;
        private const int PostDashSwimTicks = 16;
        private const int DryDamageDelayTicks = 5 * 60;
        private const int WaterGunExtraGraceTicks = 2 * 60;

        private const int LandFlopDelay = 16;
        private const float LandHopSpeedX = 2.6f;
        private const float LandHopSpeedY = 5.2f;
        private const float LandTwoBlockJumpSpeedY = 7.4f;
        private const float LandWallProbeDistance = 14f;
        private const int LandRepathTicks = 45;
        private const int ContactDamage = 22;
        private const int ContactHitCooldown = 20;
        private const int IncomingHitCooldown = 60;
        private const int IncomingHitChance = 5; // 1/5 chance when touching hostile mobs
        private const float IncomingDamageScale = 0.35f;
        private const float HostileTargetRange = 320f;
        private const float SchoolRadius = 50f;
        private const int SchoolMinSize = 3;
        private const int SchoolMaxSize = 9;
        private const float SchoolSpawnRadius = 56f;
        private const float SchoolSpawnMinSeparation = 12f;
        private const int SchoolSpawnAttemptsPerMember = 30;
        private const float SupportDetectRange = 200f;
        private const float SupportRetreatDistance = 120f;
        private const float SupportSafeDistance = 180f;
        private const int DangerousTargetLifeThreshold = 1000;
        private const float DaySpawnChance = 0.16f;
        private const float NightSpawnChance = 0.24f;
        private const float MaxMoveRotation = 0.3f;
        private const float MoveRotationLerp = 0.16f;
        private const float IdleRotationLerp = 0.1f;
        private const float MoveRotationFromHorizontalSpeed = 0.055f;
        private const float LowLifeRatio = 0.42f;
        private const int AllyHealAmount = 3;
        private const int AllyHealCooldownTicks = 24;
        private const int MaxNearbySchoolmates = 16;
        private const int WaterSeekHalfWidth = 80;
        private const int WaterSeekHalfHeight = 55;

        private int frameTimer;
        private int sequenceIndex;
        private int frameIndex;
        private bool dashQueued;
        private bool animationRhythmInitialized;
        private int perNpcFrameTickDelay;
        private int perNpcDashFrameInterval;
        private int dashFrameCounter;
        private float roamVerticalDirection = 0.25f;
        private Vector2 glideDirection = Vector2.UnitX;
        private int glideTimer;
        private int allyHealCooldown;
        private int dryOutTimer;
        private int dryOutExtraGraceTicks;
        private int rescueWaterGunHits;
        private int rescueHelperPlayer = -1;
        private bool wasInWater;

        private int swimDirection = 1;
        private int landFlopTimer;
        private int landRepathTimer;
        private Vector2? landWaterTarget;

        private readonly int[] hostileHitCooldowns = new int[Main.maxNPCs];
        private readonly int[] incomingHitCooldowns = new int[Main.maxNPCs];
        private readonly int[] nearbySchoolmates = new int[MaxNearbySchoolmates];
        private readonly int[] waterProjectileHitCooldown = new int[Main.maxProjectiles];

        public override string Texture => "Etobudet1modtipo/NPCs/Jellyfish";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = FrameCount;
        }

        public override void SetDefaults()
        {
            NPC.width = FrameWidth;
            NPC.height = FrameHeight;

            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 99;
            NPC.value = Item.buyPrice(silver: 3);
            NPC.knockBackResist = 0.75f;

            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.aiStyle = -1;
            AIType = -1;
            AnimationType = -1;

            NPC.noGravity = true;
            NPC.noTileCollide = false;

            for (int i = 0; i < waterProjectileHitCooldown.Length; i++)
            {
                waterProjectileHitCooldown[i] = 0;
            }

        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            // Jellyfish spawned by the school logic should not recursively create more schools.
            if (NPC.ai[3] == 1f)
            {
                return;
            }

            int desiredSchoolSize = Main.rand.Next(SchoolMinSize, SchoolMaxSize + 1);
            int currentSchoolSize = CountNearbyJellyfish(NPC.Center, SchoolSpawnRadius) + 1;
            int toSpawn = desiredSchoolSize - currentSchoolSize;

            if (toSpawn <= 0)
            {
                return;
            }

            IEntitySource schoolSource = NPC.GetSource_FromAI();
            for (int i = 0; i < toSpawn; i++)
            {
                if (!TryFindSchoolSpawnPosition(NPC.Center, out Vector2 spawnPosition))
                {
                    break;
                }

                int npcIndex = NPC.NewNPC(
                    schoolSource,
                    (int)spawnPosition.X,
                    (int)spawnPosition.Y,
                    Type,
                    ai3: 1f);

                if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    continue;
                }

                NPC spawned = Main.npc[npcIndex];
                if (!spawned.active || spawned.type != Type)
                {
                    continue;
                }

                spawned.velocity = Main.rand.NextVector2Circular(1.2f, 1.2f);
                spawned.netUpdate = true;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.04f, 0.17f, 0.28f);

            if (!wasInWater && NPC.wet)
            {
                TryCompleteAnimalsSaverAchievement();
            }

            TickHostileHitCooldowns();
            if (allyHealCooldown > 0)
            {
                allyHealCooldown--;
            }

            bool wetDebuffActive = NPC.HasBuff(BuffID.Wet);

            if (NPC.wet)
            {
                dryOutTimer = 0;
                RunWaterAI();
            }
            else
            {
                if (wetDebuffActive)
                {
                    // Wet debuff protects from drying, but jellyfish still behaves like on land and seeks water.
                    dryOutTimer = 0;
                }
                else if (dryOutTimer < int.MaxValue)
                {
                    dryOutTimer++;
                }

                RunLandAI();
                ApplyDryOutDamage(wetDebuffActive);
            }

            TryDamageHostileNPCsOnContact();
            TryReceiveRareHostileContactDamage();
            UpdateFacing();
            wasInWater = NPC.wet;
        }

        public override void FindFrame(int frameHeight)
        {
            EnsureAnimationRhythmInitialized();
            int sequenceLength = NPC.wet ? AnimationSequence.Length : 4;

            if (!NPC.wet)
            {
                sequenceIndex %= sequenceLength;
                frameIndex %= 2;
            }

            frameTimer++;
            if (frameTimer >= perNpcFrameTickDelay)
            {
                frameTimer = 0;
                sequenceIndex++;
                if (sequenceIndex >= sequenceLength)
                {
                    sequenceIndex = 0;
                }

                if (NPC.wet)
                {
                    frameIndex = AnimationSequence[sequenceIndex];
                    if (frameIndex == 2)
                    {
                        dashFrameCounter++;
                        if (dashFrameCounter >= perNpcDashFrameInterval)
                        {
                            dashQueued = true;
                            dashFrameCounter = 0;
                        }
                    }
                }
                else
                {
                    // On land, keep only frames 1 and 2 (indices 0 and 1).
                    frameIndex = sequenceIndex % 2;
                }
            }

            NPC.frame.X = 0;
            NPC.frame.Width = FrameWidth;
            NPC.frame.Height = FrameHeight;
            NPC.frame.Y = FrameHeight * frameIndex;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Water || spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0f;
            }

            if (CalamityAbyssHelper.IsCalamityLoaded())
            {
                CalamityAbyssLayer abyssLayer = CalamityAbyssHelper.GetAbyssLayer(spawnInfo.Player);
                bool inAllowedAbyssLayer =
                    abyssLayer == CalamityAbyssLayer.Layer1 ||
                    abyssLayer == CalamityAbyssLayer.Layer2 ||
                    abyssLayer == CalamityAbyssLayer.Layer3 ||
                    abyssLayer == CalamityAbyssLayer.Layer4;

                // With Calamity enabled, allow jellyfish in all Abyss layers and in regular ocean.
                if (!inAllowedAbyssLayer && !spawnInfo.Player.ZoneBeach)
                {
                    return 0f;
                }
            }
            else if (!spawnInfo.Player.ZoneBeach)
            {
                return 0f;
            }

            return Main.dayTime ? DaySpawnChance : NightSpawnChance;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("A peaceful jellyfish that prefers the company of its own kind."))
            });
        }

        private void RunWaterAI()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.velocity *= WaterSlowdown;
            ApplyPostDashSwim();

            bool wallAhead = IsWallAhead(swimDirection);
            if (NPC.collideX || wallAhead)
            {
                swimDirection *= -1;
                dashQueued = true;
            }

            if (NPC.collideY)
            {
                roamVerticalDirection *= -1f;
            }

            NPC.direction = swimDirection;

            int nearbySchoolCount = GetNearbySchoolmates();
            NPC hostileTarget = FindNearbyHostileNPC();

            bool lowLife = NPC.life <= (int)(NPC.lifeMax * LowLifeRatio);
            NPC injuredAlly = FindMostInjuredAllyInSupportRange();
            bool shouldHealAlly = injuredAlly != null &&
                                  injuredAlly.whoAmI != NPC.whoAmI &&
                                  IsAssignedAsHealer(injuredAlly, nearbySchoolCount >= 2 ? 2 : 1);

            Vector2 schoolAdjust = GetSchoolSteering(nearbySchoolCount);
            if (schoolAdjust != Vector2.Zero)
            {
                NPC.velocity += schoolAdjust * 0.06f;
            }

            if (dashQueued)
            {
                Vector2 dashDirection = GetDashDirection(hostileTarget, injuredAlly, shouldHealAlly, lowLife, nearbySchoolCount);

                if (NPC.collideX)
                {
                    swimDirection *= -1;
                    dashDirection = new Vector2(swimDirection, -roamVerticalDirection).SafeNormalize(new Vector2(swimDirection, 0f));
                }

                if (IsDirectionBlocked(dashDirection))
                {
                    swimDirection *= -1;
                    roamVerticalDirection = -roamVerticalDirection;
                    dashDirection = new Vector2(swimDirection, roamVerticalDirection).SafeNormalize(new Vector2(swimDirection, 0f));
                }

                dashDirection = KeepDashInsideWater(dashDirection);
                NPC.velocity = dashDirection * DashSpeed;
                if (NPC.velocity.LengthSquared() > MaxWaterSpeed * MaxWaterSpeed)
                {
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * MaxWaterSpeed;
                }

                glideDirection = NPC.velocity.SafeNormalize(new Vector2(swimDirection, 0f));
                glideTimer = PostDashSwimTicks;

                if (NPC.velocity.X > 0.02f)
                {
                    swimDirection = 1;
                }
                else if (NPC.velocity.X < -0.02f)
                {
                    swimDirection = -1;
                }

                dashQueued = false;
                NPC.netUpdate = true;
            }

            if (shouldHealAlly)
            {
                TryHealAllyByRamming(injuredAlly);
            }
        }

        private void RunLandAI()
        {
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            if (landRepathTimer > 0)
            {
                landRepathTimer--;
            }

            if (landRepathTimer <= 0 || !landWaterTarget.HasValue || Vector2.DistanceSquared(NPC.Center, landWaterTarget.Value) < 20f * 20f)
            {
                landWaterTarget = FindNearestWaterWorldDirectional(NPC.Center, swimDirection);
                landRepathTimer = LandRepathTicks;
            }

            bool wallAhead = IsLandWallAhead(swimDirection);
            bool onGround = NPC.velocity.Y == 0f;
            if ((NPC.collideX || wallAhead) && onGround)
            {
                if (CanClearTwoBlockObstacle(swimDirection))
                {
                    // Prefer jumping over short walls instead of turning around.
                    NPC.velocity.Y = -LandTwoBlockJumpSpeedY;
                    NPC.velocity.X = swimDirection * LandHopSpeedX;
                    landFlopTimer = LandFlopDelay;
                    NPC.netUpdate = true;
                }
                else
                {
                    swimDirection *= -1;
                    landWaterTarget = FindNearestWaterWorldDirectional(NPC.Center, swimDirection);
                    landRepathTimer = LandRepathTicks;
                    landFlopTimer = 0;
                }
            }

            if (landFlopTimer > 0)
            {
                landFlopTimer--;
            }

            NPC.velocity.X *= 0.95f;

            if (onGround && landFlopTimer <= 0)
            {
                Vector2? waterTarget = landWaterTarget;
                float dirX;
                if (waterTarget.HasValue)
                {
                    dirX = System.Math.Sign(waterTarget.Value.X - NPC.Center.X);
                    if (dirX == 0f)
                    {
                        dirX = swimDirection == 0 ? 1f : swimDirection;
                    }
                }
                else
                {
                    dirX = swimDirection == 0 ? 1f : swimDirection;
                }

                swimDirection = dirX > 0f ? 1 : -1;
                NPC.velocity.Y = -LandHopSpeedY;
                NPC.velocity.X = dirX * LandHopSpeedX;
                landFlopTimer = LandFlopDelay;
                NPC.netUpdate = true;
            }
        }

        private void ApplyDryOutDamage(bool wetDebuffActive)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (wetDebuffActive)
            {
                return;
            }

            if (dryOutExtraGraceTicks > 0)
            {
                dryOutExtraGraceTicks--;
                return;
            }

            if (dryOutTimer < DryDamageDelayTicks)
            {
                return;
            }

            NPC.life--;
            if (NPC.life <= 0)
            {
                NPC.life = 0;
                NPC.checkDead();
                return;
            }

            if (Main.GameUpdateCount % 15 == 0)
            {
                NPC.netUpdate = true;
            }
        }

        private void TickHostileHitCooldowns()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (hostileHitCooldowns[i] > 0)
                {
                    hostileHitCooldowns[i]--;
                }

                if (incomingHitCooldowns[i] > 0)
                {
                    incomingHitCooldowns[i]--;
                }
            }
        }

        private void TryDamageHostileNPCsOnContact()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Rectangle myHitbox = NPC.Hitbox;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (!IsValidHostileTarget(other) || hostileHitCooldowns[i] > 0)
                {
                    continue;
                }

                if (!myHitbox.Intersects(other.Hitbox))
                {
                    continue;
                }

                int hitDirection = other.Center.X >= NPC.Center.X ? 1 : -1;
                other.SimpleStrikeNPC(ContactDamage, hitDirection, crit: false, knockBack: 1f);
                hostileHitCooldowns[i] = ContactHitCooldown;
                other.GetGlobalNPC<JellyFishAggroGlobalNPC>().RegisterHitByJellyfish(NPC);
            }
        }

        private NPC FindNearbyHostileNPC()
        {
            NPC best = null;
            float bestDistSq = HostileTargetRange * HostileTargetRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (!IsValidHostileTarget(candidate))
                {
                    continue;
                }

                float distSq = Vector2.DistanceSquared(NPC.Center, candidate.Center);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    best = candidate;
                }
            }

            return best;
        }

        private int GetNearbySchoolmates()
        {
            int count = 0;
            float maxDistSq = SchoolRadius * SchoolRadius;

            for (int i = 0; i < Main.maxNPCs && count < MaxNearbySchoolmates; i++)
            {
                NPC other = Main.npc[i];
                if (other == null || !other.active || other.type != Type || other.whoAmI == NPC.whoAmI)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(NPC.Center, other.Center) > maxDistSq)
                {
                    continue;
                }

                if (!Collision.CanHitLine(NPC.Center, 1, 1, other.Center, 1, 1))
                {
                    continue;
                }

                nearbySchoolmates[count++] = other.whoAmI;
            }

            return count;
        }

        private Vector2 GetSchoolSteering(int nearbySchoolCount)
        {
            if (nearbySchoolCount <= 0)
            {
                return Vector2.Zero;
            }

            Vector2 center = Vector2.Zero;
            Vector2 separation = Vector2.Zero;
            int valid = 0;

            for (int i = 0; i < nearbySchoolCount; i++)
            {
                NPC ally = Main.npc[nearbySchoolmates[i]];
                if (ally == null || !ally.active)
                {
                    continue;
                }

                center += ally.Center;
                valid++;

                Vector2 away = NPC.Center - ally.Center;
                float dist = away.Length();
                if (dist > 0.01f && dist < 20f)
                {
                    separation += away / dist;
                }
            }

            if (valid == 0)
            {
                return Vector2.Zero;
            }

            center /= valid;
            Vector2 cohesion = (center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 steer = cohesion + separation * 1.25f;
            return steer.SafeNormalize(Vector2.Zero);
        }

        private NPC FindMostInjuredAllyInSupportRange()
        {
            NPC best = null;
            float bestScore = 0f;
            float rangeSq = SupportDetectRange * SupportDetectRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC ally = Main.npc[i];
                if (ally == null || !ally.active || ally.type != Type)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(NPC.Center, ally.Center) > rangeSq)
                {
                    continue;
                }

                if (!Collision.CanHitLine(NPC.Center, 1, 1, ally.Center, 1, 1))
                {
                    continue;
                }

                float missingRatio = 1f - ally.life / (float)ally.lifeMax;
                if (missingRatio <= 0f)
                {
                    continue;
                }

                if (missingRatio > bestScore)
                {
                    bestScore = missingRatio;
                    best = ally;
                }
            }

            return best;
        }

        private bool IsAssignedAsHealer(NPC injuredAlly, int wantedHealers)
        {
            if (injuredAlly == null || wantedHealers <= 0)
            {
                return false;
            }

            float thisDist = Vector2.DistanceSquared(NPC.Center, injuredAlly.Center);
            int betterHealthyCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC ally = Main.npc[i];
                if (ally == null || !ally.active || ally.type != Type || ally.whoAmI == injuredAlly.whoAmI)
                {
                    continue;
                }

                bool allyLowLife = ally.life <= (int)(ally.lifeMax * LowLifeRatio);
                if (allyLowLife)
                {
                    continue;
                }

                if (!Collision.CanHitLine(ally.Center, 1, 1, injuredAlly.Center, 1, 1))
                {
                    continue;
                }

                float allyDist = Vector2.DistanceSquared(ally.Center, injuredAlly.Center);
                if (allyDist < thisDist || (allyDist == thisDist && ally.whoAmI < NPC.whoAmI))
                {
                    betterHealthyCount++;
                    if (betterHealthyCount >= wantedHealers)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private Vector2 GetDashDirection(NPC hostileTarget, NPC injuredAlly, bool shouldHealAlly, bool lowLife, int nearbySchoolCount)
        {
            if (shouldHealAlly && injuredAlly != null)
            {
                Vector2 healOffset = new Vector2((NPC.whoAmI % 2 == 0 ? -14f : 14f), NPC.whoAmI % 3 == 0 ? -8f : 8f);
                return (injuredAlly.Center + healOffset - NPC.Center).SafeNormalize(new Vector2(swimDirection, roamVerticalDirection));
            }

            if (lowLife)
            {
                int wantedHealers = nearbySchoolCount >= 2 ? 2 : 1;
                NPC healer = FindClosestAssignedHealerForSelf(wantedHealers);
                if (healer != null)
                {
                    // Injured jellyfish responds to healer signal and moves toward nearest assigned healer.
                    Vector2 toHealer = healer.Center - NPC.Center;
                    if (toHealer.LengthSquared() > 16f)
                    {
                        return toHealer.SafeNormalize(new Vector2(swimDirection, roamVerticalDirection));
                    }
                }
            }

            if (lowLife && hostileTarget != null)
            {
                float distToHostile = Vector2.Distance(NPC.Center, hostileTarget.Center);
                if (distToHostile < SupportSafeDistance)
                {
                    return (NPC.Center - hostileTarget.Center).SafeNormalize(new Vector2(swimDirection, roamVerticalDirection));
                }

                Vector2 lateral = new Vector2(swimDirection, roamVerticalDirection).SafeNormalize(Vector2.UnitX);
                return lateral;
            }

            if (hostileTarget != null)
            {
                Vector2 toEnemy = hostileTarget.Center - NPC.Center;
                float enemyDist = toEnemy.Length();
                if (lowLife && enemyDist < SupportRetreatDistance)
                {
                    return (-toEnemy).SafeNormalize(new Vector2(swimDirection, roamVerticalDirection));
                }

                return toEnemy.SafeNormalize(new Vector2(swimDirection, roamVerticalDirection));
            }

            roamVerticalDirection += Main.rand.NextFloat(-0.55f, 0.55f);
            roamVerticalDirection = MathHelper.Clamp(roamVerticalDirection, -VerticalDashStrength, VerticalDashStrength);
            return new Vector2(swimDirection, roamVerticalDirection).SafeNormalize(new Vector2(swimDirection, 0f));
        }

        private NPC FindClosestAssignedHealerForSelf(int wantedHealers)
        {
            NPC best = null;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (candidate == null || !candidate.active || candidate.type != Type || candidate.whoAmI == NPC.whoAmI)
                {
                    continue;
                }

                bool candidateLowLife = candidate.life <= (int)(candidate.lifeMax * LowLifeRatio);
                if (candidateLowLife)
                {
                    continue;
                }

                if (!Collision.CanHitLine(candidate.Center, 1, 1, NPC.Center, 1, 1))
                {
                    continue;
                }

                NPC candidateTarget = FindMostInjuredAllyFor(candidate);
                if (candidateTarget == null || candidateTarget.whoAmI != NPC.whoAmI)
                {
                    continue;
                }

                if (!IsNpcAssignedAsHealerForTarget(candidate, NPC, wantedHealers))
                {
                    continue;
                }

                float distSq = Vector2.DistanceSquared(candidate.Center, NPC.Center);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    best = candidate;
                }
            }

            return best;
        }

        private NPC FindMostInjuredAllyFor(NPC observer)
        {
            NPC best = null;
            float bestScore = 0f;
            float rangeSq = SupportDetectRange * SupportDetectRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC ally = Main.npc[i];
                if (ally == null || !ally.active || ally.type != Type)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(observer.Center, ally.Center) > rangeSq)
                {
                    continue;
                }

                if (!Collision.CanHitLine(observer.Center, 1, 1, ally.Center, 1, 1))
                {
                    continue;
                }

                float missingRatio = 1f - ally.life / (float)ally.lifeMax;
                if (missingRatio <= 0f)
                {
                    continue;
                }

                if (missingRatio > bestScore)
                {
                    bestScore = missingRatio;
                    best = ally;
                }
            }

            return best;
        }

        private bool IsNpcAssignedAsHealerForTarget(NPC healer, NPC injuredTarget, int wantedHealers)
        {
            float healerDist = Vector2.DistanceSquared(healer.Center, injuredTarget.Center);
            int betterHealthyCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC ally = Main.npc[i];
                if (ally == null || !ally.active || ally.type != Type || ally.whoAmI == injuredTarget.whoAmI)
                {
                    continue;
                }

                bool allyLowLife = ally.life <= (int)(ally.lifeMax * LowLifeRatio);
                if (allyLowLife)
                {
                    continue;
                }

                if (!Collision.CanHitLine(ally.Center, 1, 1, injuredTarget.Center, 1, 1))
                {
                    continue;
                }

                float allyDist = Vector2.DistanceSquared(ally.Center, injuredTarget.Center);
                if (allyDist < healerDist || (allyDist == healerDist && ally.whoAmI < healer.whoAmI))
                {
                    betterHealthyCount++;
                    if (betterHealthyCount >= wantedHealers)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void TryHealAllyByRamming(NPC ally)
        {
            if (ally == null || !ally.active || ally.whoAmI == NPC.whoAmI || allyHealCooldown > 0)
            {
                return;
            }

            if (!NPC.Hitbox.Intersects(ally.Hitbox))
            {
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (ally.life >= ally.lifeMax)
            {
                return;
            }

            ally.life += AllyHealAmount;
            if (ally.life > ally.lifeMax)
            {
                ally.life = ally.lifeMax;
            }

            ally.netUpdate = true;
            allyHealCooldown = AllyHealCooldownTicks;
        }

        private void ApplyPostDashSwim()
        {
            if (glideTimer <= 0)
            {
                return;
            }

            glideTimer--;
            float strength = SmallSwimForce * (glideTimer / (float)PostDashSwimTicks);
            NPC.velocity += glideDirection * strength;
        }

        private void TryReceiveRareHostileContactDamage()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || NPC.life <= 0)
            {
                return;
            }

            Rectangle myHitbox = NPC.Hitbox;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (!IsValidHostileTarget(other) || incomingHitCooldowns[i] > 0)
                {
                    continue;
                }

                if (!myHitbox.Intersects(other.Hitbox))
                {
                    continue;
                }

                if (!Main.rand.NextBool(IncomingHitChance))
                {
                    continue;
                }

                int damage = System.Math.Max(1, (int)(other.damage * IncomingDamageScale));
                int hitDirection = other.Center.X >= NPC.Center.X ? 1 : -1;
                NPC.SimpleStrikeNPC(damage, hitDirection, crit: false, knockBack: 0.6f);
                incomingHitCooldowns[i] = IncomingHitCooldown;
            }
        }

        private Vector2 KeepDashInsideWater(Vector2 dashDirection)
        {
            Vector2 dir = dashDirection.SafeNormalize(new Vector2(swimDirection == 0 ? 1f : swimDirection, 0f));
            Point centerTile = NPC.Center.ToTileCoordinates();
            Tile current = Framing.GetTileSafely(centerTile.X, centerTile.Y);
            Tile above = Framing.GetTileSafely(centerTile.X, centerTile.Y - 1);

            bool nearSurface = current.LiquidType == LiquidID.Water &&
                               current.LiquidAmount > 0 &&
                               (above.LiquidType != LiquidID.Water || above.LiquidAmount < 80);

            if (nearSurface && dir.Y < -0.12f)
            {
                dir.Y = 0.2f;
            }

            Vector2 probe = NPC.Center + dir * 24f;
            Tile probeTile = Framing.GetTileSafely(probe.ToTileCoordinates());
            if (probeTile.LiquidType != LiquidID.Water || probeTile.LiquidAmount < 70)
            {
                dir.Y = System.Math.Abs(dir.Y) + 0.2f;
            }

            return dir.SafeNormalize(new Vector2(swimDirection == 0 ? 1f : swimDirection, 0.2f));
        }

        private bool IsValidHostileTarget(NPC candidate)
        {
            if (candidate == null ||
                !candidate.active ||
                candidate.whoAmI == NPC.whoAmI ||
                candidate.type == Type ||
                candidate.friendly ||
                candidate.townNPC ||
                candidate.dontTakeDamage ||
                candidate.life <= 0 ||
                candidate.boss)
            {
                return false;
            }

            // Requested behavior: ignore dangerous high-HP enemies only when they can fight back.
            if (candidate.lifeMax > DangerousTargetLifeThreshold && candidate.damage > 0)
            {
                return false;
            }

            return true;
        }

        private int CountNearbyJellyfish(Vector2 center, float radius)
        {
            int count = 0;
            float radiusSq = radius * radius;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (other == null || !other.active || other.type != Type || other.whoAmI == NPC.whoAmI)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(other.Center, center) <= radiusSq)
                {
                    count++;
                }
            }

            return count;
        }

        private bool TryFindSchoolSpawnPosition(Vector2 center, out Vector2 spawnPosition)
        {
            for (int attempt = 0; attempt < SchoolSpawnAttemptsPerMember; attempt++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(SchoolSpawnRadius, SchoolSpawnRadius);
                if (offset.LengthSquared() < SchoolSpawnMinSeparation * SchoolSpawnMinSeparation)
                {
                    continue;
                }

                Vector2 candidate = center + offset;
                Point tile = candidate.ToTileCoordinates();

                if (!WorldGen.InWorld(tile.X, tile.Y, 5))
                {
                    continue;
                }

                Tile waterTile = Framing.GetTileSafely(tile.X, tile.Y);
                if (waterTile.LiquidType != LiquidID.Water || waterTile.LiquidAmount < 120)
                {
                    continue;
                }

                Rectangle hitbox = new Rectangle(
                    (int)(candidate.X - NPC.width * 0.5f),
                    (int)(candidate.Y - NPC.height * 0.5f),
                    NPC.width,
                    NPC.height);

                if (Collision.SolidCollision(hitbox.TopLeft(), hitbox.Width, hitbox.Height))
                {
                    continue;
                }

                if (IsTooCloseToOtherJellyfish(candidate, SchoolSpawnMinSeparation))
                {
                    continue;
                }

                spawnPosition = candidate;
                return true;
            }

            spawnPosition = default;
            return false;
        }

        private bool IsTooCloseToOtherJellyfish(Vector2 candidateCenter, float minDistance)
        {
            float minDistanceSq = minDistance * minDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (other == null || !other.active || other.type != Type)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(other.Center, candidateCenter) < minDistanceSq)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateFacing()
        {
            if (NPC.velocity.X > 0.03f)
            {
                swimDirection = 1;
            }
            else if (NPC.velocity.X < -0.03f)
            {
                swimDirection = -1;
            }

            NPC.direction = swimDirection;
            NPC.spriteDirection = swimDirection;

            float targetRotation = 0f;
            float absHorizontalSpeed = System.Math.Abs(NPC.velocity.X);
            if (NPC.wet && absHorizontalSpeed > 0.02f)
            {
                // Stable lean from horizontal movement only. This avoids sudden sprite flips.
                targetRotation = MathHelper.Clamp(
                    NPC.velocity.X * MoveRotationFromHorizontalSpeed,
                    -MaxMoveRotation,
                    MaxMoveRotation);
            }

            float rotationLerp = (NPC.wet && absHorizontalSpeed > 0.02f) ? MoveRotationLerp : IdleRotationLerp;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, rotationLerp);
        }

        private void EnsureAnimationRhythmInitialized()
        {
            if (animationRhythmInitialized)
            {
                return;
            }

            // Deterministic per-NPC rhythm keeps school members desynced without MP randomness issues.
            int seed = unchecked(NPC.whoAmI * 73 + NPC.type * 17 + (int)(NPC.position.X / 16f));
            int positiveSeed = seed & int.MaxValue;
            perNpcFrameTickDelay = 6 + (positiveSeed % 5); // 6..10 ticks between frame steps
            perNpcDashFrameInterval = 1 + ((positiveSeed / 5) % 3); // trigger dash each 1..3 occurrences of frame 3
            if (perNpcDashFrameInterval < 1)
            {
                perNpcDashFrameInterval = 1;
            }

            int wetSequenceLength = AnimationSequence.Length;
            sequenceIndex = positiveSeed % wetSequenceLength;
            frameTimer = (positiveSeed / 13) % perNpcFrameTickDelay;
            frameIndex = NPC.wet ? AnimationSequence[sequenceIndex] : sequenceIndex % 2;
            dashFrameCounter = (positiveSeed / 29) % perNpcDashFrameInterval;
            animationRhythmInitialized = true;
        }

        private bool IsWallAhead(int direction)
        {
            int dir = direction == 0 ? 1 : direction;
            Vector2 probePos = NPC.position + new Vector2(dir * WallProbeDistance, 0f);
            return Collision.SolidCollision(probePos, NPC.width, NPC.height);
        }

        private bool IsDirectionBlocked(Vector2 direction)
        {
            Vector2 dir = direction.SafeNormalize(Vector2.UnitX * (swimDirection == 0 ? 1 : swimDirection));
            Vector2 probePos = NPC.position + dir * WallProbeDistance;
            return Collision.SolidCollision(probePos, NPC.width, NPC.height);
        }

        private static Vector2? FindNearestWaterWorld(Vector2 worldPos)
        {
            int centerX = (int)(worldPos.X / 16f);
            int centerY = (int)(worldPos.Y / 16f);
            int minX = Utils.Clamp(centerX - WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int maxX = Utils.Clamp(centerX + WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int minY = Utils.Clamp(centerY - WaterSeekHalfHeight, 1, Main.maxTilesY - 2);
            int maxY = Utils.Clamp(centerY + WaterSeekHalfHeight, 1, Main.maxTilesY - 2);

            Vector2 best = default;
            float bestDistSq = float.MaxValue;
            bool found = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null)
                    {
                        continue;
                    }

                    if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 100)
                    {
                        Vector2 candidate = new Vector2(x * 16f + 8f, y * 16f + 8f);
                        float distSq = Vector2.DistanceSquared(worldPos, candidate);
                        if (distSq < bestDistSq)
                        {
                            bestDistSq = distSq;
                            best = candidate;
                            found = true;
                        }
                    }
                }
            }

            return found ? best : null;
        }

        private static Vector2? FindNearestWaterWorldDirectional(Vector2 worldPos, int preferredDirection)
        {
            int centerX = (int)(worldPos.X / 16f);
            int centerY = (int)(worldPos.Y / 16f);
            int minX = Utils.Clamp(centerX - WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int maxX = Utils.Clamp(centerX + WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int minY = Utils.Clamp(centerY - WaterSeekHalfHeight, 1, Main.maxTilesY - 2);
            int maxY = Utils.Clamp(centerY + WaterSeekHalfHeight, 1, Main.maxTilesY - 2);

            Vector2 bestPreferred = default;
            Vector2 bestAny = default;
            float bestPreferredDistSq = float.MaxValue;
            float bestAnyDistSq = float.MaxValue;
            bool foundPreferred = false;
            bool foundAny = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null || tile.LiquidType != LiquidID.Water || tile.LiquidAmount <= 100)
                    {
                        continue;
                    }

                    Vector2 candidate = new Vector2(x * 16f + 8f, y * 16f + 8f);
                    float distSq = Vector2.DistanceSquared(worldPos, candidate);
                    if (distSq < bestAnyDistSq)
                    {
                        bestAnyDistSq = distSq;
                        bestAny = candidate;
                        foundAny = true;
                    }

                    int candidateDir = System.Math.Sign(candidate.X - worldPos.X);
                    if (preferredDirection != 0 && candidateDir == preferredDirection && distSq < bestPreferredDistSq)
                    {
                        bestPreferredDistSq = distSq;
                        bestPreferred = candidate;
                        foundPreferred = true;
                    }
                }
            }

            if (foundPreferred)
            {
                return bestPreferred;
            }

            return foundAny ? bestAny : null;
        }

        private bool IsLandWallAhead(int direction)
        {
            int dir = direction == 0 ? 1 : direction;
            Vector2 probePos = NPC.position + new Vector2(dir * LandWallProbeDistance, 0f);
            return Collision.SolidCollision(probePos, NPC.width, NPC.height);
        }

        private bool CanClearTwoBlockObstacle(int direction)
        {
            int dir = direction == 0 ? 1 : direction;
            Vector2 forward = NPC.position + new Vector2(dir * 6f, 0f);

            // Must be blocked at current height...
            if (!Collision.SolidCollision(forward, NPC.width, NPC.height))
            {
                return false;
            }

            // ...but have space two tiles above for a jump arc.
            Vector2 twoBlocksUp = forward + new Vector2(0f, -32f);
            if (Collision.SolidCollision(twoBlocksUp, NPC.width, NPC.height))
            {
                return false;
            }

            return true;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            TryRegisterWaterGunHit(projectile);
        }

        public void RegisterWaterGunHitFromProjectile(Projectile projectile)
        {
            TryRegisterWaterGunHit(projectile);
        }

        private void TryCompleteAnimalsSaverAchievement()
        {
            if (rescueWaterGunHits < 5 || rescueHelperPlayer < 0 || rescueHelperPlayer >= Main.maxPlayers)
            {
                ResetAnimalsSaverProgress();
                return;
            }

            Player helper = Main.player[rescueHelperPlayer];
            if (helper != null && helper.active)
            {
                AnimalsSaverAchievementPlayer helperData = helper.GetModPlayer<AnimalsSaverAchievementPlayer>();
                helperData.TryUnlockAnimalsSaver();
            }

            ResetAnimalsSaverProgress();
        }

        private void ResetAnimalsSaverProgress()
        {
            rescueWaterGunHits = 0;
            rescueHelperPlayer = -1;
        }

        private void TryRegisterWaterGunHit(Projectile projectile)
        {
            if (projectile == null || !IsWaterGunProjectile(projectile.type))
            {
                return;
            }

            // Server-authoritative counting avoids client/server desync in multiplayer.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int projIndex = projectile.whoAmI;
            if (projIndex < 0 || projIndex >= Main.maxProjectiles)
            {
                return;
            }

            int now = (int)Main.GameUpdateCount;
            if (waterProjectileHitCooldown[projIndex] > now)
            {
                return;
            }

            // Avoid per-tick spam while still allowing repeated counts from continuous water streams.
            waterProjectileHitCooldown[projIndex] = now + 8;

            if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
            {
                Player owner = Main.player[projectile.owner];
                if (owner != null && owner.active)
                {
                    owner.GetModPlayer<AnimalsSaverAchievementPlayer>().AddKindnessPoint();
                }
            }

            if (NPC.wet)
            {
                return;
            }

            dryOutExtraGraceTicks += WaterGunExtraGraceTicks;
            if (dryOutExtraGraceTicks > 15 * 60)
            {
                dryOutExtraGraceTicks = 15 * 60;
            }

            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            if (rescueHelperPlayer == -1 || rescueHelperPlayer == projectile.owner)
            {
                rescueHelperPlayer = projectile.owner;
                rescueWaterGunHits++;
            }
            else
            {
                // If another player starts helping, transfer progress to that helper.
                rescueHelperPlayer = projectile.owner;
                rescueWaterGunHits = 1;
            }
        }

        private static bool IsWaterGunProjectile(int projectileType)
        {
            return projectileType == ProjectileID.WaterGun || projectileType == ProjectileID.WaterStream;
        }
    }
}
