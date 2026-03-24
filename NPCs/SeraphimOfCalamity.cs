using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NPCs
{
    [AutoloadBossHead]
    public class SeraphimOfCalamity : ModNPC
    {
        private const float V3SpawnRadius = 500f;
        private int summonTimer;
        private int frameCounter;

        private bool canTeleport = true;
        private bool firstTeleportDone;

        private int phaseTimer;
        private int attackPhase = 1;
        private int circleProj = -1;

        private int specialAttackTimer;
        private int specialAttackCount;
        private bool doingSpecialAttack;

        private bool eyesSummoned;
        private bool doingV3SpawnAttack;
        private int v3AttackTimer;
        private int v3SpawnTimer;
        private readonly int[] eyeProjectiles = { -1, -1, -1 };

        private static readonly string[] TeleportPhrases =
        {
            "You were pulled to Seraphim!",
            "Do not try to run from fate...",
            "There is nowhere to run.",
            "You cannot escape now..."
        };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 600;
            NPC.height = 600;
            NPC.damage = 0;
            NPC.defense = 190;
            NPC.lifeMax = 1000000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(1, 0, 0, 0);

            for (int i = 0; i < NPC.buffImmune.Length; i++)
                NPC.buffImmune[i] = true;
            NPC.buffImmune[ModContent.BuffType<Pressure>()] = false;

            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TheEndTheme");
        }

        public override bool CheckActive() => false;

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target >= Main.maxPlayers)
                NPC.TargetClosest(false);

            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
                return;

            if (!firstTeleportDone)
            {
                player.position = NPC.Center - new Vector2(player.width / 2f, player.height / 2f);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(
                        MessageID.TeleportEntity,
                        -1,
                        -1,
                        null,
                        0,
                        player.whoAmI,
                        player.position.X,
                        player.position.Y,
                        1
                    );
                }

                string phrase = TeleportPhrases[Main.rand.Next(TeleportPhrases.Length)];
                Main.NewText(phrase, 255, 50, 50);
                firstTeleportDone = true;
            }

            phaseTimer++;

            if (attackPhase == 1)
            {
                summonTimer++;
                if (summonTimer >= 120)
                {
                    summonTimer = 0;
                    Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<Projectiles.SeraphimCalamityStar>(),
                        50,
                        5f,
                        Main.myPlayer
                    );
                }

                if (phaseTimer >= Main.rand.Next(600, 901))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        circleProj = Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            player.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<Projectiles.CircleoftheDyingStar>(),
                            0,
                            0f,
                            Main.myPlayer
                        );
                    }

                    attackPhase = 2;
                    phaseTimer = 0;
                }
            }
            else if (attackPhase == 2)
            {
                if (circleProj < 0 || !Main.projectile[circleProj].active)
                {
                    attackPhase = 3;
                    doingSpecialAttack = true;
                    specialAttackTimer = 0;
                    specialAttackCount = 0;
                    phaseTimer = 0;
                }
            }
            else if (attackPhase == 3)
            {
                if (doingSpecialAttack)
                {
                    specialAttackTimer++;

                    if (specialAttackTimer >= 60)
                    {
                        specialAttackTimer = 0;
                        specialAttackCount++;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = MathHelper.ToRadians(45f * i);
                                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 8f;

                                Projectile.NewProjectile(
                                    NPC.GetSource_FromAI(),
                                    NPC.Center,
                                    velocity,
                                    ModContent.ProjectileType<Projectiles.SeraphimCalamityStarV2>(),
                                    50,
                                    2f,
                                    Main.myPlayer,
                                    NPC.Center.X,
                                    NPC.Center.Y
                                );
                            }
                        }
                    }

                    if (specialAttackCount >= 5)
                    {
                        doingSpecialAttack = false;
                        attackPhase = 4;
                        eyesSummoned = false;
                        doingV3SpawnAttack = false;
                        v3AttackTimer = 0;
                        v3SpawnTimer = 0;
                        phaseTimer = 0;
                    }
                }
            }
            else if (attackPhase == 4)
            {
                if (!eyesSummoned)
                {
                    SummonCalamityEyes();
                    eyesSummoned = true;
                }

                if (!doingV3SpawnAttack)
                {
                    if (!AnySummonedEyeAlive())
                    {
                        doingV3SpawnAttack = true;
                        v3AttackTimer = 0;
                        v3SpawnTimer = 0;
                    }
                }
                else
                {
                    v3AttackTimer++;
                    v3SpawnTimer++;

                    if (v3SpawnTimer >= 15)
                    {
                        v3SpawnTimer = 0;
                        SpawnV3NearPlayer(player);
                    }

                    if (v3AttackTimer >= 300)
                    {
                        doingV3SpawnAttack = false;
                        eyesSummoned = false;
                        attackPhase = 1;
                        phaseTimer = 0;
                    }
                }
            }

            float distance = Vector2.Distance(NPC.Center, player.Center);
            if (distance > 1500f && canTeleport)
            {
                player.position = NPC.Center - new Vector2(player.width / 2f, player.height / 2f);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(
                        MessageID.TeleportEntity,
                        -1,
                        -1,
                        null,
                        0,
                        player.whoAmI,
                        player.position.X,
                        player.position.Y,
                        1
                    );
                }

                string phrase = TeleportPhrases[Main.rand.Next(TeleportPhrases.Length)];
                Main.NewText(phrase, 255, 50, 50);

                canTeleport = false;
            }
            else if (distance <= 1500f)
            {
                canTeleport = true;
            }

            frameCounter++;
            if (frameCounter >= 8)
            {
                frameCounter = 0;
                NPC.frame.Y += 600;
                if (NPC.frame.Y >= 2400)
                    NPC.frame.Y = 0;
            }
        }

        private void SummonCalamityEyes()
        {
            Array.Fill(eyeProjectiles, -1);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2[] offsets =
            {
                new Vector2(0f, -260f),
                new Vector2(-280f, -80f),
                new Vector2(280f, -80f)
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                int proj = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center + offsets[i],
                    Vector2.Zero,
                    ModContent.ProjectileType<Projectiles.CalamityEye>(),
                    0,
                    0f,
                    Main.myPlayer
                );

                eyeProjectiles[i] = proj;
            }
        }

        private bool AnySummonedEyeAlive()
        {
            for (int i = 0; i < eyeProjectiles.Length; i++)
            {
                int index = eyeProjectiles[i];
                if (index < 0 || index >= Main.maxProjectiles)
                    continue;

                Projectile p = Main.projectile[index];
                if (!p.active)
                    continue;
                if (p.type != ModContent.ProjectileType<Projectiles.CalamityEye>())
                    continue;

                return true;
            }

            return false;
        }

        private void SpawnV3NearPlayer(Player player)
        {
            float angle = MathHelper.ToRadians(Main.rand.Next(0, 360));
            Vector2 spawnPos = player.Center + angle.ToRotationVector2() * V3SpawnRadius;

            CreateSnowRing(spawnPos);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 direction = player.Center - spawnPos;
            if (direction.LengthSquared() < 0.001f)
                direction = Vector2.UnitY;

            direction.Normalize();

            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                spawnPos,
                direction * 13f,
                ModContent.ProjectileType<Projectiles.SeraphimCalamityStarV3>(),
                50,
                2f,
                Main.myPlayer,
                0f,
                -1f
            );
        }

        private static void CreateSnowRing(Vector2 center)
        {
            const int dustCount = 24;
            const float radius = 40f;

            for (int i = 0; i < dustCount; i++)
            {
                float angle = MathHelper.TwoPi / dustCount * i;
                Vector2 offset = angle.ToRotationVector2() * radius;

                Dust dust = Dust.NewDustPerfect(
                    center + offset,
                    DustID.Snow,
                    Vector2.Zero,
                    0,
                    default,
                    1.2f
                );

                dust.noGravity = true;
                dust.velocity = offset * 0.05f;
            }
        }

        public override void OnKill()
        {
            DyingBlackStar.bossActive = false;
        }
    }
}
