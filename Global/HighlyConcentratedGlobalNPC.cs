using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Global
{
    public class HighlyConcentratedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff(ModContent.BuffType<HighlyConcentratedStrike>()))
            {
                drawColor = new Color(208, 225, 99);
                Lighting.AddLight(npc.Center, 208f / 255f, 225f / 255f, 99f / 255f);
            }
        }
    }
}
