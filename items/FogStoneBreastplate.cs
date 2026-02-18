
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Body)]
    public class FogStoneBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 1;
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 3;
            player.statDefense -= 301;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.RemoveAll(t => t.Name == "Defense" && t.Mod == "Terraria");


            TooltipLine defLine = new TooltipLine(Mod, "NegativeDefense", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FogStoneBreastplate.NegativeDefense"));
            TooltipLine minionLine = new TooltipLine(Mod, "MinionBonus", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FogStoneBreastplate.MinionBonus"));


            int setIndex = tooltips.FindIndex(t => t.Name == "SetBonus");
            if (setIndex == -1)
            {
                tooltips.Add(defLine);
                tooltips.Add(minionLine);
            }
            else
            {
                tooltips.Insert(setIndex, defLine);
                tooltips.Insert(setIndex + 1, minionLine);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FogStone>(), 14)
                .AddIngredient(ItemID.IronBar, 12)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FogStone>(), 14)
                .AddIngredient(ItemID.LeadBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
