using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class DeepSeaStoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(36, 64, 92));
            DustType = DustID.Stone;
            MineResist = 40f;
            MinPick = 1000;

            RegisterItemDrop(ModContent.ItemType<DeepSeaStone>());
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, new Vector2(i * 16f, j * 16f));
                return false;
            }

            return true;
        }
    }
}
