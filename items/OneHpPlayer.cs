using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class OneHpPlayer : ModPlayer
    {
        public bool oneHpEquipped;

        public override void ResetEffects()
        {
            oneHpEquipped = false;
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return new[] {
                new Item(ModContent.ItemType<OneHpChallenge>())
            };
        }

        public override void PostUpdateEquips()
        {
            int itemType = ModContent.ItemType<OneHpChallenge>();


            for (int i = 0; i < Player.armor.Length; i++)
            {
                Item item = Player.armor[i];

                if (!item.IsAir && item.type == itemType)
                {
                    oneHpEquipped = true;
                    break;
                }
            }


            if (!oneHpEquipped)
                return;

            Player.statLifeMax2 = 1;
            Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;

            if (Player.statLife > 1)
                Player.statLife = 1;
        }
    }
}
