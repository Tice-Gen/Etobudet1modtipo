
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Legs)]
    public class FloweringChlorophyteLeggings : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 15;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.08f;
            player.GetCritChance<EndlessThrower>() += 0.07f;



            Lighting.AddLight(player.Center, 1f, 2f, 1f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestLeggingsDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FloweringChlorophyteLeggings.AniseForestLeggingsDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FloweringChlorophyteBar>(), 12)
                .AddIngredient(ItemID.ChlorophyteBar, 13)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
