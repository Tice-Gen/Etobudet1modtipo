using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.Rarities;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class AniseForestHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 3;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<EndlessThrower>() += 0.03f;
            player.GetDamage(DamageClass.Ranged) += 0.02f;
            player.GetCritChance(DamageClass.Ranged) += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AniseForestBreastplate>() &&
                   legs.type == ModContent.ItemType<AniseForestLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+20% Endless Thrower attack speed\n+15% movement speed\nImmunity to Highly Concentrated Taste";
            player.GetAttackSpeed<EndlessThrower>() += 0.20f;
            player.moveSpeed += 0.15f;
            

            player.buffImmune[ModContent.BuffType<HighlyConcentratedTaste>()] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Increases Endless Thrower damage by 3%\nIncreases ranged damage by 2% and critical strike chance by 4%")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
                .AddIngredient(ItemID.StarAnise, 5)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 10)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
