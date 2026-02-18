using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Global
{
    public class AngelStatueSpawnGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            if (item == null || item.type != 52)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Main.rand.NextFloat() < 0.01f)
            {
                Vector2 spawn = new Vector2(i * 16 + 16, j * 16 + 32);
                int npcId = NPC.NewNPC(
                    new EntitySource_TileBreak(i, j),
                    (int)spawn.X,
                    (int)spawn.Y,
                    ModContent.NPCType<Statue>()
                );

                if (npcId >= 0)
                    Main.npc[npcId].netUpdate = true;
            }
        }
    }
}
