using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Etobudet1modtipo.NPCs;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class AniseKingSlimeBag : ModItem
    {
        public override void SetStaticDefaults()
        {

            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {




            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AromaticGel>(), 1, 50, 100));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseRifle>(), 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<HeartOfAniseForest>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestBar>(), 1, 30, 50));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseSoda>(), 4, 5, 10));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AromaticHalo>(), 10));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseBombard>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestHelmet>(), 5));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestBreastplate>(), 5));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestLeggings>(), 5));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PoisonGun>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseGasEmitter>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TameAngryAnise>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AniseForestSeeds>(), 1, 20, 25));
            itemLoot.Add(ItemDropRule.Common(ItemID.StarAnise, 1, 100, 150));


            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<AniseKingSlime>()));
        }
    }
}