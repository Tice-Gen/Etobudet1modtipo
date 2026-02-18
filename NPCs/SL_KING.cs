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
    [AutoloadBossHead]
    public class SL_KING : ModNPC
    {
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.KingSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 100;
            NPC.aiStyle = 1;
            NPC.damage = 10;
            NPC.defense = 5;
            NPC.lifeMax = 250000;
            NPC.value = 60f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AIType = NPCID.KingSlime;
            AnimationType = NPCID.KingSlime;
            NPC.scale = 2;
            NPC.aiStyle = 15;
            NPC.knockBackResist = 0f;


            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[ModContent.BuffType<HighlyConcentratedStrike>()] = true;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gearanise>(), 1, 1, 1));

        }



        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("!")
            });
        }
    }
}
