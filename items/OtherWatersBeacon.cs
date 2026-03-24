using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class OtherWatersBeacon : ModItem
    {
        public override string Texture => "Etobudet1modtipo/items/DeepSeaHook";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item4;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool CanUseItem(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return true;
            }

            if (!global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersAvailable())
            {
                Main.NewText("Other Waters is unavailable. Check SubworldLibrary and reload mods.", Color.OrangeRed);
                return true;
            }

            if (global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive())
            {
                if (!global::Etobudet1modtipo.Etobudet1modtipo.ExitSubworld())
                {
                    Main.NewText("Could not exit Other Waters.", Color.OrangeRed);
                }

                return true;
            }

            if (!global::Etobudet1modtipo.Etobudet1modtipo.EnterOtherWaters())
            {
                Main.NewText("Could not enter Other Waters.", Color.OrangeRed);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MagicMirror)
                .AddIngredient(ItemID.Coral, 8)
                .AddIngredient(ItemID.Starfish, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
