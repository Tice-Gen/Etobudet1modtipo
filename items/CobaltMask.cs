using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class CobaltMask : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 11;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.10f;

            player.GetAttackSpeed<EndlessThrower>() += 0.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.CobaltBreastplate &&
           legs.type == ItemID.CobaltLeggings;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower attack speed by 20%";
            player.GetAttackSpeed<EndlessThrower>() += 0.20f;
            player.armorEffectDrawOutlines = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CobaltMask.AniseForestHelmetDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 11)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

