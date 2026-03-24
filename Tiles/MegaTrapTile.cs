using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Etobudet1modtipo.Tiles
{
    public class MegaTrapTile : ModTile
    {
        private const int TileFrameWidth = 16;
        private const int TileStyleWidth = 32;
        private const int DrawFrameWidth = 31;
        private const int FullFrameHeight = 20;
        private const int AngleStepCount = 8;

        private enum MountSurface
        {
            Floor = 0,
            Ceiling = 1,
            LeftWall = 2,
            RightWall = 3
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.CanBeSloped[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateWidth = TileFrameWidth;
            TileObjectData.newTile.CoordinateHeights = new[] { FullFrameHeight };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
            TileObjectData.newTile.AnchorTop = AnchorData.Empty;
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newTile.AnchorRight = AnchorData.Empty;
            TileObjectData.addTile(Type);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, 2, 0);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(0);

            AddMapEntry(new Color(72, 104, 76));
            RegisterItemDrop(ModContent.ItemType<items.MegaTrap>());

            DustType = DustID.Iron;
            HitSound = SoundID.Dig;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Point16 topLeft = GetTopLeft(i, j);
            int defaultAngleStyle = GetDefaultAngleStyle(GetMountSurface(topLeft));
            SetAngleStyle(topLeft, defaultAngleStyle);

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, 2);
        }

        public override bool RightClick(int i, int j)
        {
            Systems.MegaTrapSystem.TryActivate(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Point16 topLeft = GetTopLeft(i, j);
            Wiring.SkipWire(topLeft.X, topLeft.Y);
            Wiring.SkipWire(topLeft.X + 1, topLeft.Y);
            Systems.MegaTrapSystem.TryActivate(topLeft.X, topLeft.Y);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<items.MegaTrap>();
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point16 topLeft = GetTopLeft(i, j);
            if (i != topLeft.X || j != topLeft.Y)
                return false;

            Vector2 drawPosition = new Vector2(topLeft.X * 16f, topLeft.Y * 16f) - Main.screenPosition;
            if (!Main.drawToScreen)
                drawPosition += new Vector2(Main.offScreenRange);

            Vector2 origin = new Vector2(DrawFrameWidth * 0.5f, FullFrameHeight * 0.5f);
            Vector2 centeredDrawPosition = drawPosition + origin;
            GetDrawTransform(topLeft, out float rotation, out SpriteEffects effects);

            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                centeredDrawPosition,
                new Rectangle(0, 0, DrawFrameWidth, FullFrameHeight),
                Lighting.GetColor(i, j),
                rotation,
                origin,
                1f,
                effects,
                0f
            );

            return false;
        }

        public override bool Slope(int i, int j)
        {
            Point16 topLeft = GetTopLeft(i, j);
            if (!RotateAngle(topLeft))
                return false;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, 2);

            return false;
        }

        public static Point16 GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int frameX = tile.TileFrameX % TileStyleWidth;
            int left = i - (frameX >= TileFrameWidth ? 1 : 0);
            return new Point16(left, j);
        }

        public static Vector2 GetAimDirection(Point16 topLeft)
        {
            int angleStyle = GetAngleStyle(topLeft);
            return Vector2.UnitX.RotatedBy(MathHelper.PiOver4 * angleStyle);
        }

        public static float GetRotation(Point16 topLeft)
        {
            int angleStyle = GetAngleStyle(topLeft);
            return MathHelper.PiOver4 * angleStyle;
        }

        private static void GetDrawTransform(Point16 topLeft, out float rotation, out SpriteEffects effects)
        {
            rotation = MathHelper.WrapAngle(GetRotation(topLeft));
            effects = SpriteEffects.None;

            if (rotation > MathHelper.PiOver2)
            {
                rotation -= MathHelper.Pi;
                effects = SpriteEffects.FlipHorizontally;
            }
            else if (rotation < -MathHelper.PiOver2)
            {
                rotation += MathHelper.Pi;
                effects = SpriteEffects.FlipHorizontally;
            }
        }

        private static bool RotateAngle(Point16 topLeft)
        {
            int nextAngleStyle = (GetAngleStyle(topLeft) + 1) % AngleStepCount;
            return SetAngleStyle(topLeft, nextAngleStyle);
        }

        private static int GetAngleStyle(Point16 topLeft)
        {
            Tile tile = Framing.GetTileSafely(topLeft.X, topLeft.Y);
            return tile.TileFrameX / TileStyleWidth;
        }

        private static MountSurface GetMountSurface(Point16 topLeft)
        {
            Tile tile = Framing.GetTileSafely(topLeft.X, topLeft.Y);
            int mountIndex = tile.TileFrameY / FullFrameHeight;
            return mountIndex switch
            {
                1 => MountSurface.Ceiling,
                2 => MountSurface.LeftWall,
                3 => MountSurface.RightWall,
                _ => MountSurface.Floor
            };
        }

        private static int GetDefaultAngleStyle(MountSurface mountSurface)
        {
            return mountSurface switch
            {
                MountSurface.Ceiling => 2,
                MountSurface.RightWall => 4,
                _ => 0
            };
        }

        private static bool SetAngleStyle(Point16 topLeft, int angleStyle)
        {
            Tile left = Framing.GetTileSafely(topLeft.X, topLeft.Y);
            Tile right = Framing.GetTileSafely(topLeft.X + 1, topLeft.Y);

            if (!left.HasTile || !right.HasTile)
                return false;

            int styleOffset = angleStyle * TileStyleWidth;
            left.TileFrameX = (short)styleOffset;
            right.TileFrameX = (short)(styleOffset + TileFrameWidth);
            return true;
        }
    }
}
