using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Etobudet1modtipo.Gores;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NPCs
{
    [AutoloadBossHead]
    public class TheHungriest : ModNPC
    {
        const int STATE_DASH = 0;
        const int STATE_VOMIT = 1;
        const int STATE_REPOSITION = 2;
        const int STATE_HOVER = 3;

        const float DASH_SPEED = 22f;
        const int DASH_TIME = 22;

        const float REPOSITION_SPEED = 14f;
        const int HOVER_TIME = 40;

        const int VOMIT_PROJECTILE = 811;
        const int VOMIT_COUNT = 14;
        const float VOMIT_SPREAD = 1.9f;

        const float PROJECTILE_RANGE = 260f;
        const float DODGE_STRENGTH = 3.6f;
        const float DODGE_CHANCE = 0.40f;
        const int SCAN_INTERVAL = 6;
        const int DODGE_MIN_TIME = 12;
        const int DODGE_MAX_TIME = 20;

        const int FRAME_COUNT = 6;
        const int FRAME_HEIGHT = 50;

        Vector2 pendingDodge;
        int dodgeTimer;
        int scanCooldown;

        Vector2 dashVelocity;
        Vector2 repositionTarget;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = FRAME_COUNT;
            NPCID.Sets.TrailCacheLength[NPC.type] = 14;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 50;
            NPC.scale = 3f;

            NPC.damage = 66;
            NPC.defense = 12;
            NPC.lifeMax = 5000;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;

            NPC.knockBackResist = 0f;

            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath13;
        }




        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            int speed =
                NPC.ai[0] == STATE_DASH ? 3 :
                NPC.ai[0] == STATE_VOMIT ? 5 : 7;

            if (NPC.frameCounter >= speed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += FRAME_HEIGHT;
                if (NPC.frame.Y >= FRAME_HEIGHT * FRAME_COUNT)
                    NPC.frame.Y = 0;
            }
        }




        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
                return;

            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            if (scanCooldown-- <= 0)
            {
                scanCooldown = SCAN_INTERVAL;

                Vector2 avoid = ScanProjectiles();
                if (avoid != Vector2.Zero && Main.rand.NextFloat() < DODGE_CHANCE)
                {
                    pendingDodge = avoid * DODGE_STRENGTH;
                    dodgeTimer = Main.rand.Next(DODGE_MIN_TIME, DODGE_MAX_TIME);
                }
            }

            if (dodgeTimer > 0 && --dodgeTimer <= 0)
                pendingDodge = Vector2.Zero;

            switch ((int)NPC.ai[0])
            {
                case STATE_DASH:
                    if (NPC.ai[1] == 0)
                    {
                        dashVelocity =
                            (player.Center - NPC.Center)
                            .SafeNormalize(Vector2.UnitX)
                            * DASH_SPEED;
                        NPC.velocity = dashVelocity;
                    }

                    NPC.ai[1]++;

                    Dust d = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.Blood,
                        -NPC.velocity.X * 0.3f,
                        -NPC.velocity.Y * 0.3f,
                        120,
                        default,
                        1.6f);
                    d.noGravity = true;

                    if (NPC.ai[1] >= DASH_TIME)
                    {
                        NPC.ai[0] = STATE_VOMIT;
                        NPC.ai[1] = 0;
                        NPC.velocity *= 0.4f;
                    }
                    break;

                case STATE_VOMIT:
                    NPC.velocity *= 0.92f;
                    NPC.ai[1]++;

                    if (NPC.ai[1] == 25)
                        Vomit(player);

                    if (NPC.ai[1] >= 55)
                    {
                        float side = Main.rand.NextBool() ? -1f : 1f;
                        repositionTarget = player.Center + new Vector2(side * 170f, -170f);
                        NPC.ai[0] = STATE_REPOSITION;
                        NPC.ai[1] = 0;
                    }
                    break;

                case STATE_REPOSITION:
                    Vector2 dir = repositionTarget - NPC.Center;
                    if (dir.Length() > 8f)
                    {
                        dir.Normalize();
                        NPC.velocity = Vector2.Lerp(
                            NPC.velocity,
                            dir * REPOSITION_SPEED,
                            0.18f);
                    }
                    else
                    {
                        NPC.velocity *= 0.6f;
                        NPC.ai[0] = STATE_HOVER;
                        NPC.ai[1] = 0;
                    }
                    break;

                case STATE_HOVER:
                    NPC.velocity *= 0.92f;
                    if (++NPC.ai[1] >= HOVER_TIME)
                    {
                        NPC.ai[0] = STATE_DASH;
                        NPC.ai[1] = 0;
                    }
                    break;
            }

            if (pendingDodge != Vector2.Zero)
                NPC.velocity += pendingDodge * 0.08f;

            NPC.rotation = NPC.velocity.X * 0.015f;
        }




        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 2, 2));
            npcLoot.Add(ItemDropRule.Common(
                ModContent.ItemType<LivingBlade>(),
                1,
                1,
                1));
        }




        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < 180; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.Blood,
                    Main.rand.NextFloat(-14f, 14f),
                    Main.rand.NextFloat(-14f, 14f),
                    80,
                    default,
                    Main.rand.NextFloat(1.4f, 2.8f)
                );
                dust.noGravity = Main.rand.NextBool(3);
            }

            for (int i = 0; i < 120; i++)
            {
                Dust.NewDust(
                    NPC.Center,
                    2,
                    2,
                    DustID.Blood,
                    Main.rand.NextFloat(-9f, 9f),
                    Main.rand.NextFloat(-9f, 9f),
                    120,
                    default,
                    Main.rand.NextFloat(0.6f, 1.2f)
                );
            }

            for (int i = 0; i < 3; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center,
                    Main.rand.NextVector2Circular(8f, 8f), 140, 1.3f);

            for (int i = 0; i < 6; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center,
                    Main.rand.NextVector2Circular(10f, 10f), 137, 1.1f);

            for (int i = 0; i < 10; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center,
                    Main.rand.NextVector2Circular(12f, 12f), 142, 0.9f);
        }




        void Vomit(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCHit6, NPC.Center);

            Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            float baseRot = dir.ToRotation();

            for (int i = 0; i < VOMIT_COUNT; i++)
            {
                float t = (float)i / (VOMIT_COUNT - 1);
                float rot = baseRot - VOMIT_SPREAD / 2f + VOMIT_SPREAD * t;

                Vector2 vel = new Vector2(9f, 0).RotatedBy(rot);

                int p = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    vel,
                    VOMIT_PROJECTILE,
                    0,
                    1f,
                    Main.myPlayer
                );

                Projectile proj = Main.projectile[p];
                proj.hostile = true;
                proj.friendly = false;
                proj.damage = 35;
                proj.netUpdate = true;
            }
        }




        Vector2 ScanProjectiles()
        {
            Vector2 result = Vector2.Zero;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || !p.friendly || p.damage <= 0)
                    continue;

                float dist = Vector2.Distance(p.Center, NPC.Center);
                if (dist > PROJECTILE_RANGE)
                    continue;

                Vector2 dir = p.velocity.SafeNormalize(Vector2.Zero);
                Vector2 toNPC = (NPC.Center - p.Center).SafeNormalize(Vector2.Zero);

                if (Vector2.Dot(dir, toNPC) < 0.65f)
                    continue;

                Vector2 perp = new Vector2(-dir.Y, dir.X);
                if (Main.rand.NextBool()) perp = -perp;

                result += perp;
            }

            return result.LengthSquared() > 0.01f
                ? Vector2.Normalize(result)
                : Vector2.Zero;
        }




        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement(
                    "!"
                )
            });
        }
    }
}
