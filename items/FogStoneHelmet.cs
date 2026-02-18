using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class FogStoneHelmet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 1;
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.buyPrice(silver: 5);
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense -= 201;
            player.maxMinions += 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FogStoneBreastplate>() && 
                   legs.type == ModContent.ItemType<FogStoneLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases incoming damage by 500%\nUnlocks use of the Broken Rod of Teleporting";
            

            player.endurance -= 5f;
            

            player.GetModPlayer<FogStoneArmorPlayer>().hasBrokenRodAccess = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(t => t.Name == "Defense" && t.Mod == "Terraria");
            
            TooltipLine defLine = new TooltipLine(Mod, "NegativeDefense", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FogStoneHelmet.NegativeDefense"));
            TooltipLine minionLine = new TooltipLine(Mod, "MinionBonus", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FogStoneHelmet.MinionBonus"));
            
            int setIndex = tooltips.FindIndex(t => t.Name == "SetBonus");
            if (setIndex == -1)
            {
                tooltips.Add(defLine);
                tooltips.Add(minionLine);
            }
            else
            {
                tooltips.Insert(setIndex, defLine);
                tooltips.Insert(setIndex + 1, minionLine);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FogStone>(), 20)
                .AddIngredient(ItemID.IronBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
                
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FogStone>(), 20)
                .AddIngredient(ItemID.LeadBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    

    public class FogStoneArmorPlayer : ModPlayer
    {
        public bool hasBrokenRodAccess = false;
        
        public override void ResetEffects()
        {
            hasBrokenRodAccess = false;
        }
    }
}
