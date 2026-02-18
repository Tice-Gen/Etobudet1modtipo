using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class WoodenRandomBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.consumable = true;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            double roll = Main.rand.NextDouble() * 100;

            if (roll < 5)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Gladius);
            }
            else if (roll < 8)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.EnchantedSword);
            }
            else if (roll < 10)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Terragrim);
            }
            else if (roll < 1)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Arkhalis);
            }
            else if (roll < 40)
            {
                int amount = Main.rand.Next(10, 41);
                int type = Main.rand.NextBool() ? ItemID.IronBar : ItemID.LeadBar;
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), type, amount);
            }
            else if (roll < 50)
            {
                int amount = Main.rand.Next(20, 31);
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.GoldBar, amount);
            }


            int coins = Main.rand.Next(50, 101);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.SilverCoin, coins);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 40)
                .AddIngredient(ItemID.IronBar, 30)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}