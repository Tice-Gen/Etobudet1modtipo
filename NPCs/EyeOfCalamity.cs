using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NPCs
{
    [AutoloadBossHead]
    public class EyeOfCalamity : ModNPC
    {
        private int summonTimer = 0;
        private int bigLaserTimer = 0;


        private int damageTimer = 0;
        private int damageTaken = 0;
        private bool enragedPhase = false;
        private int enragedTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 20;
            NPC.defense = 12;
            NPC.lifeMax = 6000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 10, 50, 0);
            NPC.knockBackResist = 0f;

            NPC.aiStyle = 4;
            AIType = NPCID.EyeofCthulhu;
            AnimationType = -1;
            NPC.boss = true;
            Music = MusicID.Boss2;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];

            if (!target.active || target.dead)
            {
                NPC.TargetClosest(false);
                target = Main.player[NPC.target];
                if (!target.active || target.dead)
                {
                    NPC.velocity.Y -= 10f;
                    return;
                }
            }


            if (!enragedPhase)
            {
                damageTimer++;
                if (damageTimer >= 2400)
                {
                    damageTimer = 0;
                    damageTaken = 0;
                }

                if (damageTaken >= 1000)
                {
                    enragedPhase = true;
                    enragedTimer = 1800;
                    damageTaken = 0;
                    damageTimer = 0;

                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText("Глаз Бедствия впадает в ярость!", 200, 50, 50);
                }
            }
            else
            {
                enragedTimer--;


                NPC.defense = 24;


                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(80f, 80f);
                    int dust = Dust.NewDust(pos, 1, 1, DustID.Enchanted_Gold);
                    Main.dust[dust].velocity *= 0.5f;
                    Main.dust[dust].noGravity = true;
                }

                if (enragedTimer <= 0)
                {
                    enragedPhase = false;
                    NPC.defense = 12;
                }
            }


            summonTimer++;
            int summonInterval = (NPC.life <= NPC.lifeMax * 0.3f) ? 300 : 400;
            if (summonTimer >= summonInterval)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MinionOfCalamity>());
                }
                summonTimer = 0;
            }


            if (NPC.life <= NPC.lifeMax * 0.25f)
            {
                bigLaserTimer++;
                int laserInterval = (NPC.life <= NPC.lifeMax * 0.05f) ? 60 : 100;
                if (bigLaserTimer >= laserInterval)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = target.Center - NPC.Center;
                        direction.Normalize();
                        direction *= 14f;

                        int damage = enragedPhase ? 56 : 35;

                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            direction,
                            ProjectileID.EyeLaser,
                            damage,
                            4f,
                            Main.myPlayer
                        );
                    }
                    bigLaserTimer = 0;
                }
            }
        }


        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!enragedPhase)
                damageTaken += damageDone;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!enragedPhase)
                damageTaken += damageDone;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 8.5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 167;
                if (NPC.frame.Y >= 167 * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void OnKill()
        {

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextFloat() < 0.30f)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<HeartOfCalamity>());

                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.GoldCoin, 5);

                int particleAmount = Main.rand.Next(15, 41);
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<ParticleOfCalamity>(), particleAmount);
            }
        }
    }
}
