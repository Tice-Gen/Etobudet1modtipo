using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Tiles;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class SkySteel : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 50);
            Item.rare = ItemRarityID.Red;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;


            Item.createTile = ModContent.TileType<SkySteelTile>();
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip1", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.SkySteel.Tooltip1")));
        }
    }
}
