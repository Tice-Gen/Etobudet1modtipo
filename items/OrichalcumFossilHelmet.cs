using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class OrichalcumFossilHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 15;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 4);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.19f;

            player.GetCritChance<EndlessThrower>() += 12;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.OrichalcumBreastplate &&
           legs.type == ItemID.OrichalcumLeggings;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower attack speed by 15% and critical strike chance by 10%";
            player.GetCritChance<EndlessThrower>() += 10f;
            player.GetAttackSpeed<EndlessThrower>() += 0.15f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.OrichalcumFossilHelmet.AniseForestHelmetDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.OrichalcumBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

