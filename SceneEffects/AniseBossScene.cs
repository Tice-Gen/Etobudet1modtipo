using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.SceneEffects
{
    public class AniseBossScene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return NPC.AnyNPCs(ModContent.NPCType<AniseKingSlime>());
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Etobudet1modtipo:AniseBossTint", isActive);
        }
    }
}
