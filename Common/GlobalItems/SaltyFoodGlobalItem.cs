using System.Collections.Generic;
using System.IO;
using System.Linq;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class SaltyFoodGlobalItem : GlobalItem
    {
        private const int BonusDurationTicks = 60 * 60;
        private const string SaltyTag = "IsSaltyFood";

        private static readonly HashSet<int> TargetFoodTypes = new()
        {
            ItemID.SeafoodDinner,
            ItemID.CookedFish,
            ItemID.CookedShrimp,
            ItemID.GrilledSquirrel,
            ItemID.RoastedBird,
            ItemID.RoastedDuck,
            ItemID.BowlofSoup,
            ItemID.ShuckedOyster,
            ItemID.FriedEgg,
            ItemID.BBQRibs,
            ItemID.Steak,
            ItemID.Spaghetti
        };

        private static readonly int[] FoodBuffTypes =
        {
            BuffID.WellFed,
            BuffID.WellFed2,
            BuffID.WellFed3
        };

        public override bool InstancePerEntity => true;

        public bool isSaltyFood;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return TargetFoodTypes.Contains(entity.type);
        }

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (context is not RecipeItemCreationContext recipeContext)
                return;

            int saltType = ModContent.ItemType<Salt>();
            if (!recipeContext.ConsumedItems.Any(i => i.type == saltType))
                return;

            isSaltyFood = true;
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (!isSaltyFood)
                return;

            int foodBuffType = item.buffType;
            if (!FoodBuffTypes.Contains(foodBuffType))
                return;

            int foodBuffIndex = player.FindBuffIndex(foodBuffType);
            int saltyDuration;

            if (foodBuffIndex >= 0)
            {
                player.buffTime[foodBuffIndex] += BonusDurationTicks;
                saltyDuration = player.buffTime[foodBuffIndex];
            }
            else
            {
                saltyDuration = item.buffTime + BonusDurationTicks;
                player.AddBuff(foodBuffType, saltyDuration);
            }

            player.AddBuff(ModContent.BuffType<SaltyFoodBonus>(), saltyDuration);
        }

        public override bool CanStack(Item destination, Item source)
        {
            bool destinationSalty = destination.GetGlobalItem<SaltyFoodGlobalItem>().isSaltyFood;
            bool sourceSalty = source.GetGlobalItem<SaltyFoodGlobalItem>().isSaltyFood;
            return destinationSalty == sourceSalty;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!isSaltyFood)
                return;

            string saltyPrefix = Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.SaltyFood.Prefix");
            for (int i = 0; i < tooltips.Count; i++)
            {
                TooltipLine line = tooltips[i];
                if (line.Name == "ItemName")
                {
                    line.Text = saltyPrefix + " " + line.Text;
                    break;
                }
            }

            tooltips.Add(new TooltipLine(
                Mod,
                "SaltyFoodBonus",
                Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.SaltyFood.Bonus")));
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (isSaltyFood)
                tag[SaltyTag] = true;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            isSaltyFood = tag.GetBool(SaltyTag);
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(isSaltyFood);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            isSaltyFood = reader.ReadBoolean();
        }
    }
}
