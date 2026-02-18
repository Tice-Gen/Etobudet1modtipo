using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.NPCs;
using Terraria.Audio;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class StrangeBigAromaticGel : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 20;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.consumable = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {

            return !NPC.AnyNPCs(ModContent.NPCType<AniseKingSlime>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {

                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AniseKingSlime>());
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 50)
                .AddIngredient(ItemID.StarAnise, 25)
                .AddIngredient(ModContent.ItemType<AngryAnise>())
                .Register();
        }
    }
}
