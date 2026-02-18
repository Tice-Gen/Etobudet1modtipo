
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Body)]
    public class AniseForestBreastplate : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 4;
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.06f;

            player.GetDamage(DamageClass.Ranged) += 0.05f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseForestBreastplateDesc", Terraria.Localization.Language.GetTextValue("Increases Endless Thrower damage by 6%\nIncreases ranged damage by 5%")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.StarAnise, 10)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 20)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

