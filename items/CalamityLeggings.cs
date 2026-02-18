using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Legs)]
    public class CalamityLeggings : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 6;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.07f;
            player.GetDamage<EndlessThrower>() += 0.06f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "EndlessThrowerLeggingsDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CalamityLeggings.EndlessThrowerLeggingsDesc")));
        }

        public override void AddRecipes()
        {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 15);
            recipe1.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 15);
            recipe2.AddIngredient(ItemID.DemoniteBar, 5);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
