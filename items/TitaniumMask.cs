using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class TitaniumMask : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 19;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 4);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.27f;

            player.GetCritChance<EndlessThrower>() += 15;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.TitaniumBreastplate &&
           legs.type == ItemID.TitaniumLeggings;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower damage by 25%";
            player.GetDamage<EndlessThrower>() += 0.25f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.TitaniumMask.AniseForestHelmetDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 13)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

