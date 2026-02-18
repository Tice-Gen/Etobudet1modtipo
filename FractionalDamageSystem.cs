using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Etobudet1modtipo
{
    public static class FractionalDamage
    {
        public static void AddToNPC(NPC npc, float amount)
        {
            if (npc == null || !npc.active) return;
            npc.GetGlobalNPC<FractionalDamageGlobalNPC>().accumulator += amount;
        }

        public static void AddToPlayer(Player player, float amount)
        {
            if (player == null || !player.active) return;
            player.GetModPlayer<FractionalDamagePlayer>().accumulator += amount;
        }
    }

    public class FractionalDamageGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public float accumulator = 0f;

        public override void AI(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (accumulator >= 1f)
            {
                int toApply = (int)accumulator;
                accumulator -= toApply;

                npc.life -= toApply;
                if (npc.life < 0) npc.life = 0;

                CombatText.NewText(npc.Hitbox, Color.Orange, toApply, dramatic: false, dot: true);
                npc.checkDead();
            }
        }
    }

    public class FractionalDamagePlayer : ModPlayer
    {
        public float accumulator = 0f;

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            if (accumulator >= 1f)
            {
                int toApply = (int)accumulator;
                accumulator -= toApply;

                Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} succumbed to fractional damage."), toApply, 0);
            }
        }


        public override void ResetEffects() { }
    }
}
