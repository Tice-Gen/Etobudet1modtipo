using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Threading.Tasks;
using Etobudet1modtipo.NPCs;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class DyingBlackStar : ModItem
    {
        public static bool bossActive = false;

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
            Item.rare = ItemRarityID.Red;
        }

        public override bool CanUseItem(Player player)
        {

            return !NPC.AnyNPCs(ModContent.NPCType<SeraphimOfCalamity>()) && !bossActive;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ)
            {
                bossActive = true;
                HandleBossSummon(player);
            }
            return true;
        }

                public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 250)
                .AddIngredient(ItemID.LunarBar, 30)
                .AddIngredient(ItemID.FragmentVortex, 10)
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ItemID.FragmentNebula, 10)                
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        private async void HandleBossSummon(Player player)
        {
            await Task.Delay(5000);
            Main.NewText("[?!?] Your nightmare is coming...", 128, 128, 128);

            await Task.Delay(10000);
            Main.NewText("[?!?] The air is becoming clear...", 128, 128, 128);

            await Task.Delay(5000);
            Main.NewText("[?!?] ...and empty", 128, 128, 128);

            await Task.Delay(10000);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SeraphimOfCalamity>());
            }
        }
    }
}
