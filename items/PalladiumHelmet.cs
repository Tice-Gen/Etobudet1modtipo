using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class PalladiumHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 12;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.14f;
            player.GetAttackSpeed<EndlessThrower>() += 0.11f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.PalladiumBreastplate &&
           legs.type == ItemID.PalladiumLeggings;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower critical strike chance by 10%";
            player.GetCritChance<EndlessThrower>() += 0.20f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "PalladiumHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.PalladiumHelmet.PalladiumHelmetDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PalladiumBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

