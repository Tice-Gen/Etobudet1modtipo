using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Systems.InfernalAwakening
{
    public class InfernalAwakeningSystem : ModSystem
    {
        public static InfernalAwakeningSystem Instance;

        public bool InfernalActive;
        public bool SkeletronDefeated;
        public bool AniseDefeated;
        public bool AniseKingSlimeDefeated;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override void PostUpdateWorld()
        {

            if (!SkeletronDefeated && NPC.downedBoss3)
            {
                SkeletronDefeated = true;
                TryActivateInfernal();
            }


            if (!AniseDefeated)
            {
                int aniseType = ModContent.NPCType<NPCs.AniseKingSlime>();


                if (!NPC.AnyNPCs(aniseType))
                {

                    if (NPC.downedSlimeKing)
                    {
                        AniseDefeated = true;
                        TryActivateInfernal();
                    }
                }
            }
        }

        public void TryActivateInfernal()
        {
            if (InfernalActive)
                return;

            if (!SkeletronDefeated || !AniseDefeated)
                return;

            InfernalActive = true;

            if (Main.netMode != NetmodeID.Server)
                Main.NewText("Infernal Awakening has begun.", 255, 120, 140);

            ReplaceDormantWithAwakened();
        }

        private void ReplaceDormantWithAwakened()
        {
            int dormantType = ModContent.ItemType<ObsidianDemonicScythe>();
            int awakenedType = ModContent.ItemType<ObsidianDemonicScytheAwakened>();


            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (!player.active) continue;

                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item it = player.inventory[i];
                    if (it.type != dormantType) continue;

                    int stack = it.stack;
                    byte prefix = (byte)it.prefix;

                    it.SetDefaults(awakenedType);
                    it.stack = stack;
                    it.Prefix(prefix);
                }
            }


            for (int i = 0; i < Main.maxItems; i++)
            {
                Item it = Main.item[i];
                if (!it.active || it.type != dormantType) continue;

                int stack = it.stack;
                byte prefix = (byte)it.prefix;

                it.SetDefaults(awakenedType);
                it.stack = stack;
                it.Prefix(prefix);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["InfernalActive"] = InfernalActive;
            tag["SkeletronDefeated"] = SkeletronDefeated;
            tag["AniseDefeated"] = AniseDefeated;
            tag["AniseKingSlimeDefeated"] = AniseKingSlimeDefeated;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            InfernalActive = tag.GetBool("InfernalActive");
            SkeletronDefeated = tag.GetBool("SkeletronDefeated");
            AniseDefeated = tag.GetBool("AniseDefeated");
            AniseKingSlimeDefeated = tag.GetBool("AniseKingSlimeDefeated");
        }

        public static bool IsActive()
        {
            return Instance != null && Instance.InfernalActive;
        }
    }
}
