using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Achievements
{
    public class Orbit : ModAchievement
    {
        private CustomFlagCondition craftAllOrbitalYoyosCondition;

        public override void SetStaticDefaults()
        {
            craftAllOrbitalYoyosCondition = AddCondition("CraftAllOrbitalYoyos");
        }

        public void Complete()
        {
            craftAllOrbitalYoyosCondition?.Complete();
        }
    }
}
