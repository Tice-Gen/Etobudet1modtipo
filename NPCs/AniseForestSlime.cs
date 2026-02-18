using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Gores;
namespace Etobudet1modtipo.NPCs
{
    public class AniseForestSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.BlueSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 24;
            NPC.aiStyle = 1;
            NPC.damage = 10;
            NPC.defense = 5;
            NPC.lifeMax = 25;
            NPC.value = 60f;
            NPC.knockBackResist = 0.9f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;


            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[ModContent.BuffType<HighlyConcentratedStrike>()] = true;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            if (spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneCorrupt && !spawnInfo.Player.ZoneCrimson && !spawnInfo.Player.ZoneHallow)
            {
                return 0.4f;
            }

            return 0f;
        }


        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(5))
            {
                target.AddBuff(ModContent.BuffType<HighlyConcentratedTaste>(), 600);
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AromaticGel>(), 1, 1, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseSoda>(), 5, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HighlyConcentratedAniseSoda>(), 75));
            npcLoot.Add(ItemDropRule.Common(ItemID.StarAnise, 1, 1, 1));
        }


        public override void HitEffect(NPC.HitInfo hit)
{
    if (NPC.life <= 0)
    {

        for (int i = 0; i < 12; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, 0f, 150, default, 1.2f);


        if (Main.netMode != NetmodeID.Server)
        {
            int goreType1 = ModContent.GoreType<AniseSlimeGore_1>();
            int goreType2 = ModContent.GoreType<AniseSlimeGore_2>();


            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-6f, -2f));
                int gIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.position, vel, goreType1);
                if (gIndex >= 0 && gIndex < Main.gore.Length)
                {
                    Main.gore[gIndex].velocity *= Main.rand.NextFloat(0.6f, 1.2f);
                    Main.gore[gIndex].scale = Main.rand.NextFloat(0.9f, 1.15f);
                    Main.gore[gIndex].rotation = Main.rand.NextFloat(-2f, 2f);
                }
            }


            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, 0f));
                int gIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.position, vel, goreType2);
                if (gIndex >= 0 && gIndex < Main.gore.Length)
                {
                    Main.gore[gIndex].velocity *= Main.rand.NextFloat(0.5f, 1.0f);
                    Main.gore[gIndex].scale = Main.rand.NextFloat(0.6f, 0.95f);
                    Main.gore[gIndex].rotation = Main.rand.NextFloat(-3f, 3f);
                }
            }
        }
    }
    else
    {

        for (int i = 0; i < 2; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, 0f, 100, default, 0.9f);
    }
}



        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A cute, viscous slime that visited the Anise Forest and became concentrated!")
            });
        }
    }
}

