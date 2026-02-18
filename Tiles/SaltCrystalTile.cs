using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class SaltCrystalTile : ModTile
    {
        private const int FrameSize = 18;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateWidth = FrameSize;
            TileObjectData.newTile.CoordinateHeights = new[] { FrameSize };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorTop = AnchorData.Empty;
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newTile.AnchorRight = AnchorData.Empty;
            TileObjectData.addTile(Type);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(0);

            DustType = DustID.SilverFlame;
            HitSound = SoundID.Shatter;
            AddMapEntry(new Color(220, 220, 235));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
            {
                return;
            }

            noItem = true;
            int stack = Main.rand.Next(1, 3);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<Salt>(), stack);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!tile.HasTile)
            {
                return false;
            }

            int style = tile.TileFrameX / FrameSize;
            int orientation = tile.TileFrameY / FrameSize;

            Rectangle source = new Rectangle(style * FrameSize, 0, FrameSize, FrameSize);
            Vector2 center = new Vector2(i * 16 + 8f, j * 16 + 8f);
            Vector2 drawPos = center - Main.screenPosition;
            if (!Main.drawToScreen)
            {
                drawPos += new Vector2(Main.offScreenRange);
            }

            float rotation = 0f;
            SpriteEffects effects = SpriteEffects.None;

            if (orientation == 1)
            {
                effects = SpriteEffects.FlipVertically;
            }
            else if (orientation == 2)
            {
                rotation = -MathHelper.PiOver2;
            }
            else if (orientation == 3)
            {
                rotation = MathHelper.PiOver2;
            }

            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPos,
                source,
                Lighting.GetColor(i, j),
                rotation,
                new Vector2(FrameSize * 0.5f, FrameSize * 0.5f),
                1f,
                effects,
                0f);

            return false;
        }
    }
}
