using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.items
{
    public class CosmicSandClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 61;
            Item.height = 86;
            Item.maxStack = 20;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Roar;
            Item.consumable = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Astraclysm>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Astraclysm>());
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentSolar, 6)
                .AddIngredient(ItemID.FragmentVortex, 6)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddIngredient(ItemID.SandBlock, 50)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
