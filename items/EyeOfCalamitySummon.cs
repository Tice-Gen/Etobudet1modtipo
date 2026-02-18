using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.items
{
    public class EyeOfCalamitySummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {

            return !NPC.AnyNPCs(ModContent.NPCType<EyeOfCalamity>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<EyeOfCalamity>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SuspiciousLookingEye, 1)
                .AddIngredient(ItemID.DemoniteOre, 15)
                .AddTile(TileID.DemonAltar)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SuspiciousLookingEye, 1)
                .AddIngredient(ItemID.CrimtaneOre, 15)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}