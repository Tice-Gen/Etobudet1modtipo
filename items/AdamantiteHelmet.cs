using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class AdamantiteHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 17;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 4);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.23f;

            player.GetCritChance<EndlessThrower>() += 13;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
{
    return body.type == ItemID.AdamantiteBreastplate &&
           legs.type == ItemID.AdamantiteLeggings;
}


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases Endless Thrower critical strike chance by 22%";
            player.GetCritChance<EndlessThrower>() += 0.22f;
            player.armorEffectDrawOutlines = true;
        }

        public override void ArmorSetShadows(Player player)
        {

            player.armorEffectDrawShadow = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Increases Endless Thrower damage by 23%, Endless Thrower critical strike chance by 13%")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

