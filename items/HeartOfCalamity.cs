using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class HeartOfCalamity : ModItem
    {
        private int healTimer = 0;



        public override void SetStaticDefaults()
        {

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.consumable = false;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "HeartOfCalamity", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.HeartOfCalamity.HeartOfCalamity")));
        }

        public override void HoldItem(Player player)
{
    if (player.channel)
    {
        healTimer++;
        if (healTimer >= 6)
        {
            int healAmount = 1;

            player.statLife += healAmount;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            if (Main.myPlayer == player.whoAmI)
            {
                CombatText.NewText(
                    player.Hitbox,
                    new Microsoft.Xna.Framework.Color(180, 60, 255),
                    healAmount
                );
            }

            healTimer = 0;
        }
    }
    else
    {
        healTimer = 0;
    }
}

    }
}