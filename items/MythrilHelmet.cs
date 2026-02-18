using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class MythrilHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 14;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 4);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.13f;

            player.GetCritChance<EndlessThrower>() += 11;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.MythrilChainmail &&
           legs.type == ItemID.MythrilGreaves;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower attack speed by 13% and damage by 16%";
            player.GetAttackSpeed<EndlessThrower>() += 0.13f;
            player.GetDamage<EndlessThrower>() += 0.16f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.MythrilHelmet.AniseForestHelmetDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MythrilBar, 11)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

