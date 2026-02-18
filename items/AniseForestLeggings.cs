
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Legs)]
    public class AniseForestLeggings : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 2;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.GetDamage<EndlessThrower>() += 0.02f;

            player.GetCritChance(DamageClass.Ranged) += 2;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestLeggingsDesc", Terraria.Localization.Language.GetTextValue("Increases Endless Thrower damage by 2%, movement speed by 10%\nIncreases ranged critical strike chance by 2%")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 5)
                .AddIngredient(ItemID.StarAnise, 15)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 10)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

