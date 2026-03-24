using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.NPCs
{
    public class LongWorm : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Worm];
            Main.npcCatchable[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Worm);
            AIType = NPCID.Worm;
            AnimationType = NPCID.Worm;
            NPC.width *= 2;
            NPC.catchItem = ModContent.ItemType<WormBait>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.dayTime)
            {
                return 0f;
            }

            if (!spawnInfo.Player.ZoneForest || !spawnInfo.Player.ZoneOverworldHeight)
            {
                return 0f;
            }

            return 0.22f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                return;
            }

            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.Worm,
                    hit.HitDirection * 1.4f,
                    -0.4f,
                    120,
                    default,
                    1.2f
                );
                dust.velocity += Main.rand.NextVector2Circular(0.9f, 0.9f);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                new FlavorTextBestiaryInfoElement("A harmless worm that crawls through underground soil.")
            });
        }
    }
}
