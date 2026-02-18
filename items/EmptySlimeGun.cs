using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class EmptySlimeGun : ModItem
    {

        public override void SetStaticDefaults()
        {

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SlimeGun)
                .AddIngredient(ItemID.Bottle)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "EmptySlimeGunDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.EmptySlimeGun.EmptySlimeGunDesc")));
        }
    }
}