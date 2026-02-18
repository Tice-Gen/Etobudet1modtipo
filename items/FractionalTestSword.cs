using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Etobudet1modtipo;

namespace Etobudet1modtipo.items
{
    public class FractionalTestSword : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 1;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Melee;
            Item.value = 1000;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.RemoveAll(line => line.Name == "Damage" && line.Mod == "Terraria");


            string damageWord = Language.GetTextValue("LegacyTooltip.2");


            TooltipLine newLine = new TooltipLine(Mod, "Damage", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FractionalTestSword.DamageLine", damageWord));
            int nameIndex = tooltips.FindIndex(line => line.Name == "ItemName");
            if (nameIndex != -1)
                tooltips.Insert(nameIndex + 1, newLine);
            else
                tooltips.Insert(0, newLine);
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.life += damageDone;
            if (target.life > target.lifeMax)
                target.life = target.lifeMax;


            FractionalDamage.AddToNPC(target, 1.5f);


            CombatText.NewText(target.Hitbox, Color.Orange, "1.5", dramatic: false, dot: true);


            if (Main.netMode == NetmodeID.Server)
                target.netUpdate = true;
        }
    }
}
