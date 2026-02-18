using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Systems
{
    public class StarBurnSystem : ModSystem
    {
        public static bool AnyHostileNpcHasStarBurn { get; private set; }

        public override void PostUpdateNPCs()
        {
            AnyHostileNpcHasStarBurn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                {
                    continue;
                }

                if (npc.HasBuff(ModContent.BuffType<StarBurn>()))
                {
                    AnyHostileNpcHasStarBurn = true;
                    break;
                }
            }
        }

        public override void OnWorldUnload()
        {
            AnyHostileNpcHasStarBurn = false;
        }
    }

    public class StarBurnPlayer : ModPlayer
    {
        public override void PostUpdateRunSpeeds()
        {
            if (StarBurnSystem.AnyHostileNpcHasStarBurn)
            {
                Player.moveSpeed += 0.5f;
            }
        }
    }
}
