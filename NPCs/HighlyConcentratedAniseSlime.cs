using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Gores;

namespace Etobudet1modtipo.NPCs
{
    public class HighlyConcentratedAniseSlime : ModNPC
    {
        private int shootTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.BlueSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 24;
            NPC.damage = 10;
            NPC.defense = 5;
            NPC.lifeMax = 50;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;

            NPC.aiStyle = 1;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;


            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[ModContent.BuffType<HighlyConcentratedStrike>()] = true;
        }

        public override void AI()
        {
            shootTimer++;

            if (shootTimer >= 60)
            {
                ShootAniseStingers();
                shootTimer = 0;
            }
        }

        private void ShootAniseStingers()
        {
            Vector2 direction = Vector2.UnitY * -1f;
            float speed = 6f;

            int projectileCount = 4;
            float totalSpread = MathHelper.ToRadians(40f);
            float step = totalSpread / (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = -totalSpread / 2f + step * i;
                Vector2 velocity = direction.RotatedBy(angleOffset) * speed;

                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    velocity,
                    ModContent.ProjectileType<AniseStinger>(),
                    15,
                    1f,
                    Main.myPlayer
                );
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AromaticGel>(), 1, 1, 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.StarAnise, 1, 1, 1));
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDust(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.Grass,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-2f, 2f),
                        150,
                        default,
                        1.3f
                    );
                }


                if (Main.netMode != NetmodeID.Server)
                {
                    int goreBig = ModContent.GoreType<AniseSlimeGore_1>();
                    int goreSmall = ModContent.GoreType<AniseSlimeGore_2>();


                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vel = new Vector2(
                            Main.rand.NextFloat(-3f, 3f),
                            Main.rand.NextFloat(-6f, -2f)
                        );

                        int g = Gore.NewGore(
                            NPC.GetSource_Death(),
                            NPC.position,
                            vel,
                            goreBig
                        );

                        if (g >= 0 && g < Main.gore.Length)
                        {
                            Main.gore[g].scale = Main.rand.NextFloat(0.9f, 1.2f);
                            Main.gore[g].rotation = Main.rand.NextFloat(-2f, 2f);
                        }
                    }


                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vel = new Vector2(
                            Main.rand.NextFloat(-2f, 2f),
                            Main.rand.NextFloat(-4f, 0f)
                        );

                        int g = Gore.NewGore(
                            NPC.GetSource_Death(),
                            NPC.position,
                            vel,
                            goreSmall
                        );

                        if (g >= 0 && g < Main.gore.Length)
                        {
                            Main.gore[g].scale = Main.rand.NextFloat(0.6f, 0.9f);
                            Main.gore[g].rotation = Main.rand.NextFloat(-3f, 3f);
                        }
                    }
                }
            }
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(
                    "A slime oversaturated with concentrated star anise. The pressure inside its body makes it release poisonous clumps."
                )
            });
        }
    }
}

