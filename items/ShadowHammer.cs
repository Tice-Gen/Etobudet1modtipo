using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class ShadowHammer : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();

            Item.width = 60;
            Item.height = 60;

            Item.useTime = 33;
            Item.useAnimation = 33;


            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;


            Item.shoot = ModContent.ProjectileType<ShadowHammerProj>();
            Item.shootSpeed = 12f;

            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 27;
            Item.noMelee = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ShadowHammerDescription", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ShadowHammer.ShadowHammerDescription")));
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddIngredient(ItemID.DarkShard, 2)
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
