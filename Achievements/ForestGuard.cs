using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Achievements
{
    public class ForestGuard : ModAchievement
    {
        private CustomFlagCondition defeatAniseKingSlimeCondition;

        public override void SetStaticDefaults()
        {
            defeatAniseKingSlimeCondition = AddCondition("DefeatAniseKingSlime");
        }

        public void Complete()
        {
            defeatAniseKingSlimeCondition?.Complete();
        }
    }
}
