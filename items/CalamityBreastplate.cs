using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Body)]
    public class CalamityBreastplate : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 8;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<EndlessThrower>() += 0.14f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "EndlessThrowerBreastplateDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CalamityBreastplate.EndlessThrowerBreastplateDesc")));
        }

        public override void AddRecipes()
        {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 20);
            recipe1.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 20);
            recipe2.AddIngredient(ItemID.DemoniteBar, 12);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
