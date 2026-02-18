using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;

namespace Etobudet1modtipo.UI
{
    public class BerserkSlot : ModAccessorySlot
    {
        public override string Name => "BerserkSlot";

        public override bool CanAcceptItem(Item item, AccessorySlotType context)
        {
            return item.type == ModContent.ItemType<items.BerserkAmulet>();
        }

        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            return false;
        }

        public override Vector2? CustomLocation => new Vector2(Main.screenWidth / 2 + 587f, Main.screenHeight / 2f - 51f);

        public override bool IsEnabled() => true;
        public override bool IsVisibleWhenNotEnabled() => true;
    }
}
