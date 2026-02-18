using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Classes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class PinwheelShuriken : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<Pinwheel>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            TooltipLine funLine = new TooltipLine(Mod, "FunWeapon", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.PinwheelShuriken.FunWeapon"))
            {
                OverrideColor = Color.Red
            };

            int damageIndex = tooltips.FindIndex(t => t.Name == "Damage");
            if (damageIndex != -1)
                tooltips.Insert(damageIndex, funLine);
            else
                tooltips.Add(funLine);


            tooltips.Add(new TooltipLine(Mod, "Desc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.PinwheelShuriken.Desc")));
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            float spreadAngle = Main.rand.NextFloat(-0.0872665f, 0.0872665f);
            velocity = velocity.RotatedBy(spreadAngle);


            float verticalOffset = Main.rand.Next(2) == 0 ? 0f : 5f;
            position.Y += verticalOffset * player.direction;
        }
    }
}