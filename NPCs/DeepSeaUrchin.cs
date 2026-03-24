using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Gores;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Common;

namespace Etobudet1modtipo.NPCs
{
    public class DeepSeaUrchin : ModNPC
    {
        private const int FrameCount = 2;
        private const int FrameSize = 59;
        private const int BaseDefense = 300;
        private const int SleepDefenseBonus = 999;
        private const float SleepDamageMultiplier = 0.000001f;
        private const float WakeDefenseDropPerTick = 20f;
        private const float WalkSpeed = 1.1f;
        private const float WalkLerp = 0.08f;
        private const float TwoBlockJumpSpeed = 6.3f;

        private int jumpCooldown;
        private int turnCooldown;
        private int noDamageTimer;
        private int sleepTimer;
        private int nextSleepDelay;
        private bool isSleeping;
        private float currentDefenseBonus;
        private float sleepRegenAccumulator;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = FrameCount;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 46;
            NPC.damage = 50;
            NPC.defense = BaseDefense;
            NPC.lifeMax = 9000;
            NPC.value = Item.buyPrice(silver: 90);
            NPC.knockBackResist = 0.15f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.scale = 1.5f;
            NPC.buffImmune[ModContent.BuffType<Pressure>()] = true;

            nextSleepDelay = Main.rand.Next(600, 901);
            currentDefenseBonus = 0f;
        }

        public override void AI()
        {
            if (NPC.direction == 0)
            {
                NPC.direction = Main.rand.NextBool() ? 1 : -1;
            }

            if (NPC.justHit)
            {
                noDamageTimer = 0;
                nextSleepDelay = Main.rand.Next(600, 901);
                if (isSleeping)
                {
                    WakeUp();
                }
            }

            if (turnCooldown > 0)
            {
                turnCooldown--;
            }

            if (isSleeping)
            {
                currentDefenseBonus = SleepDefenseBonus;
                NPC.defense = BaseDefense + SleepDefenseBonus;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.2f);
                if (!NPC.wet)
                {
                    NPC.velocity.Y += 0.18f;
                }

                if (--sleepTimer <= 0)
                {
                    WakeUp();
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life < NPC.lifeMax)
                {
                    sleepRegenAccumulator += 100f / 60f;
                    int heal = (int)sleepRegenAccumulator;
                    if (heal > 0)
                    {
                        NPC.life = Math.Min(NPC.life + heal, NPC.lifeMax);
                        sleepRegenAccumulator -= heal;
                    }
                }

                NPC.spriteDirection = NPC.direction;
                return;
            }

            if (currentDefenseBonus > 0f)
            {
                currentDefenseBonus -= WakeDefenseDropPerTick;
                if (currentDefenseBonus < 0f)
                {
                    currentDefenseBonus = 0f;
                }
            }

            NPC.defense = BaseDefense + (int)Math.Ceiling(currentDefenseBonus);
            noDamageTimer++;

            bool onGround = NPC.velocity.Y == 0f;
            if (onGround && NPC.wet && noDamageTimer >= nextSleepDelay)
            {
                GoToSleep();
                NPC.spriteDirection = NPC.direction;
                return;
            }

            if (jumpCooldown > 0)
            {
                jumpCooldown--;
            }
            float wantedX = NPC.direction * WalkSpeed;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, wantedX, WalkLerp);
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -WalkSpeed, WalkSpeed);

            if (NPC.wet && !onGround)
            {
                // Keeps the urchin near the sea floor instead of floating up too much.
                NPC.velocity.Y += 0.08f;
            }

            // Do not turn around at tile edges: allow stepping/falling off blocks naturally.
            if (onGround && turnCooldown <= 0 && !HasWaterAhead(NPC.direction))
            {
                TurnAround();
            }

            if (onGround && NPC.collideX)
            {
                if (CanStepUpOneBlock(NPC.direction) && HasWaterAhead(NPC.direction))
                {
                    // 1 block obstacle: crawl up instantly.
                    NPC.position.Y -= 16f;
                    NPC.velocity.Y = -0.1f;
                    NPC.netUpdate = true;
                }
                else if (jumpCooldown <= 0 && HasWaterAhead(NPC.direction))
                {
                    // 2 block obstacle: short jump.
                    NPC.velocity.Y = -TwoBlockJumpSpeed;
                    jumpCooldown = 28;
                    NPC.netUpdate = true;
                }
                else
                {
                    TurnAround();
                }
            }

            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = FrameSize;
            NPC.frame.Height = FrameSize;

            if (isSleeping)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = FrameSize;
                return;
            }

            NPC.frameCounter += Math.Abs(NPC.velocity.X) > 0.05f ? 0.88f : 0.32f;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += FrameSize;
                if (NPC.frame.Y >= FrameSize * FrameCount)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (isSleeping)
            {
                modifiers.FinalDamage *= SleepDamageMultiplier;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0 || Main.dedServ)
            {
                return;
            }

            int goreType1 = ModContent.GoreType<DeepSeaUrchinGore1>();
            int goreType2 = ModContent.GoreType<DeepSeaUrchinGore2>();

            SpawnUrchinGore(goreType1);
            SpawnUrchinGore(goreType2);
            SpawnUrchinGore(goreType2);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!NPC.downedMoonlord || spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0f;
            }

            if (CalamityAbyssHelper.IsCalamityLoaded())
            {
                if (CalamityAbyssHelper.GetAbyssLayer(spawnInfo.Player) != CalamityAbyssLayer.Layer4)
                {
                    return 0f;
                }
            }
            else if (!spawnInfo.Player.ZoneBeach)
            {
                return 0f;
            }

            if (!HasWaterAtSpawn(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
            {
                return 0f;
            }

            if (!HasSolidFloorBelow(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY, 8))
            {
                return 0f;
            }

            float chance = 0.05f;
            if (!Main.dayTime)
            {
                chance *= 1.3f;
            }

            return chance;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeepSeaStone>(), 1, 6, 14));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement("A heavy sea urchin that crawls along the ocean floor and hops over rocks.")
            });
        }

        private static bool HasWaterAtSpawn(int tileX, int tileY)
        {
            for (int y = -1; y <= 3; y++)
            {
                Tile tile = Framing.GetTileSafely(tileX, tileY + y);
                if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount >= 80)
                {
                    return true;
                }
            }

            return false;
        }

        private void SpawnUrchinGore(int goreType)
        {
            Vector2 velocity = new Vector2(
                Main.rand.NextFloat(-2.2f, 2.2f),
                Main.rand.NextFloat(-4.6f, -1.2f)
            );

            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, velocity, goreType, 1f);
        }

        private static bool HasSolidFloorBelow(int tileX, int tileY, int maxDepth)
        {
            for (int y = 1; y <= maxDepth; y++)
            {
                int checkY = tileY + y;
                if (IsWalkableGroundTile(tileX, checkY))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldTurnAtEdge()
        {
            int dir = NPC.direction == 0 ? 1 : NPC.direction;
            Vector2 checkPos = new Vector2(
                NPC.Center.X + dir * (NPC.width * 0.5f + 6f),
                NPC.Bottom.Y + 2f
            );
            Point tile = checkPos.ToTileCoordinates();

            if (tile.X <= 1 || tile.X >= Main.maxTilesX - 1 || tile.Y <= 1 || tile.Y >= Main.maxTilesY - 1)
            {
                return true;
            }

            return !IsWalkableGroundTile(tile.X, tile.Y);
        }

        private bool HasWaterAhead(int direction)
        {
            int dir = direction == 0 ? 1 : direction;
            Vector2 checkPos = new Vector2(
                NPC.Center.X + dir * (NPC.width * 0.5f + 10f),
                NPC.Center.Y - 6f
            );
            Point tile = checkPos.ToTileCoordinates();

            if (tile.X <= 1 || tile.X >= Main.maxTilesX - 1 || tile.Y <= 1 || tile.Y >= Main.maxTilesY - 2)
            {
                return false;
            }

            Tile here = Framing.GetTileSafely(tile.X, tile.Y);
            Tile above = Framing.GetTileSafely(tile.X, tile.Y - 1);
            return here.LiquidType == LiquidID.Water && here.LiquidAmount > 90 &&
                   above.LiquidType == LiquidID.Water && above.LiquidAmount > 40;
        }

        private bool CanStepUpOneBlock(int direction)
        {
            int dir = direction == 0 ? 1 : direction;
            Vector2 forwardOffset = new Vector2(dir * 2f, 0f);
            Vector2 frontBottom = new Vector2(
                NPC.Center.X + dir * (NPC.width * 0.5f + 4f),
                NPC.Bottom.Y - 2f
            );
            Point frontTile = frontBottom.ToTileCoordinates();

            bool hasObstacleAhead =
                Collision.SolidCollision(NPC.position + forwardOffset, NPC.width, NPC.height) ||
                IsWalkableGroundTile(frontTile.X, frontTile.Y) ||
                IsWalkableGroundTile(frontTile.X, frontTile.Y - 1);

            if (!hasObstacleAhead)
            {
                return false;
            }

            Vector2 oneBlockUp = NPC.position + forwardOffset + new Vector2(0f, -16f);
            if (Collision.SolidCollision(oneBlockUp, NPC.width, NPC.height))
            {
                return false;
            }

            Point supportTile = new Vector2(frontBottom.X, frontBottom.Y - 14f).ToTileCoordinates();
            if (IsWalkableGroundTile(supportTile.X, supportTile.Y))
            {
                return true;
            }

            Vector2 supportCheck = oneBlockUp + new Vector2(0f, NPC.height + 1f);
            return Collision.SolidCollision(supportCheck, NPC.width, 2);
        }

        private static bool IsWalkableGroundTile(int x, int y)
        {
            if (x <= 1 || x >= Main.maxTilesX - 1 || y <= 1 || y >= Main.maxTilesY - 1)
            {
                return false;
            }

            Tile tile = Framing.GetTileSafely(x, y);
            if (!tile.HasTile)
            {
                return false;
            }

            if (WorldGen.SolidOrSlopedTile(x, y))
            {
                return true;
            }

            return Main.tileSolidTop[tile.TileType];
        }

        private void TurnAround()
        {
            NPC.direction *= -1;
            turnCooldown = 15;
            NPC.netUpdate = true;
        }

        private void GoToSleep()
        {
            isSleeping = true;
            sleepTimer = Main.rand.Next(600, 1201);
            currentDefenseBonus = SleepDefenseBonus;
            sleepRegenAccumulator = 0f;
            NPC.velocity.X = 0f;
            NPC.netUpdate = true;
        }

        private void WakeUp()
        {
            isSleeping = false;
            sleepTimer = 0;
            noDamageTimer = 0;
            nextSleepDelay = Main.rand.Next(600, 901);
            sleepRegenAccumulator = 0f;
            NPC.netUpdate = true;
        }
    }
}
