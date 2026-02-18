using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.items
{
    public class BlurTester : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            ScreenBlurSystem.Enabled = !ScreenBlurSystem.Enabled;
            return true;
        }
    }
}
