using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Biomes;
using Terraria.GameContent.Bestiary;
using Etobudet1modtipo.Systems.InfernalAwakening;

namespace Etobudet1modtipo.NPCs
{
    public class AngelAnise : ModNPC
    {











        const float BASE_ATTACK_SPEED = 6.2f;
        const float RETREAT_SPEED = 10.5f;
        const int RETREAT_TIME = 45;


        bool spawnedGroup = false;


        const float PROJECTILE_DETECT_RANGE = 220f;
        const float BASE_DODGE_STRENGTH = 2.8f;
        const float BASE_DODGE_CHANCE = 0.45f;
        const int BASE_PROJECTILE_SCAN_INTERVAL = 6;
        const int PENDING_DODGE_DURATION_MIN = 10;
        const int PENDING_DODGE_DURATION_MAX = 18;


        const float ENRAGE_HP_RATIO = 0.35f;
        const float ENRAGE_DODGE_MULT = 1.6f;
        const float ENRAGE_ATTACK_MULT = 1.15f;
        const float ENRAGE_SCAN_INTERVAL_DIV = 2f;
        const int ALLY_BUFF_DURATION = 600;
        const float ALLY_BUFF_DODGE_MULT = 1.15f;
        const float ALLY_BUFF_ATTACK_MULT = 1.08f;
        const float ALLY_BUFF_RANGE = 520f;


        Vector2 pendingDodge = Vector2.Zero;
        int pendingDodgeTimer = 0;
        int projectileScanCooldown = 0;


        bool enraged = false;
        int allyBuffTimer = 0;
        Vector2 lastAppliedEnragePos = Vector2.Zero;


        int healTicker = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;

            NPC.damage = 18;
            NPC.defense = 6;
            NPC.lifeMax = 140;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.3f;

            NPC.value = 150f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }

        public override void AI()
        {

            if (!spawnedGroup && Main.netMode != NetmodeID.MultiplayerClient)
            {
                spawnedGroup = true;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = new Vector2(
                        Main.rand.NextFloat(-180f, 180f),
                        Main.rand.NextFloat(-100f, 100f)
                    );

                    int id = NPC.NewNPC(
                        NPC.GetSource_FromAI(),
                        (int)(NPC.Center.X + offset.X),
                        (int)(NPC.Center.Y + offset.Y),
                        NPC.type
                    );

                    if (id >= 0 && id < Main.maxNPCs)
                    {
                        NPC clone = Main.npc[id];
                        clone.velocity = new Vector2(
                            Main.rand.NextFloat(-3f, 3f),
                            Main.rand.NextFloat(-2f, 2f)
                        );


                        if (clone.ModNPC is AngelAnise a)
                            a.spawnedGroup = true;
                    }
                }

                NPC.netUpdate = true;
            }



            healTicker++;

            if (healTicker >= 30)
            {
                healTicker = 0;

                if (NPC.life > 0 && NPC.life < NPC.lifeMax)
                {
                    int actualHealed = Math.Min(5, NPC.lifeMax - NPC.life);

                    NPC.life += actualHealed;

                    CombatText.NewText(
                        NPC.Hitbox,
                        Color.LightGreen,
                        actualHealed
                    );

                    NPC.netUpdate = true;
                }
            }


            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                NPC.velocity.Y += 0.2f;
                return;
            }


            if (!enraged && NPC.life < NPC.lifeMax * ENRAGE_HP_RATIO)
            {
                enraged = true;


                pendingDodge = pendingDodge == Vector2.Zero ? new Vector2(NPC.direction, 0f) * BASE_DODGE_STRENGTH * 0.9f : pendingDodge * 1.2f;
                pendingDodgeTimer = Math.Max(pendingDodgeTimer, 12);


                int whoAmI = NPC.whoAmI;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == whoAmI) continue;
                    NPC other = Main.npc[i];
                    if (other == null || !other.active) continue;
                    if (other.type != NPC.type) continue;

                    if (Vector2.Distance(other.Center, NPC.Center) <= ALLY_BUFF_RANGE)
                    {
                        if (other.ModNPC is AngelAnise ally)
                        {
                            ally.GrantAllyBuff(ALLY_BUFF_DURATION);
                            other.netUpdate = true;
                        }
                    }
                }

                NPC.netUpdate = true;
            }


            if (allyBuffTimer > 0)
                allyBuffTimer--;

            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;


            if (float.IsNaN(NPC.velocity.X) || float.IsInfinity(NPC.velocity.X))
                NPC.velocity = Vector2.Zero;


            NPC.localAI[2] += 0.08f;


            float sidestepBaseChance = 0.008f;
            if (enraged) sidestepBaseChance *= 1.8f;
            if (allyBuffTimer > 0) sidestepBaseChance *= 1.12f;

            if (NPC.localAI[0] <= 0f && Main.rand.NextFloat() < sidestepBaseChance)
            {
                NPC.localAI[0] = Main.rand.Next(30, 80);

                NPC.localAI[1] = Main.rand.NextBool() ? 1f : -1f;
            }



            float currentDodgeChance = BASE_DODGE_CHANCE * (enraged ? ENRAGE_DODGE_MULT : 1f) * (allyBuffTimer > 0 ? ALLY_BUFF_DODGE_MULT : 1f);
            float currentDodgeStrength = BASE_DODGE_STRENGTH * (enraged ? 1.2f : 1f) * (allyBuffTimer > 0 ? 1.1f : 1f);
            int scanInterval = (int)Math.Max(1, BASE_PROJECTILE_SCAN_INTERVAL / (enraged ? ENRAGE_SCAN_INTERVAL_DIV : 1f));

            if (projectileScanCooldown <= 0)
            {
                projectileScanCooldown = scanInterval;
                Vector2 avoidance = GetProjectileAvoidanceVector();

                if (avoidance.LengthSquared() > 0.0001f && Main.rand.NextFloat() < currentDodgeChance)
                {
                    pendingDodge = Vector2.Normalize(avoidance) * currentDodgeStrength;
                    pendingDodgeTimer = Main.rand.Next(PENDING_DODGE_DURATION_MIN, PENDING_DODGE_DURATION_MAX + 1);
                }
            }
            else
            {
                projectileScanCooldown--;
            }

            if (pendingDodgeTimer > 0)
            {
                pendingDodgeTimer--;
                if (pendingDodgeTimer == 0)
                    pendingDodge = Vector2.Zero;
            }


            float attackSpeedMultiplier = 1f;
            if (enraged) attackSpeedMultiplier *= ENRAGE_ATTACK_MULT;
            if (allyBuffTimer > 0) attackSpeedMultiplier *= ALLY_BUFF_ATTACK_MULT;
            float currentAttackSpeed = BASE_ATTACK_SPEED * attackSpeedMultiplier;


            if (NPC.ai[0] == 0f)
            {
                if (NPC.localAI[0] > 0f)
                    NPC.localAI[0]--;

                float dist = Vector2.Distance(NPC.Center, player.Center);
                float lead = MathHelper.Clamp(dist / 400f, 0f, 0.55f);

                Vector2 predicted = player.Center + player.velocity * lead;
                Vector2 dir = predicted - NPC.Center;

                if (dir.LengthSquared() < 0.001f)
                    dir = new Vector2(NPC.direction, 0f);

                dir.Normalize();

                Vector2 perp = new Vector2(-dir.Y, dir.X);

                float sway = (float)Math.Sin(NPC.localAI[2]) * 0.45f * MathHelper.Clamp(dist / 200f, 0.3f, 1.2f);
                Vector2 lateral = perp * sway;

                if (NPC.localAI[0] > 0f)
                    lateral += perp * NPC.localAI[1] * 1.6f;


                Vector2 dodge = pendingDodge;

                if (dodge.LengthSquared() > 0.001f)
                    dodge = Vector2.Lerp(Vector2.Zero, dodge, 0.9f);

                Vector2 desired = dir + lateral + dodge;

                if (desired.LengthSquared() < 0.001f)
                    desired = dir;

                desired.Normalize();

                NPC.velocity = Vector2.Lerp(
                    NPC.velocity,
                    desired * currentAttackSpeed,
                    0.12f
                );
            }

            else
            {
                NPC.ai[1]++;

                NPC.velocity *= 1.06f;

                if (NPC.velocity.Length() > RETREAT_SPEED)
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * RETREAT_SPEED;

                NPC.velocity.Y -= 0.04f;

                if (NPC.ai[1] >= RETREAT_TIME)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                }
            }

            NPC.rotation = NPC.velocity.X * 0.04f;
        }


        Vector2 GetProjectileAvoidanceVector()
        {
            Vector2 dodge = Vector2.Zero;
            float bestWeight = 0f;


            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p == null || !p.active)
                    continue;


                if (!p.friendly || p.damage <= 0)
                    continue;


                if (p.velocity.LengthSquared() < 0.25f)
                    continue;

                float dist = Vector2.Distance(p.Center, NPC.Center);
                if (dist > PROJECTILE_DETECT_RANGE)
                    continue;

                Vector2 projDir = p.velocity;
                if (projDir.LengthSquared() < 0.0001f)
                    continue;
                projDir.Normalize();

                Vector2 toNPC = NPC.Center - p.Center;
                if (toNPC.LengthSquared() < 0.0001f)
                    continue;
                toNPC.Normalize();


                float dot = Vector2.Dot(projDir, toNPC);
                if (dot <= 0.65f)
                    continue;


                float speedFactor = MathHelper.Clamp(p.velocity.Length() / 12f, 0.6f, 2.0f);
                float closeness = 1f - (dist / PROJECTILE_DETECT_RANGE);
                float weight = closeness * dot * speedFactor;


                Vector2 perp = new Vector2(-projDir.Y, projDir.X);


                if (Main.rand.NextFloat() < 0.5f)
                    perp = -perp;

                Vector2 candidate = perp * weight;


                dodge += candidate;
                bestWeight = Math.Max(bestWeight, weight);
            }

            if (dodge.LengthSquared() > 0.0001f)
                return Vector2.Normalize(dodge);

            return Vector2.Zero;
        }


        public void GrantAllyBuff(int duration)
        {
            allyBuffTimer = Math.Max(allyBuffTimer, duration);

            NPC.netUpdate = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Vector2 away = NPC.Center - target.Center;

            if (away.LengthSquared() < 0.01f)
                away = new Vector2(NPC.direction, -0.5f);

            away.Normalize();

            NPC.velocity = away * 4f;


            pendingDodge = Vector2.Zero;
            pendingDodgeTimer = 0;

            NPC.localAI[0] = 0f;
            NPC.ai[0] = 1f;
            NPC.ai[1] = 0f;

            NPC.netUpdate = true;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            var system = InfernalAwakeningSystem.Instance;
            if (system == null || !system.AniseDefeated)
                return 0f;

            if (spawnInfo.Player == null)
                return 0f;

            bool forest =
                spawnInfo.Player.ZoneForest &&
                !spawnInfo.Player.ZoneCorrupt &&
                !spawnInfo.Player.ZoneCrimson &&
                !spawnInfo.Player.ZoneHallow &&
                !spawnInfo.Player.ZoneJungle &&
                !spawnInfo.Player.ZoneSnow &&
                !spawnInfo.Player.ZoneDesert;

            bool aniseForest =
                spawnInfo.Player.InModBiome<AniseForestBiome>();

            return (forest || aniseForest) ? 0.08f : 0f;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.Grass,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f)
                );
        }
    }
}
