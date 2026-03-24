using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Global
{
    public class CosmolFireGlobalNPC : GlobalNPC
    {
        private const int CosmolFireDisplayedDamage = 500;
        private const int CosmolFireDamagePerSecond = 1000;
        private const int CosmolFireLifeRegenPenalty = CosmolFireDamagePerSecond * 2;

        public override bool InstancePerEntity => true;

        public bool cosmolFireActive;

        public override void ResetEffects(NPC npc)
        {
            cosmolFireActive = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!cosmolFireActive || !npc.active || npc.life <= 0 || npc.friendly || npc.dontTakeDamage)
            {
                return;
            }

            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }

            npc.lifeRegen -= CosmolFireLifeRegenPenalty;
            if (damage < CosmolFireDisplayedDamage)
            {
                damage = CosmolFireDisplayedDamage;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!cosmolFireActive)
            {
                return;
            }

            drawColor = Color.Lerp(drawColor, new Color(180, 110, 255), 0.35f);
            Lighting.AddLight(npc.Center, 0.22f, 0.06f, 0.32f);

            if (!Main.rand.NextBool(10))
            {
                return;
            }

            Dust dust = Dust.NewDustDirect(
                npc.position,
                npc.width,
                npc.height,
                DustID.Shadowflame,
                npc.velocity.X * 0.1f,
                -1f,
                120,
                new Color(210, 150, 255),
                1.1f
            );
            dust.noGravity = true;
        }
    }
}
