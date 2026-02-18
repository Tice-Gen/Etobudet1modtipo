using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Projectiles;
using Terraria.Utilities;

namespace Etobudet1modtipo.items
{
    public class TameAngryAnise : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.value = Item.buyPrice(silver: 50);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44;

            Item.noMelee = true;
            Item.accessory = true;
            Item.buffType = ModContent.BuffType<TameAngryAniseBuff>(); 
            Item.shoot = ModContent.ProjectileType<TameAngryAniseProj>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "TameAngryAnise", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.TameAngryAnise.TameAngryAnise"));
            line.OverrideColor = new Color(180, 255, 180);
            tooltips.Add(line);
        }




        public override bool AllowPrefix(int pre)
        {

            if (pre > 0) return false;


            return base.AllowPrefix(pre);
        }


        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {

            if (pre == -3) return false;


            if (pre == -1) return false;


            if (pre == -2) return false;


            if (pre > 0) return false;


            return false;
        }
    }
}
