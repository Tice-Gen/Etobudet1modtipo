using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.Projectiles;
using Terraria.Audio;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Buffs;
using Terraria.DataStructures;
using System;
using Etobudet1modtipo.Gores;
using Etobudet1modtipo.Systems.InfernalAwakening;

namespace Etobudet1modtipo.NPCs
{
    [AutoloadBossHead]
    public class AniseKingSlime : ModNPC
    {
        private int explosiveTimer = 0;
        private float angleOffset = 0f;

        private int angryTimer = 0;
        private int angryShotCounter = 0;
        private int angryShotInterval = 30;
        private bool shootingAngry = false;


        private int desperationStage = 0;
        private int desperationCounter = 0;

        private bool isFrozen = false;
        private int frozenTimer = 0;

        private int lastHPCheckpoint;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.KingSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 166;
            NPC.height = 111;
            NPC.damage = 12;
            NPC.defense = 6;
            NPC.lifeMax = 3000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 10000f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 15;
            AIType = NPCID.KingSlime;
            AnimationType = NPCID.KingSlime;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            Music = MusicID.Boss1;


            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.buffImmune[ModContent.BuffType<HighlyConcentratedStrike>()] = true;
            NPC.buffImmune[BuffID.Confused] = true;

            if (Main.expertMode)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.2f);
                NPC.damage = (int)(NPC.damage * 1.1f);
            }

            if (Main.masterMode)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.3f);
                NPC.damage = (int)(NPC.damage * 1.1f);
            }

            lastHPCheckpoint = NPC.life;
        }

        public override void OnSpawn(IEntitySource source)
        {
            lastHPCheckpoint = NPC.life;

            desperationStage = 0;
            desperationCounter = 0;
        }

        public override void AI()
        {
            if (isFrozen)
            {
                frozenTimer++;
                if (frozenTimer >= 300)
                {
                    isFrozen = false;
                    frozenTimer = 0;
                }
                return;
            }

            explosiveTimer++;
            if (explosiveTimer >= 60)
            {
                ShootExplosiveAnise();
                explosiveTimer = 0;
                angleOffset += MathHelper.ToRadians(22.5f);
            }

            angryTimer++;
            if (!shootingAngry && angryTimer >= 600)
            {
                shootingAngry = true;
                angryShotCounter = 0;
                angryTimer = 0;


                desperationStage = 0;
                desperationCounter = 0;
            }

            if (shootingAngry)
            {

                bool isDesperationMode = (Main.expertMode || Main.masterMode) && (NPC.life < NPC.lifeMax * 0.35f);

                if (isDesperationMode)
                {

                    if (desperationStage == 0)
                    {
                        if (angryTimer >= 6)
                        {
                            angryTimer = 0;

                            int projectileType = ModContent.ProjectileType<AngryAniseProj>();
                            float speed = 8f;

                            Vector2 targetDirection = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            float baseAngle = targetDirection.ToRotation();
                            float spreadStep = MathHelper.ToRadians(5f);
                            float currentOffset = (desperationCounter - 10) * spreadStep;

                            Vector2 velocity = new Vector2(speed, 0).RotatedBy(baseAngle + currentOffset);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, projectileType, 14, 2f, Main.myPlayer);
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                            desperationCounter++;

                            if (desperationCounter >= 20)
                            {
                                desperationStage = 1;
                                desperationCounter = 0;
                                angryTimer = 0;
                            }
                        }
                    }
                    else if (desperationStage == 1)
                    {
                        int gasType = ModContent.ProjectileType<AngryGas>();
                        float gasSpeed = 4f;
                        int totalGas = 50;

                        Vector2 targetDirection = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        float baseAngle = targetDirection.ToRotation();
                        float spreadStep = MathHelper.ToRadians(3.6f);

                        for (int i = 0; i < totalGas; i++)
                        {
                            float startOffset = -(totalGas / 2f) * spreadStep;
                            float angle = baseAngle + startOffset + (i * spreadStep);

                            Vector2 velocity = new Vector2(gasSpeed, 0).RotatedBy(angle);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, gasType, 10, 2f, Main.myPlayer);
                        }

                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                        desperationStage = 2;
                        desperationCounter = 0;
                        angryTimer = 0;
                    }
                    else if (desperationStage == 2 || desperationStage == 3)
                    {
                        if (angryTimer >= 1)
                        {
                            angryTimer = 0;

                            int gasType = ModContent.ProjectileType<AngryGas>();
                            float gasSpeed = 3f;

                            int shotsPerCircle = 60;
                            float angleStep = MathHelper.TwoPi / shotsPerCircle;

                            float angle = -MathHelper.PiOver2 + (desperationCounter * angleStep);
                            Vector2 velocity = new Vector2(gasSpeed, 0).RotatedBy(angle);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, gasType, 10, 2f, Main.myPlayer);

                            if (desperationCounter % 5 == 0)
                                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                            desperationCounter++;

                            if (desperationCounter >= shotsPerCircle)
                            {
                                if (desperationStage == 2)
                                {
                                    desperationStage = 3;
                                    desperationCounter = 0;
                                }
                                else
                                {
                                    shootingAngry = false;
                                    angryTimer = 0;
                                }
                            }
                        }
                    }
                }
                else
                {

                    if (angryTimer >= angryShotInterval)
                    {
                        ShootAngryAnise();
                        angryShotCounter++;
                        angryTimer = 0;

                        if (angryShotCounter >= 10)
                        {
                            shootingAngry = false;
                            isFrozen = true;
                            frozenTimer = 0;

                            int gasType = ModContent.ProjectileType<AngryGas>();
                            float speed = 5f;
                            int gasCount = 112;
                            float angleStep = MathHelper.TwoPi / gasCount;

                            for (int i = 0; i < gasCount; i++)
                            {
                                float angle = angleStep * i;
                                Vector2 velocity = new Vector2(speed, 0).RotatedBy(angle);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, gasType, 2, 2f, Main.myPlayer);
                            }
                        }
                    }
                }
            }


            while (lastHPCheckpoint - NPC.life >= 500)
            {
                lastHPCheckpoint -= 500;

                Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-50, 51));
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<HighlyConcentratedAniseSlime>());

                int forestSlimeCount = Main.rand.Next(2, 3);
                for (int i = 0; i < forestSlimeCount; i++)
                {
                    Vector2 forestPos = NPC.Center + new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-50, 51));
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)forestPos.X, (int)forestPos.Y, ModContent.NPCType<AniseForestSlime>());
                }
            }

            base.AI();
        }

        private void ShootExplosiveAnise()
        {
            int projectileType = ModContent.ProjectileType<ExplosiveAnise>();
            float speed = 10f;

            for (int i = 0; i < 8; i++)
            {
                float rotation = MathHelper.ToRadians(45 * i) + angleOffset;
                Vector2 velocity = new Vector2(speed, 0).RotatedBy(rotation);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, projectileType, 12, 2f, Main.myPlayer);
            }

            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
        }
        public override void OnKill()
{
    var system = InfernalAwakeningSystem.Instance;

	    if (system == null)
	        return;

	    system.AniseKingSlimeDefeated = true;

	    if (!system.AniseDefeated)
	    {
        system.AniseDefeated = true;
        system.TryActivateInfernal();
    }
}


        private void ShootAngryAnise()
        {
            int projectileType = ModContent.ProjectileType<AngryAniseProj>();
            float speed = 12f;

            Vector2 targetDirection = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 velocity = targetDirection * speed;

            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, projectileType, 10, 2f, Main.myPlayer);
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AniseKingSlimeBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseInfusion>(), 1, 15, 20));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AromaticGel>(), 1, 50, 100));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseRifle>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HeartOfAniseForest>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseForestBar>(), 1, 30, 50));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseSoda>(), 2, 5, 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseInfusion>(), 1, 10, 15));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AromaticHalo>(), 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseBombard>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseGlove>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseForestHelmet>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseForestBreastplate>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseForestLeggings>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PoisonGun>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TameAngryAnise>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AniseForestSeeds>(), 1, 20, 25));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.StarAnise, 1, 100, 150));

            npcLoot.Add(notExpertRule);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(
                    "Once an ordinary star anise, it was trapped in aromatic gel and gained sentience over time. Now it rules the Anise Forest and fiercely attacks anyone who dares disturb its domain."
                )
            });
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

            target.AddBuff(ModContent.BuffType<HighlyConcentratedTaste>(), 15 * 60);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {

                int goreTypeSmall = ModContent.GoreType<DeadAniseKing1>();
                int goreTypeBig = ModContent.GoreType<DeadAniseKing2>();


                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-10f, -3f));
                    int idx = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, vel, goreTypeSmall, 1f);
                    if (idx >= 0 && idx < Main.gore.Length)
                    {
                        Main.gore[idx].velocity = vel;
                        Main.gore[idx].rotation = Main.rand.NextFloat(-0.6f, 0.6f);
                        Main.gore[idx].scale = Main.rand.NextFloat(0.85f, 1.05f);
                    }
                }


                Vector2 velBig = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-8f, -2f));
                int idxBig = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, velBig, goreTypeBig, 1f);
                if (idxBig >= 0 && idxBig < Main.gore.Length)
                {
                    Main.gore[idxBig].velocity = velBig;
                    Main.gore[idxBig].rotation = Main.rand.NextFloat(-0.3f, 0.3f);
                    Main.gore[idxBig].scale = Main.rand.NextFloat(0.95f, 1.25f);
                }
            }
        }
    }
}

