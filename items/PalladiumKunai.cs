using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class PalladiumKunai : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 44;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<PalladiumKunaiProj>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.None;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "PalladiumKunaiDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.PalladiumKunai.PalladiumKunaiDesc")));
        }
    }
}
