using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace Etobudet1modtipo.items
{
    public class Glacier : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;


            Item.shoot = ModContent.ProjectileType<GlacierWave>();
            Item.shootSpeed = 16f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.Glacier.Description"))
            {
                OverrideColor = Color.Cyan
            });
        }
    }
}