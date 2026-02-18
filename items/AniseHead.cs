using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class AniseHead : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 7777;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(platinum: 9999);
            Item.maxStack = 9999;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AniseForestBreastplate>() &&
                   legs.type == ModContent.ItemType<AniseForestLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+1000% Endless Thrower attack speed";

            player.GetAttackSpeed<EndlessThrower>() += 10f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseHeadDesc", Terraria.Localization.Language.GetTextValue("Terrible at impressing the developer!")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 9999)
                .AddIngredient(ItemID.LunarBar, 9999)
                .AddIngredient(ItemID.LunarBar, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.StarAnise, 9999)
                .AddIngredient(ItemID.DirtiestBlock, 9999)
                .AddIngredient(ItemID.DirtiestBlock, 9999)
                .AddTile(TileID.DirtiestBlock)
                .Register();
        }
    }
}
