using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class FragmentationGrenade : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 9999;
            Item.consumable = true;

            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 45;
            Item.useAnimation = 45;

            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.FragmentationGrenadeProj>();
            Item.shootSpeed = 6f;

            Item.knockBack = 6f;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 0, 50);

            Item.UseSound = SoundID.Item1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
{
    tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FragmentationGrenade.Description")));
}

    }
}
