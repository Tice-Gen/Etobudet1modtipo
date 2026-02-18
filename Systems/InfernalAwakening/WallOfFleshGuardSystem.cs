using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.Systems.InfernalAwakening
{
    public class WallOfFleshGuardSystem : ModSystem
    {

        private int messageCooldown = 0;

        public override void PostUpdateWorld()
        {
            if (messageCooldown > 0)
                messageCooldown--;


            if (InfernalAwakeningSystem.IsActive())
                return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active)
                    continue;

                if (npc.type == NPCID.WallofFlesh)
                {

                    npc.active = false;
                    npc.netUpdate = true;


                    RestoreVoodooDoll();


                    if (messageCooldown <= 0 && Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText("", Color.OrangeRed);
                        Main.NewText("✦ The Underworld rejects your offering ✦", new Color(255, 80, 80));
                        Main.NewText("Two ancient guardians still stand.", Color.Orange);
                        Main.NewText("• Guardian of the Dungeon — Skeletron", new Color(180, 200, 255));
                        Main.NewText("• Guardian of the Forest — Anise King Slime", new Color(120, 255, 160));
                        Main.NewText("Only after their defeat may the Infernal Awakening begin.", new Color(255, 120, 120));
                        Main.NewText("", Color.OrangeRed);

                        messageCooldown = 300;
                    }
                }
            }
        }

        private void RestoreVoodooDoll()
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (!player.active) continue;
                if (!player.ZoneUnderworldHeight) continue;

                for (int s = 0; s < 58; s++)
                {
                    Item item = player.inventory[s];

                    if (item.type == ItemID.GuideVoodooDoll)
                    {
                        item.stack++;

                        if (item.stack > item.maxStack)
                            item.stack = item.maxStack;

                        return;
                    }
                }


                Item.NewItem(
                    player.GetSource_GiftOrReward(),
                    player.Hitbox,
                    ItemID.GuideVoodooDoll
                );
            }
        }
    }
}
