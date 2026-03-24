using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Achievements
{
    public class AnimalsSaver : ModAchievement
    {
        private CustomFlagCondition rescueCondition;

        public override void SetStaticDefaults()
        {
            rescueCondition = AddCondition("Rescue");
        }

        public void Complete()
        {
            rescueCondition?.Complete();
        }
    }
}
