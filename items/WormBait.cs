using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.items
{
    public class WormBait : ModItem
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.White;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;

            const int baitPower = 30;
            Item.bait = baitPower;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = player.Center + new Vector2(player.direction * 20f, -8f);
                NPC.NewNPC(
                    player.GetSource_ItemUse(Item),
                    (int)spawnPos.X,
                    (int)spawnPos.Y,
                    ModContent.NPCType<NPCs.LongWorm>()
                );
            }

            return true;
        }
    }
}
