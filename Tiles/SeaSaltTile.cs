using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class SeaSaltTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(210, 200, 180));
            DustType = DustID.SilverFlame;
            MineResist = 1.0f;
            MinPick = 0;

            RegisterItemDrop(ModContent.ItemType<SeaSalt>());
        }

        public override void RandomUpdate(int i, int j)
        {

            if (!WorldGen.genRand.NextBool(3))
            {
                return;
            }

            int saltCrystalTile = ModContent.TileType<SaltCrystalTile>();
            TryPlaceSaltCrystalFromAnchor(i, j, saltCrystalTile);
        }

        private static void TryPlaceSaltCrystalFromAnchor(int x, int y, int saltCrystalTile)
        {
            int first = WorldGen.genRand.Next(4);

            for (int step = 0; step < 4; step++)
            {
                int dir = (first + step) % 4;
                int tx = x;
                int ty = y;
                int alt = 0;

                switch (dir)
                {
                    case 0:
                        ty = y - 1;
                        alt = 0;
                        break;
                    case 1:
                        ty = y + 1;
                        alt = 1;
                        break;
                    case 2:
                        tx = x - 1;
                        alt = 2;
                        break;
                    default:
                        tx = x + 1;
                        alt = 3;
                        break;
                }

                if (!WorldGen.InWorld(tx, ty, 2))
                {
                    continue;
                }

                Tile target = Framing.GetTileSafely(tx, ty);
                if (target.HasTile || target.LiquidAmount > 130)
                {
                    continue;
                }

                bool placed = WorldGen.PlaceObject(
                    tx,
                    ty,
                    saltCrystalTile,
                    mute: true,
                    style: 0,
                    alternate: alt,
                    random: WorldGen.genRand.Next(3));

                if (!placed)
                {
                    continue;
                }

                Tile placedTile = Framing.GetTileSafely(tx, ty);
                placedTile.TileFrameY = (short)(alt * 18);
                WorldGen.SquareTileFrame(tx, ty, true);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, tx, ty, 3);
                }

                break;
            }
        }
    }
}
