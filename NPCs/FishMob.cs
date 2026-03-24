using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Common;
using ReLogic.Utilities;

namespace Etobudet1modtipo.NPCs
{
    public class FishMob : ModNPC
    {
        private const int FrameCount = 4;
        private const int FrameWidth = 242;
        private const int FrameHeight = 190;
        private const int HitboxWidth = 70; 
        private const int HitboxHeight = 70; 
        private const int RequiredWaterTiles = 1000;
        private const int WaterScanHalfWidth = 80;
        private const int WaterScanHalfHeight = 45;
        private const float SwimSpeed = 10f;
        private const float PassiveSwimSpeed = 6.4f;
        private const float DashSpeed = 16f;
        private const float MinWaterSpeed = 3.1f;
        private const int DashWindupTicks = 75;
        private const int DashCooldownMin = 150;
        private const int DashCooldownMax = 240;
        private const int LandHopDelay = 20;
        private const float LandHopStrength = 8.5f;
        private const float LandMoveSpeed = 5.2f;
        private const int WaterSeekHalfWidth = 110;
        private const int WaterSeekHalfHeight = 70;
        private const int DeepSeaVolleyProjectiles = 3;
        private const int DeepSeaVolleyRepeatCount = 2;
        private const int DeepSeaVolleyInterval = 10;
        private const int DeepSeaShotSpeed = 8;
        private const float DeepSeaSpreadRadians = 0.18f;
        private const int DeepSeaBurstChance = 220;
        private const int DeepSeaBurstCooldownMin = 150;
        private const int DeepSeaBurstCooldownMax = 260;
        private const float DeepSeaAttackRange = 680f;
        private const int RetreatToPassiveDelay = 5 * 60;
        private const float RetreatFarDistance = 420f;
        private const int DeepWaterScanInterval = 45;
        private const int DamageWindowTicks = 15 * 60;
        private const int DamageToTriggerRegen = 1000;
        private const int RegenDurationTicks = 3 * 60;
        private const int RegenTickInterval = 30;
        private const int RegenHealPerTick = 1000;
        private const int DeepScreamRepeatDelayTicks = 5 * 60;
        private const int DeepSingingChance = 1200;
        private const int PathRecalcInterval = 18;
        private const int PathHalfWidthMaxTiles = 80;
        private const int PathHalfHeightMaxTiles = 60;
        private const int PathWaterMinAmount = 80;
        private const float ProximityAggroDistance = 500f;
        private const float NpcTargetSearchRange = 900f;
        private const int NpcBiteCooldownTicks = 18;
        private const float NpcBiteRange = 86f;
        private const int AttackerDamageWindowTicks = 60;
        private const int AttackerDpsSwitchThreshold = 100;
        private const float MobPlayerPriorityRange = 200f;
        private const int HostileMobAttackCooldownTicks = 20;
        private const float HostileMobAttackRangePadding = 12f;
        private const float HostileMobChaseMinSpeed = 2.2f;
        private const float HostileMobChaseMaxSpeed = 7.5f;
        private const float HostileMobChaseLerp = 0.08f;
        private const int HostileMobShootCooldownTicks = 42;
        private const float HostileMobShootMinDistance = 110f;
        private const float HostileMobShootMaxDistance = 760f;
        private const float HostileMobShootSpeed = 10.8f;
        private const float HostileMobShootInaccuracy = 0.1f;
        private const float HostileMobShootDamageScale = 0.7f;
        private const int UpSwimCheckInterval = 90;
        private const int UpSwimChancePercent = 5;
        private const int UpSwimRiseTicks = 24;
        private const int UpSwimDescendTicks = 42;
        private const float UpSwimSideSpeed = 4.7f;
        private const float UpSwimRiseSpeed = 5.1f;
        private const float UpSwimDescendSpeed = 2.3f;
        private const int StuckCheckTicks = 36;
        private const float StuckDistanceThreshold = 10f;
        private const float ObstacleProbeDistance = 34f;
        private const float ObstacleVerticalProbe = 30f;
        private const float ShardLikeDashSpeed = 21f;
        private const int ShardLikeDashDuration = 24;
        private const int ShardLikeMaxDashes = 1;
        private const int ShardLikeBurstCooldown = 120;
        private const float ShardLikeReachDistance = 88f;
        private const float ShardLikeResetDistance = 220f;
        private const int OtherWatersFishMobSoftCap = 6;
        private const int MaxNpcAggroLife = 10000;
        private const float VoidLayerPlayerCheckRange = 1800f;

        private int dashCooldown;
        private int dashWindup;
        private int landHopTimer;
        private int deepSeaBurstCooldown;
        private int deepSeaVolleysRemaining;
        private int deepSeaVolleyTimer;
        private int recentDamageIndex;
        private int recentDamageTotal;
        private int regenTimer;
        private int regenTickTimer;
        private int retreatTimer;
        private int deepWaterScanTimer;
        private Vector2? deepWaterTarget;
        private SlotId deepScreamSlot;
        private bool deepScreamActive;
        private int deepScreamCooldown;
        private SlotId deepSingingSlot;
        private bool deepSingingActive;
        private int pathRecalcTimer;
        private Vector2? pathTargetWorld;
        private int npcBiteCooldown;
        private int upSwimCheckTimer;
        private int upSwimState;
        private int upSwimTimer;
        private float upSwimDirection;
        private int stuckTimer;
        private Vector2 stuckReferencePosition;
        private int shardLikeDashTimer;
        private int shardLikeDashCount;
        private int shardLikeBurstCooldownTimer;
        private int attackerDamageIndex;
        private readonly int[] recentDamageByTick = new int[DamageWindowTicks];
        private readonly int[,] playerDamageByTick = new int[Main.maxPlayers, AttackerDamageWindowTicks];
        private readonly int[] playerDamageRecentTotal = new int[Main.maxPlayers];
        private readonly int[,] npcDamageByTick = new int[Main.maxNPCs, AttackerDamageWindowTicks];
        private readonly int[] npcDamageRecentTotal = new int[Main.maxNPCs];
        private readonly int[] hostileMobHitCooldowns = new int[Main.maxNPCs];
        private readonly int[] hostileMobShootCooldowns = new int[Main.maxNPCs];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = FrameCount;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = HitboxWidth;
            NPC.height = HitboxHeight;

            NPC.damage = 120;
            NPC.defense = 110;
            NPC.lifeMax = 990000;
            NPC.value = 250f;
            NPC.knockBackResist = 0f;

            NPC.HitSound = new SoundStyle("Etobudet1modtipo/Sounds/DeepHit")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 20
            };

            NPC.DeathSound = new SoundStyle("Etobudet1modtipo/Sounds/DeepDeath")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 20
            };

            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.aiStyle = -1;
            AnimationType = -1;
            NPC.buffImmune[ModContent.BuffType<Pressure>()] = true;
        }

        public override void AI()
        {
            if (NPC.timeLeft < 750)
            {
                NPC.timeLeft = 750;
            }

            AdvanceDamageWindow();
            AdvanceAttackerDamageWindow();
            UpdateDamageRegen();
            UpdateHostileMobAggroAndDamage();
            if (npcBiteCooldown > 0)
            {
                npcBiteCooldown--;
            }
            Lighting.AddLight(NPC.Center, 0.04f, 0.11f, 0.16f); // Very weak blue glow.
            UpdateDeepScreamLoop();
            UpdateDeepSingingChance();

            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.97f;
                NPC.velocity.Y += 0.2f;
                return;
            }

            if (NPC.wet)
            {
                NPC.noGravity = true;
                NPC.noTileCollide = false;
                float playerDistance = Vector2.Distance(NPC.Center, player.Center);
                bool playerAggro = player.wet || playerDistance <= ProximityAggroDistance;
                NPC mobTarget = FindHostileMobTarget();
                bool mobAggro = false;
                float mobDistance = float.MaxValue;

                if (mobTarget != null)
                {
                    mobDistance = Vector2.Distance(NPC.Center, mobTarget.Center);
                    mobAggro = mobTarget.wet || mobDistance <= ProximityAggroDistance;
                }

                bool forceDpsTarget = TryGetHighDpsAggroTarget(out Player dpsPlayerTarget, out NPC dpsNpcTarget);

                bool useMobTarget;
                bool shouldAggro;
                bool combatTargetWet;
                Vector2 combatTarget;
                NPC activeMobTarget = mobTarget;

                if (forceDpsTarget)
                {
                    useMobTarget = dpsNpcTarget != null;
                    shouldAggro = true;
                    if (useMobTarget)
                    {
                        activeMobTarget = dpsNpcTarget;
                        combatTarget = dpsNpcTarget.Center;
                        combatTargetWet = dpsNpcTarget.wet;
                    }
                    else
                    {
                        player = dpsPlayerTarget;
                        combatTarget = dpsPlayerTarget.Center;
                        combatTargetWet = dpsPlayerTarget.wet;
                    }
                }
                else
                {
                    useMobTarget = mobAggro && (!playerAggro || mobDistance < playerDistance);
                    combatTarget = useMobTarget ? mobTarget.Center : player.Center;
                    combatTargetWet = useMobTarget ? mobTarget.wet : player.wet;
                    shouldAggro = useMobTarget || playerAggro;
                }

                if (shouldAggro)
                {
                    retreatTimer = 0;

                    if (combatTargetWet)
                    {
                        UpdateWaterPathTarget(combatTarget);
                    }
                    else
                    {
                        ClearWaterPathTarget();
                    }

                    bool hasDirectLine = Collision.CanHitLine(NPC.Center, 1, 1, combatTarget, 1, 1);
                    bool playerIsCombatTarget = !useMobTarget;
                    if (!playerIsCombatTarget)
                    {
                        ResetShardLikeDash();
                    }

                    SwimAggressive(combatTarget, hasDirectLine, playerIsCombatTarget);
                    HandleDeepSeaBurst(combatTarget);

                    if (useMobTarget)
                    {
                        TryBiteMobTarget(activeMobTarget);
                    }
                }
                else
                {
                    // Player is on land: fish usually swims away from the shoreline/player.
                    dashWindup = 0;
                    ResetDeepSeaBurst();
                    ClearWaterPathTarget();
                    ResetShardLikeDash();

                    if (retreatTimer < RetreatToPassiveDelay)
                    {
                        retreatTimer++;
                    }

                    float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);
                    if (retreatTimer < RetreatToPassiveDelay || distanceToPlayer < RetreatFarDistance)
                    {
                        SwimRetreatFromPlayer(player);
                    }
                    else
                    {
                        SwimPassive();
                    }
                }

                ApplyWaterObstacleAvoidance();
                UpdateUpSwimMotion();
                UpdateStuckRecovery(shouldAggro ? (Vector2?)combatTarget : null);
                KeepWaterSpeed();
            }
            else
            {
                // On land: do not chase the player, jump toward nearest water source.
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                ResetDeepSeaBurst();
                ClearWaterPathTarget();
                ResetShardLikeDash();
                upSwimState = 0;
                stuckTimer = 0;
                stuckReferencePosition = NPC.Center;
                HopToNearestWater();
            }

            if (NPC.velocity.X != 0f)
            {
                NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.direction = NPC.spriteDirection;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            RegisterDamageTaken(damageDone);
            RegisterPlayerDamageSource(player?.whoAmI ?? -1, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            RegisterDamageTaken(damageDone);

            if (projectile != null &&
                projectile.friendly &&
                !projectile.hostile &&
                projectile.owner >= 0 &&
                projectile.owner < Main.maxPlayers)
            {
                RegisterPlayerDamageSource(projectile.owner, damageDone);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0 || Main.dedServ)
            {
                return;
            }

            if (deepScreamActive && SoundEngine.TryGetActiveSound(deepScreamSlot, out ActiveSound scream))
            {
                scream.Stop();
            }

            if (deepSingingActive && SoundEngine.TryGetActiveSound(deepSingingSlot, out ActiveSound singing))
            {
                singing.Stop();
            }

            deepScreamActive = false;
            deepSingingActive = false;

            int gore1 = Mod.Find<ModGore>("FishMobGore1").Type;
            int gore2 = Mod.Find<ModGore>("FishMobGore2").Type;
            int gore3 = Mod.Find<ModGore>("FishMobGore3").Type;

            SpawnFishGore(gore1, 1.15f);
            SpawnFishGore(gore2, 1.1f);
            SpawnFishGore(gore3, 1.2f);
        }

        public override void FindFrame(int frameHeight)
        {
            // Texture sheet expected: 242x760 with 4 vertical frames of 242x190.
            NPC.frameCounter++;
            if (NPC.frameCounter >= 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += FrameHeight;

                if (NPC.frame.Y >= FrameHeight * FrameCount)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = NPC.frame.Size() * 0.5f;
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
            {
                if (NPC.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (NPC.oldPos.Length - i) / (float)NPC.oldPos.Length;
                Vector2 drawPos = new Vector2(
                    NPC.oldPos[i].X + NPC.width * 0.5f,
                    NPC.oldPos[i].Y + NPC.height - NPC.frame.Height * 0.5f + NPC.gfxOffY
                ) - screenPos;
                Color shadowColor = new Color(0, 0, 0, 190) * (0.55f * progress);
                float scale = NPC.scale * (0.9f + progress * 0.28f);
                float rotation = i < NPC.oldRot.Length ? NPC.oldRot[i] : NPC.rotation;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    NPC.frame,
                    shadowColor,
                    rotation,
                    origin,
                    scale,
                    effects,
                    0
                );
            }

            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Water || spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0f;
            }

            if (!NPC.downedMoonlord)
            {
                return 0f;
            }

            int fishMobCount = NPC.CountNPCS(Type);
            bool inOtherWaters = global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive();

            // Hard rule: never allow more than one FishMob alive at the same time.
            if (fishMobCount > 0)
            {
                return 0f;
            }

            if (CalamityAbyssHelper.IsCalamityLoaded())
            {
                if (CalamityAbyssHelper.GetAbyssLayer(spawnInfo.Player) != CalamityAbyssLayer.Layer4)
                {
                    return 0f;
                }

                if (Main.GameUpdateCount % 60 != 0)
                {
                    return 0f;
                }

                return 1f / 100f;
            }

            // Inside Other Waters allow groups, but keep a soft cap.
            if (inOtherWaters && fishMobCount >= OtherWatersFishMobSoftCap)
            {
                return 0f;
            }

            if (!IsLargeWaterBody(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
            {
                return 0f;
            }

            if (Main.GameUpdateCount % 60 != 0)
            {
                return 0f;
            }

            return 1f / 100f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement("A large predatory fish that hunts anything entering its waters.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeepSeaCutter>(), 15));
        }

        private void SwimAggressive(Vector2 targetCenter, bool canDirectlyReachTarget, bool isPlayerTarget)
        {
            if (dashCooldown > 0)
            {
                dashCooldown--;
            }

            if (shardLikeBurstCooldownTimer > 0)
            {
                shardLikeBurstCooldownTimer--;
            }

            Vector2 chaseTarget = pathTargetWorld ?? targetCenter;
            Vector2 toTarget = chaseTarget - NPC.Center;
            Vector2 toDirectTarget = targetCenter - NPC.Center;
            float distance = toDirectTarget.Length();

            if (isPlayerTarget)
            {
                // Short chained dashes similar to AstraniseShard when the fish reaches the player.
                if (shardLikeDashTimer > 0)
                {
                    shardLikeDashTimer--;
                    if (shardLikeDashTimer <= 0)
                    {
                        NPC.velocity *= 0.5f;
                    }

                    return;
                }

                if (distance > ShardLikeResetDistance)
                {
                    shardLikeDashCount = 0;
                }

                if (distance <= ShardLikeReachDistance && canDirectlyReachTarget && shardLikeBurstCooldownTimer <= 0)
                {
                    if (shardLikeDashCount >= ShardLikeMaxDashes)
                    {
                        shardLikeDashCount = 0;
                        shardLikeBurstCooldownTimer = ShardLikeBurstCooldown;
                    }
                    else
                    {
                        Vector2 dashDirection = toDirectTarget.SafeNormalize(new Vector2(NPC.direction == 0 ? 1f : NPC.direction, 0f));
                        NPC.velocity = dashDirection * ShardLikeDashSpeed;
                        shardLikeDashTimer = ShardLikeDashDuration;
                        shardLikeDashCount++;
                        NPC.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                        return;
                    }
                }
            }

            if (distance < 280f && canDirectlyReachTarget)
            {
                dashWindup++;
            }
            else
            {
                dashWindup = 0;
            }

            bool canDash = dashCooldown <= 0 && dashWindup >= DashWindupTicks && distance > 8f && canDirectlyReachTarget;
            if (canDash)
            {
                // 100% dash when the fish commits to an attack.
                Vector2 dashDir = Vector2.Normalize(toDirectTarget);
                NPC.velocity = dashDir * DashSpeed;
                dashCooldown = Main.rand.Next(DashCooldownMin, DashCooldownMax + 1);
                dashWindup = 0;
                NPC.netUpdate = true;
                return;
            }

            // Faster turning and stronger vertical correction when already aligned by X.
            float absX = Math.Abs(toTarget.X);
            float absY = Math.Abs(toTarget.Y);
            float inertia = absX < 44f ? 4.4f : 8.5f;
            float targetSpeed = SwimSpeed;

            Vector2 desired = toTarget.Length() > 6f ? Vector2.Normalize(toTarget) * targetSpeed : Vector2.Zero;

            if (absX < 44f && absY > 24f)
            {
                desired.Y *= 1.85f;
                desired.X *= 0.78f;
            }

            // Vertical axis reacts faster so the fish can climb/descend quickly after matching X.
            float xLerp = 1f / inertia;
            float yLerp = absX < 70f ? 0.30f : 0.20f;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desired.X, xLerp);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, desired.Y, yLerp);

            if (toTarget.Y < -20f && absX < 120f)
            {
                NPC.velocity.Y -= 0.20f;
            }
        }

        private void UpdateWaterPathTarget(Vector2 targetCenter)
        {
            if (pathRecalcTimer > 0)
            {
                pathRecalcTimer--;
            }

            bool needRepath = pathRecalcTimer <= 0 || !pathTargetWorld.HasValue;
            if (!needRepath)
            {
                float distanceToPath = Vector2.DistanceSquared(NPC.Center, pathTargetWorld.Value);
                if (distanceToPath < 26f * 26f)
                {
                    needRepath = true;
                }
            }

            if (!needRepath)
            {
                return;
            }

            pathRecalcTimer = PathRecalcInterval;

            if (Collision.CanHitLine(NPC.Center, 1, 1, targetCenter, 1, 1))
            {
                pathTargetWorld = null;
                return;
            }

            pathTargetWorld = FindWaterPathStepToward(targetCenter);
        }

        private void ClearWaterPathTarget()
        {
            pathTargetWorld = null;
            pathRecalcTimer = 0;
        }

        private Vector2? FindWaterPathStepToward(Vector2 targetWorld)
        {
            Point startTileRaw = NPC.Center.ToTileCoordinates();
            Point goalTileRaw = targetWorld.ToTileCoordinates();

            Point? startTile = FindNearestSwimmableTile(startTileRaw, 6);
            Point? goalTile = FindNearestSwimmableTile(goalTileRaw, 8);
            if (!startTile.HasValue || !goalTile.HasValue)
            {
                return null;
            }

            int centerX = (startTile.Value.X + goalTile.Value.X) / 2;
            int centerY = (startTile.Value.Y + goalTile.Value.Y) / 2;
            int halfW = Math.Min(PathHalfWidthMaxTiles, Math.Abs(goalTile.Value.X - startTile.Value.X) + 24);
            int halfH = Math.Min(PathHalfHeightMaxTiles, Math.Abs(goalTile.Value.Y - startTile.Value.Y) + 18);

            int minX = Utils.Clamp(centerX - halfW, 1, Main.maxTilesX - 2);
            int maxX = Utils.Clamp(centerX + halfW, 1, Main.maxTilesX - 2);
            int minY = Utils.Clamp(centerY - halfH, 1, Main.maxTilesY - 2);
            int maxY = Utils.Clamp(centerY + halfH, 1, Main.maxTilesY - 2);

            if (!IsTileInside(startTile.Value, minX, maxX, minY, maxY) || !IsTileInside(goalTile.Value, minX, maxX, minY, maxY))
            {
                return null;
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            int nodeCount = width * height;

            int[] previous = new int[nodeCount];
            bool[] visited = new bool[nodeCount];
            Array.Fill(previous, -1);

            int startIndex = ToLocalIndex(startTile.Value.X, startTile.Value.Y, minX, minY, width);
            int goalIndex = ToLocalIndex(goalTile.Value.X, goalTile.Value.Y, minX, minY, width);

            Queue<int> queue = new Queue<int>();
            queue.Enqueue(startIndex);
            visited[startIndex] = true;

            int[] dirX = { 1, -1, 0, 0, 1, 1, -1, -1 };
            int[] dirY = { 0, 0, 1, -1, 1, -1, 1, -1 };

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                if (current == goalIndex)
                {
                    break;
                }

                int localX = current % width;
                int localY = current / width;
                int worldX = minX + localX;
                int worldY = minY + localY;

                for (int i = 0; i < 8; i++)
                {
                    int nx = worldX + dirX[i];
                    int ny = worldY + dirY[i];
                    if (nx < minX || nx > maxX || ny < minY || ny > maxY)
                    {
                        continue;
                    }

                    if (!IsSwimmableTile(nx, ny))
                    {
                        continue;
                    }

                    bool isDiagonal = i >= 4;
                    if (isDiagonal)
                    {
                        if (!IsSwimmableTile(worldX + dirX[i], worldY) || !IsSwimmableTile(worldX, worldY + dirY[i]))
                        {
                            continue;
                        }
                    }

                    int ni = ToLocalIndex(nx, ny, minX, minY, width);
                    if (visited[ni])
                    {
                        continue;
                    }

                    visited[ni] = true;
                    previous[ni] = current;
                    queue.Enqueue(ni);
                }
            }

            int resolvedGoal = goalIndex;
            if (!visited[goalIndex])
            {
                int goalLocalX = goalIndex % width;
                int goalLocalY = goalIndex / width;
                int bestAlt = -1;
                float bestAltDistSq = float.MaxValue;

                for (int i = 0; i < nodeCount; i++)
                {
                    if (!visited[i] || i == startIndex)
                    {
                        continue;
                    }

                    int localX = i % width;
                    int localY = i / width;
                    float distSq = Vector2.DistanceSquared(
                        new Vector2(localX, localY),
                        new Vector2(goalLocalX, goalLocalY)
                    );

                    if (distSq < bestAltDistSq)
                    {
                        bestAltDistSq = distSq;
                        bestAlt = i;
                    }
                }

                if (bestAlt == -1)
                {
                    return null;
                }

                resolvedGoal = bestAlt;
            }

            int step = resolvedGoal;
            while (previous[step] != -1 && previous[step] != startIndex)
            {
                step = previous[step];
            }

            int stepLocalX = step % width;
            int stepLocalY = step / width;
            return new Vector2((minX + stepLocalX) * 16f + 8f, (minY + stepLocalY) * 16f + 8f);
        }

        private Point? FindNearestSwimmableTile(Point origin, int radius)
        {
            if (IsSwimmableTile(origin.X, origin.Y))
            {
                return origin;
            }

            for (int r = 1; r <= radius; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        int x = origin.X + dx;
                        int y = origin.Y + dy;
                        if (x <= 1 || x >= Main.maxTilesX - 1 || y <= 1 || y >= Main.maxTilesY - 1)
                        {
                            continue;
                        }

                        if (IsSwimmableTile(x, y))
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsTileInside(Point tile, int minX, int maxX, int minY, int maxY)
        {
            return tile.X >= minX && tile.X <= maxX && tile.Y >= minY && tile.Y <= maxY;
        }

        private static int ToLocalIndex(int worldX, int worldY, int minX, int minY, int width)
        {
            return (worldX - minX) + (worldY - minY) * width;
        }

        private static bool IsSwimmableTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
            {
                return false;
            }

            if (tile.LiquidType != LiquidID.Water || tile.LiquidAmount < PathWaterMinAmount)
            {
                return false;
            }

            if (WorldGen.SolidOrSlopedTile(x, y))
            {
                return false;
            }

            return true;
        }

        private NPC FindHostileMobTarget()
        {
            float maxDistSq = NpcTargetSearchRange * NpcTargetSearchRange;
            NPC best = null;
            float bestScore = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (!candidate.active || candidate.whoAmI == NPC.whoAmI || candidate.type == NPC.type)
                {
                    continue;
                }

                if (!IsValidNpcCombatTarget(candidate))
                {
                    continue;
                }

                float distSq = Vector2.DistanceSquared(NPC.Center, candidate.Center);
                if (distSq > maxDistSq)
                {
                    continue;
                }

                float score = candidate.wet ? distSq * 0.85f : distSq;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            return best;
        }

        private bool TryGetHighDpsAggroTarget(out Player playerTarget, out NPC npcTarget)
        {
            playerTarget = null;
            npcTarget = null;
            int bestDamagePerSecond = AttackerDpsSwitchThreshold;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                int damage = playerDamageRecentTotal[i];
                if (damage <= bestDamagePerSecond)
                {
                    continue;
                }

                Player candidate = Main.player[i];
                if (!candidate.active || candidate.dead)
                {
                    continue;
                }

                bestDamagePerSecond = damage;
                playerTarget = candidate;
                npcTarget = null;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                int damage = npcDamageRecentTotal[i];
                if (damage <= bestDamagePerSecond)
                {
                    continue;
                }

                NPC candidate = Main.npc[i];
                if (!candidate.active || candidate.whoAmI == NPC.whoAmI || candidate.type == NPC.type)
                {
                    continue;
                }

                if (!IsValidNpcCombatTarget(candidate))
                {
                    continue;
                }

                bestDamagePerSecond = damage;
                npcTarget = candidate;
                playerTarget = null;
            }

            return playerTarget != null || npcTarget != null;
        }

        private void UpdateHostileMobAggroAndDamage()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            for (int i = 0; i < hostileMobHitCooldowns.Length; i++)
            {
                if (hostileMobHitCooldowns[i] > 0)
                {
                    hostileMobHitCooldowns[i]--;
                }
            }

            for (int i = 0; i < hostileMobShootCooldowns.Length; i++)
            {
                if (hostileMobShootCooldowns[i] > 0)
                {
                    hostileMobShootCooldowns[i]--;
                }
            }

            float maxDistSq = NpcTargetSearchRange * NpcTargetSearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC candidate = Main.npc[i];
                if (!candidate.active || candidate.whoAmI == NPC.whoAmI || candidate.type == NPC.type)
                {
                    continue;
                }

                if (!IsValidNpcCombatTarget(candidate))
                {
                    continue;
                }

                if (IsPlayerNearby(candidate.Center, MobPlayerPriorityRange))
                {
                    continue;
                }

                float distSq = Vector2.DistanceSquared(candidate.Center, NPC.Center);
                if (distSq > maxDistSq)
                {
                    continue;
                }

                Vector2 toFish = NPC.Center - candidate.Center;
                float distance = (float)Math.Sqrt(distSq);
                if (distance > 0.01f)
                {
                    Vector2 direction = toFish / distance;
                    float chaseSpeed = MathHelper.Clamp(candidate.velocity.Length(), HostileMobChaseMinSpeed, HostileMobChaseMaxSpeed);
                    Vector2 desiredVelocity = direction * chaseSpeed;
                    candidate.velocity = Vector2.Lerp(candidate.velocity, desiredVelocity, HostileMobChaseLerp);
                    candidate.direction = direction.X >= 0f ? 1 : -1;
                    candidate.spriteDirection = candidate.direction;
                }

                float attackRange = (candidate.width + NPC.width) * 0.5f + HostileMobAttackRangePadding;
                if (distSq > attackRange * attackRange)
                {
                    continue;
                }

                if (hostileMobHitCooldowns[candidate.whoAmI] > 0)
                {
                    continue;
                }

                if (!Collision.CanHitLine(candidate.Center, 1, 1, NPC.Center, 1, 1))
                {
                    continue;
                }

                int damage = Math.Max(1, candidate.damage);
                int hitDirection = NPC.Center.X >= candidate.Center.X ? 1 : -1;
                NPC.SimpleStrikeNPC(damage, hitDirection, crit: false, knockBack: 1f);
                RegisterDamageTaken(damage);
                RegisterNpcDamageSource(candidate.whoAmI, damage);
                hostileMobHitCooldowns[candidate.whoAmI] = HostileMobAttackCooldownTicks;
                NPC.netUpdate = true;
            }
        }

        private void TryHostileMobRangedAttack(NPC shooter, float distSq)
        {
            if (!CanMobUseRangedAttack(shooter))
            {
                return;
            }

            if (hostileMobShootCooldowns[shooter.whoAmI] > 0)
            {
                return;
            }

            float minDistSq = HostileMobShootMinDistance * HostileMobShootMinDistance;
            float maxDistSq = HostileMobShootMaxDistance * HostileMobShootMaxDistance;
            if (distSq < minDistSq || distSq > maxDistSq)
            {
                return;
            }

            if (!Collision.CanHitLine(shooter.Center, 1, 1, NPC.Center, 1, 1))
            {
                return;
            }

            Vector2 direction = (NPC.Center - shooter.Center).SafeNormalize(Vector2.UnitX);
            Vector2 velocity = direction
                .RotatedBy(Main.rand.NextFloat(-HostileMobShootInaccuracy, HostileMobShootInaccuracy))
                * HostileMobShootSpeed;

            int damage = Math.Max(1, (int)(shooter.damage * HostileMobShootDamageScale));
            Projectile.NewProjectile(
                shooter.GetSource_FromAI(),
                shooter.Center,
                velocity,
                ModContent.ProjectileType<FishMobHostileShot>(),
                damage,
                0f,
                Main.myPlayer,
                NPC.whoAmI,
                shooter.whoAmI
            );

            hostileMobShootCooldowns[shooter.whoAmI] = Main.rand.Next(HostileMobShootCooldownTicks - 6, HostileMobShootCooldownTicks + 12);
        }

        private static bool CanMobUseRangedAttack(NPC mob)
        {
            if (mob.type >= 0 && mob.type < NPCID.Sets.ProjectileNPC.Length && NPCID.Sets.ProjectileNPC[mob.type])
            {
                return true;
            }

            // Fallback for common ranged/caster styles when set flags are missing.
            return mob.aiStyle == 3 || mob.aiStyle == 8 || mob.aiStyle == 14 || mob.aiStyle == 29 || mob.aiStyle == 44 || mob.aiStyle == 77;
        }

        private static bool IsPlayerNearby(Vector2 center, float range)
        {
            float rangeSq = range * range;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(player.Center, center) <= rangeSq)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidNpcCombatTarget(NPC candidate)
        {
            if (candidate == null || !candidate.active || candidate.whoAmI == NPC.whoAmI || candidate.type == NPC.type || candidate.life <= 0)
            {
                return false;
            }

            if (candidate.boss || candidate.lifeMax > MaxNpcAggroLife)
            {
                return false;
            }

            int urchinType = ModContent.NPCType<DeepSeaUrchin>();
            if (candidate.type == urchinType)
            {
                return false;
            }

            if (IsNpcInCalamityVoid(candidate))
            {
                return false;
            }

            return candidate.CanBeChasedBy(this) && !candidate.friendly && !candidate.townNPC && !candidate.dontTakeDamage;
        }

        private static bool IsNpcInCalamityVoid(NPC candidate)
        {
            if (candidate == null || !candidate.active || !CalamityAbyssHelper.IsCalamityLoaded())
            {
                return false;
            }

            float maxDistSq = VoidLayerPlayerCheckRange * VoidLayerPlayerCheckRange;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(player.Center, candidate.Center) > maxDistSq)
                {
                    continue;
                }

                if (CalamityAbyssHelper.GetAbyssLayer(player) == CalamityAbyssLayer.Layer4)
                {
                    return true;
                }
            }

            return false;
        }

        public void RegisterExternalHostileDamage(int sourceNpcWhoAmI, int damageDone)
        {
            if (damageDone <= 0)
            {
                return;
            }

            RegisterDamageTaken(damageDone);
            RegisterNpcDamageSource(sourceNpcWhoAmI, damageDone);
        }

        private void TryBiteMobTarget(NPC target)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !IsValidNpcCombatTarget(target))
            {
                return;
            }

            if (npcBiteCooldown > 0)
            {
                return;
            }

            if (Vector2.DistanceSquared(NPC.Center, target.Center) > NpcBiteRange * NpcBiteRange)
            {
                return;
            }

            if (!Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
            {
                return;
            }

            int hitDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
            target.SimpleStrikeNPC(NPC.damage, hitDirection, crit: false, knockBack: 1f);
            npcBiteCooldown = NpcBiteCooldownTicks;
        }

        private void ApplyWaterObstacleAvoidance()
        {
            if (NPC.velocity.LengthSquared() < 0.25f)
            {
                return;
            }

            Vector2 forward = NPC.velocity.SafeNormalize(new Vector2(NPC.direction == 0 ? 1f : NPC.direction, 0f));
            Vector2 probe = NPC.Center + forward * ObstacleProbeDistance;
            if (IsWaterPassable(probe))
            {
                return;
            }

            Vector2 upProbe = probe + new Vector2(0f, -ObstacleVerticalProbe);
            Vector2 downProbe = probe + new Vector2(0f, ObstacleVerticalProbe);

            if (IsWaterPassable(upProbe))
            {
                NPC.velocity.Y -= 0.55f;
                return;
            }

            if (IsWaterPassable(downProbe))
            {
                NPC.velocity.Y += 0.40f;
                return;
            }

            NPC.velocity.X *= -0.45f;
            NPC.velocity.Y -= 1.4f;
            pathRecalcTimer = 0;
        }

        private void UpdateUpSwimMotion()
        {
            if (upSwimState == 0)
            {
                if (upSwimCheckTimer > 0)
                {
                    upSwimCheckTimer--;
                }

                if (upSwimCheckTimer <= 0)
                {
                    upSwimCheckTimer = UpSwimCheckInterval;
                    if (Main.rand.Next(100) < UpSwimChancePercent)
                    {
                        upSwimState = 1;
                        upSwimTimer = UpSwimRiseTicks;
                        upSwimDirection = Main.rand.NextBool() ? 1f : -1f;
                    }
                }

                return;
            }

            if (upSwimState == 1)
            {
                Vector2 riseDesired = new Vector2(upSwimDirection * UpSwimSideSpeed, -UpSwimRiseSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, riseDesired, 0.08f);
                upSwimTimer--;

                if (upSwimTimer <= 0)
                {
                    upSwimState = 2;
                    upSwimTimer = UpSwimDescendTicks;
                }

                return;
            }

            float descendProgress = 1f - upSwimTimer / (float)UpSwimDescendTicks;
            float sideSpeed = MathHelper.Lerp(UpSwimSideSpeed * 0.8f, UpSwimSideSpeed * 0.2f, descendProgress);
            float descendSpeed = MathHelper.Lerp(-1f, UpSwimDescendSpeed, descendProgress);
            Vector2 descendDesired = new Vector2(upSwimDirection * sideSpeed, descendSpeed);

            NPC.velocity = Vector2.Lerp(NPC.velocity, descendDesired, 0.06f);
            upSwimTimer--;

            if (upSwimTimer <= 0)
            {
                upSwimState = 0;
                upSwimCheckTimer = UpSwimCheckInterval;
            }
        }

        private void UpdateStuckRecovery(Vector2? desiredTarget)
        {
            if (stuckReferencePosition == Vector2.Zero)
            {
                stuckReferencePosition = NPC.Center;
            }

            float movedSq = Vector2.DistanceSquared(NPC.Center, stuckReferencePosition);
            float minMovedSq = StuckDistanceThreshold * StuckDistanceThreshold;
            float lowSpeedSq = SwimSpeed * SwimSpeed * 0.35f;

            if (NPC.velocity.LengthSquared() < lowSpeedSq && movedSq < minMovedSq)
            {
                stuckTimer++;
            }
            else
            {
                stuckTimer = Math.Max(0, stuckTimer - 2);
                if (movedSq >= minMovedSq)
                {
                    stuckReferencePosition = NPC.Center;
                }
            }

            if (stuckTimer < StuckCheckTicks)
            {
                return;
            }

            Vector2 fallbackTarget = desiredTarget
                ?? deepWaterTarget
                ?? (NPC.Center + new Vector2(Main.rand.NextBool() ? 120f : -120f, -80f));

            Vector2 escapeDir = (fallbackTarget - NPC.Center).SafeNormalize(Vector2.UnitY);
            if (escapeDir.Y > -0.15f)
            {
                escapeDir.Y -= 0.45f;
            }

            escapeDir.Normalize();
            NPC.velocity = escapeDir * (SwimSpeed + 2.4f);
            NPC.velocity.Y -= 1.2f;

            pathTargetWorld = null;
            pathRecalcTimer = 0;
            stuckTimer = 0;
            stuckReferencePosition = NPC.Center;
            NPC.netUpdate = true;
        }

        private static bool IsWaterPassable(Vector2 worldPos)
        {
            Point tilePos = worldPos.ToTileCoordinates();
            if (tilePos.X <= 1 || tilePos.X >= Main.maxTilesX - 1 || tilePos.Y <= 1 || tilePos.Y >= Main.maxTilesY - 1)
            {
                return false;
            }

            return IsSwimmableTile(tilePos.X, tilePos.Y);
        }

        private void HandleDeepSeaBurst(Vector2 targetCenter)
        {
            if (deepSeaBurstCooldown > 0)
            {
                deepSeaBurstCooldown--;
            }

            if (deepSeaVolleysRemaining > 0)
            {
                if (deepSeaVolleyTimer > 0)
                {
                    deepSeaVolleyTimer--;
                }

                if (deepSeaVolleyTimer <= 0)
                {
                    FireDeepSeaVolley(targetCenter);
                    deepSeaVolleysRemaining--;

                    if (deepSeaVolleysRemaining > 0)
                    {
                        deepSeaVolleyTimer = DeepSeaVolleyInterval;
                    }
                    else
                    {
                        deepSeaBurstCooldown = Main.rand.Next(DeepSeaBurstCooldownMin, DeepSeaBurstCooldownMax + 1);
                    }
                }

                return;
            }

            if (deepSeaBurstCooldown > 0)
            {
                return;
            }

            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget > DeepSeaAttackRange)
            {
                return;
            }

            if (!Collision.CanHitLine(NPC.Center, 1, 1, targetCenter, 1, 1))
            {
                return;
            }

            if (Main.rand.NextBool(DeepSeaBurstChance))
            {
                deepSeaVolleysRemaining = DeepSeaVolleyRepeatCount;
                deepSeaVolleyTimer = 0;
                NPC.netUpdate = true;
            }
        }

        private void FireDeepSeaVolley(Vector2 targetCenter)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Vector2 baseDirection = (targetCenter - NPC.Center).SafeNormalize(new Vector2(NPC.direction == 0 ? 1f : NPC.direction, 0f));
            Vector2 shootPos = NPC.Center + baseDirection * 24f;
            int projectileType = ModContent.ProjectileType<DeepSeaProj>();
            int projectileDamage = (int)(NPC.damage * 0.9f);

            for (int i = 0; i < DeepSeaVolleyProjectiles; i++)
            {
                float spreadLerp = (DeepSeaVolleyProjectiles == 1) ? 0f : i / (float)(DeepSeaVolleyProjectiles - 1);
                float angle = MathHelper.Lerp(-DeepSeaSpreadRadians, DeepSeaSpreadRadians, spreadLerp);
                Vector2 velocity = baseDirection.RotatedBy(angle) * DeepSeaShotSpeed;

                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    shootPos,
                    velocity,
                    projectileType,
                    projectileDamage,
                    0f,
                    Main.myPlayer
                );
            }
        }

        private void ResetDeepSeaBurst()
        {
            deepSeaVolleysRemaining = 0;
            deepSeaVolleyTimer = 0;
        }

        private void ResetShardLikeDash()
        {
            shardLikeDashTimer = 0;
            shardLikeDashCount = 0;
            shardLikeBurstCooldownTimer = 0;
        }

        private void SwimPassive()
        {
            if (deepWaterScanTimer > 0)
            {
                deepWaterScanTimer--;
            }

            if (deepWaterScanTimer <= 0 || !deepWaterTarget.HasValue)
            {
                deepWaterScanTimer = DeepWaterScanInterval;
                deepWaterTarget = FindDeeperWaterWorld(NPC.Center);
            }

            Vector2 desired;
            if (deepWaterTarget.HasValue && Vector2.DistanceSquared(NPC.Center, deepWaterTarget.Value) > 36f * 36f)
            {
                desired = (deepWaterTarget.Value - NPC.Center).SafeNormalize(Vector2.UnitY) * PassiveSwimSpeed;
            }
            else
            {
                float wave = (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.18f;
                desired = new Vector2(NPC.direction, wave);
                if (desired == Vector2.Zero)
                {
                    desired = new Vector2(1f, 0f);
                }

                desired.Normalize();
                desired *= PassiveSwimSpeed;
            }

            NPC.velocity = (NPC.velocity * 9f + desired) / 10f;
        }

        private void SwimRetreatFromPlayer(Player player)
        {
            Vector2 away = NPC.Center - player.Center;
            if (away.LengthSquared() < 0.001f)
            {
                away = new Vector2(NPC.direction == 0 ? 1f : NPC.direction, 0f);
            }

            away.Normalize();
            Vector2 desired = away * (PassiveSwimSpeed + 1.6f);

            // Slight down bias so it tends to move away into deeper water.
            desired.Y += 0.45f;

            float distSq = Vector2.DistanceSquared(NPC.Center, player.Center);
            if (distSq < 260f * 260f)
            {
                desired *= 1.18f;
            }

            NPC.velocity = (NPC.velocity * 7f + desired) / 8f;
        }

        private void KeepWaterSpeed()
        {
            // If touching the floor, push slightly upward so movement doesn't get stuck in collision response.
            if (NPC.collideY && NPC.velocity.Y >= -0.5f)
            {
                NPC.velocity.Y -= 2.6f;
            }

            if (NPC.collideX)
            {
                NPC.velocity.X *= -0.65f;
            }

            // Keep minimum total speed (not only X), so vertical movement isn't suppressed.
            float speedSq = NPC.velocity.LengthSquared();
            if (speedSq < MinWaterSpeed * MinWaterSpeed)
            {
                Vector2 dir = NPC.velocity;
                if (dir == Vector2.Zero)
                {
                    dir = new Vector2(NPC.direction == 0 ? 1f : NPC.direction, -0.1f);
                }

                dir.Normalize();
                NPC.velocity = dir * MinWaterSpeed;
            }
        }

        private void HopToNearestWater()
        {
            if (landHopTimer > 0)
            {
                landHopTimer--;
            }

            if (NPC.velocity.Y == 0f && landHopTimer <= 0)
            {
                Vector2? waterTarget = FindNearestWaterWorld(NPC.Center);
                float dirX;

                if (waterTarget.HasValue)
                {
                    dirX = Math.Sign(waterTarget.Value.X - NPC.Center.X);
                    if (dirX == 0f)
                    {
                        dirX = NPC.direction == 0 ? 1f : NPC.direction;
                    }
                }
                else
                {
                    dirX = NPC.direction == 0 ? 1f : NPC.direction;
                }

                NPC.velocity.X = dirX * LandMoveSpeed;
                NPC.velocity.Y = -LandHopStrength;
                landHopTimer = LandHopDelay;
                NPC.netUpdate = true;
            }

            NPC.velocity.X *= 0.985f;
        }

        private static bool IsLargeWaterBody(int centerX, int centerY)
        {
            int minX = Utils.Clamp(centerX - WaterScanHalfWidth, 1, Main.maxTilesX - 2);
            int maxX = Utils.Clamp(centerX + WaterScanHalfWidth, 1, Main.maxTilesX - 2);
            int minY = Utils.Clamp(centerY - WaterScanHalfHeight, 1, Main.maxTilesY - 2);
            int maxY = Utils.Clamp(centerY + WaterScanHalfHeight, 1, Main.maxTilesY - 2);

            int waterTiles = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null)
                    {
                        continue;
                    }

                    if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water)
                    {
                        waterTiles++;
                        if (waterTiles >= RequiredWaterTiles)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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

        private static Vector2? FindDeeperWaterWorld(Vector2 worldPos)
        {
            int centerX = (int)(worldPos.X / 16f);
            int centerY = (int)(worldPos.Y / 16f);
            int minX = Utils.Clamp(centerX - WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int maxX = Utils.Clamp(centerX + WaterSeekHalfWidth, 1, Main.maxTilesX - 2);
            int minY = Utils.Clamp(centerY - 20, 1, Main.maxTilesY - 2);
            int maxY = Utils.Clamp(centerY + WaterSeekHalfHeight + 45, 1, Main.maxTilesY - 2);

            Vector2 best = default;
            float bestScore = float.MinValue;
            bool found = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null || tile.LiquidType != LiquidID.Water || tile.LiquidAmount < 150)
                    {
                        continue;
                    }

                    float depthScore = y - centerY;
                    float distancePenalty = Math.Abs(x - centerX) * 0.22f;
                    float score = depthScore - distancePenalty;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = new Vector2(x * 16f + 8f, y * 16f + 8f);
                        found = true;
                    }
                }
            }

            return found ? best : null;
        }

        private void RegisterDamageTaken(int damageDone)
        {
            if (damageDone <= 0)
            {
                return;
            }

            recentDamageByTick[recentDamageIndex] += damageDone;
            recentDamageTotal += damageDone;

            if (regenTimer <= 0 && recentDamageTotal > DamageToTriggerRegen)
            {
                regenTimer = RegenDurationTicks;
                regenTickTimer = 0;
                NPC.netUpdate = true;
            }
        }

        private void RegisterPlayerDamageSource(int playerIndex, int damageDone)
        {
            if (damageDone <= 0 || playerIndex < 0 || playerIndex >= Main.maxPlayers)
            {
                return;
            }

            playerDamageByTick[playerIndex, attackerDamageIndex] += damageDone;
            playerDamageRecentTotal[playerIndex] += damageDone;
        }

        private void RegisterNpcDamageSource(int npcIndex, int damageDone)
        {
            if (damageDone <= 0 || npcIndex < 0 || npcIndex >= Main.maxNPCs)
            {
                return;
            }

            npcDamageByTick[npcIndex, attackerDamageIndex] += damageDone;
            npcDamageRecentTotal[npcIndex] += damageDone;
        }

        private void AdvanceDamageWindow()
        {
            recentDamageIndex++;
            if (recentDamageIndex >= DamageWindowTicks)
            {
                recentDamageIndex = 0;
            }

            recentDamageTotal -= recentDamageByTick[recentDamageIndex];
            recentDamageByTick[recentDamageIndex] = 0;
            if (recentDamageTotal < 0)
            {
                recentDamageTotal = 0;
            }
        }

        private void AdvanceAttackerDamageWindow()
        {
            attackerDamageIndex++;
            if (attackerDamageIndex >= AttackerDamageWindowTicks)
            {
                attackerDamageIndex = 0;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                int expired = playerDamageByTick[i, attackerDamageIndex];
                if (expired <= 0)
                {
                    continue;
                }

                playerDamageRecentTotal[i] -= expired;
                if (playerDamageRecentTotal[i] < 0)
                {
                    playerDamageRecentTotal[i] = 0;
                }

                playerDamageByTick[i, attackerDamageIndex] = 0;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                int expired = npcDamageByTick[i, attackerDamageIndex];
                if (expired <= 0)
                {
                    continue;
                }

                npcDamageRecentTotal[i] -= expired;
                if (npcDamageRecentTotal[i] < 0)
                {
                    npcDamageRecentTotal[i] = 0;
                }

                npcDamageByTick[i, attackerDamageIndex] = 0;
            }
        }

        private void UpdateDamageRegen()
        {
            if (regenTimer <= 0)
            {
                return;
            }

            regenTimer--;
            regenTickTimer++;

            if (regenTickTimer < RegenTickInterval)
            {
                return;
            }

            regenTickTimer = 0;
            if (NPC.life <= 0 || NPC.life >= NPC.lifeMax)
            {
                return;
            }

            int healAmount = System.Math.Min(RegenHealPerTick, NPC.lifeMax - NPC.life);
            NPC.life += healAmount;
            NPC.HealEffect(healAmount);
            NPC.netUpdate = true;
        }

        private void SpawnFishGore(int goreType, float speedScale)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(3.2f, 3.2f) + new Vector2(0f, -1.2f);
            velocity *= speedScale;

            Gore.NewGore(
                NPC.GetSource_Death(),
                NPC.Center,
                velocity,
                goreType,
                Main.rand.NextFloat(0.9f, 1.15f)
            );
        }

        private void UpdateDeepScreamLoop()
        {
            if (Main.dedServ)
            {
                return;
            }

            if (deepScreamActive)
            {
                if (SoundEngine.TryGetActiveSound(deepScreamSlot, out ActiveSound activeSound))
                {
                    activeSound.Position = NPC.Center;
                    return;
                }

                deepScreamActive = false;
                deepScreamCooldown = DeepScreamRepeatDelayTicks;
            }

            if (deepScreamCooldown > 0)
            {
                deepScreamCooldown--;
                return;
            }

            SoundStyle screamStyle = new SoundStyle("Etobudet1modtipo/Sounds/DeepScreem")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 1
            };

            deepScreamSlot = SoundEngine.PlaySound(screamStyle, NPC.Center);
            if (deepScreamSlot == SlotId.Invalid)
            {
                return;
            }

            deepScreamActive = true;
        }

        private void UpdateDeepSingingChance()
        {
            if (Main.dedServ)
            {
                return;
            }

            if (deepSingingActive)
            {
                if (SoundEngine.TryGetActiveSound(deepSingingSlot, out ActiveSound activeSound))
                {
                    activeSound.Position = NPC.Center;
                    return;
                }

                deepSingingActive = false;
            }

            if (!Main.rand.NextBool(DeepSingingChance))
            {
                return;
            }

            SoundStyle singingStyle = new SoundStyle("Etobudet1modtipo/Sounds/DeepSinging")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 1
            };

            deepSingingSlot = SoundEngine.PlaySound(singingStyle, NPC.Center);
            if (deepSingingSlot == SlotId.Invalid)
            {
                return;
            }

            deepSingingActive = true;
        }

    }
}
