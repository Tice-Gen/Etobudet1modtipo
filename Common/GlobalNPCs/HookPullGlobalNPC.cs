using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Common.GlobalNPCs
{
    public class HookPullGlobalNPC : GlobalNPC
    {
        private const float TrackDistance = 96f;
        private const int MaxTicks = 240;

        private readonly int[] proximityTicks = new int[Main.maxPlayers];

        public override bool InstancePerEntity => true;

        public override void AI(NPC npc)
        {
            if (!npc.active || npc.friendly || npc.townNPC)
            {
                ResetAll();
                return;
            }

            int hookType = ModContent.ItemType<items.Hook>();

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player == null || !player.active || player.dead)
                {
                    proximityTicks[i] = 0;
                    continue;
                }

                bool usingHook = player.HeldItem != null && player.HeldItem.type == hookType;
                if (!usingHook)
                {
                    proximityTicks[i] = 0;
                    continue;
                }

                float dist = npc.Distance(player.Center);
                if (dist <= TrackDistance)
                {
                    proximityTicks[i]++;
                    if (proximityTicks[i] > MaxTicks)
                    {
                        proximityTicks[i] = MaxTicks;
                    }
                }
                else if (proximityTicks[i] > 0)
                {
                    proximityTicks[i]--;
                }
            }
        }

        public int GetProximityTicks(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= proximityTicks.Length)
            {
                return 0;
            }

            return proximityTicks[playerIndex];
        }

        private void ResetAll()
        {
            for (int i = 0; i < proximityTicks.Length; i++)
            {
                proximityTicks[i] = 0;
            }
        }
    }
}
