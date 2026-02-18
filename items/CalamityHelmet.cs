using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class CalamityHelmet : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 7;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<EndlessThrower>() += 0.14f;
        }


        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<CalamityBreastplate>() &&
                   legs.type == ModContent.ItemType<CalamityLeggings>();
        }


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Set bonus: 6% reduced incoming damage.\nWhen below 50% HP: +4 defense and +8% Endless Thrower damage.";
            player.endurance += 0.06f;


            if (player.statLife < player.statLifeMax2 / 2)
            {
                player.statDefense += 4;
                player.GetDamage<EndlessThrower>() += 0.08f;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "EndlessThrowerHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CalamityHelmet.EndlessThrowerHelmetDesc")));
        }

        public override void AddRecipes()
        {

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 15);
            recipe1.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 15);
            recipe2.AddIngredient(ItemID.DemoniteBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}

