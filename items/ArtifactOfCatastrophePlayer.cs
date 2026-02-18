using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class ArtifactOfCatastrophePlayer : ModPlayer
    {
        public bool NoHealingFromArtifact;

        private int lifeAtTickStart;
        private int damageTakenThisTick;

        public override void ResetEffects()
        {
            NoHealingFromArtifact = false;
        }

        public override void PreUpdate()
        {
            lifeAtTickStart = Player.statLife;
            damageTakenThisTick = 0;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!NoHealingFromArtifact)
                return;

            damageTakenThisTick += info.Damage;
        }

        public override void UpdateLifeRegen()
        {
            if (!NoHealingFromArtifact)
                return;

            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;

            if (Player.lifeRegenTime > 0)
                Player.lifeRegenTime = 0;
        }

        public override void PostUpdate()
        {
            if (!NoHealingFromArtifact || Player.dead)
                return;

            int maxAllowedLife = lifeAtTickStart - damageTakenThisTick;
            if (maxAllowedLife < 0)
                maxAllowedLife = 0;

            if (Player.statLife > maxAllowedLife)
            {
                Player.statLife = maxAllowedLife;
                NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null, Player.whoAmI, Player.statLife, Player.statMana);
            }
        }
    }
}
