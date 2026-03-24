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
    public class Fan : ModTile
    {
        private const int TileFrameSize = 16;
        private const int TileFootprintWidth = 16;
        private const int TileFootprintHeight = 32;
        private const int DrawTextureWidth = 26;
        private const int DrawTextureHeight = 38;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateWidth = TileFrameSize;
            TileObjectData.newTile.CoordinateHeights = new[] { TileFrameSize, TileFrameSize };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = AnchorData.Empty;
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newTile.AnchorRight = AnchorData.Empty;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table, 1, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(135, 160, 178));
            RegisterItemDrop(ModContent.ItemType<items.Fan>());

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            EnsureFanProjectile(GetTopLeft(i, j));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Point16 topLeft = GetTopLeft(i, j);
            if (topLeft.X != i || topLeft.Y != j)
            {
                return;
            }

            EnsureFanProjectile(topLeft);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<items.Fan>();
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point16 topLeft = GetTopLeft(i, j);
            if (topLeft.X != i || topLeft.Y != j)
            {
                return false;
            }

            Vector2 drawPosition = new Vector2(
                topLeft.X * 16f + (TileFootprintWidth - DrawTextureWidth) * 0.5f,
                topLeft.Y * 16f + (TileFootprintHeight - DrawTextureHeight) * 0.5f) - Main.screenPosition;

            if (!Main.drawToScreen)
            {
                drawPosition += new Vector2(Main.offScreenRange);
            }

            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPosition,
                new Rectangle(0, 0, DrawTextureWidth, DrawTextureHeight),
                Lighting.GetColor(i, j));

            return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 topLeft = new Point16(i, j - (frameY / TileFrameSize) % 2);
            KillLinkedFan(topLeft);
        }

        public static Point16 GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int top = j - (tile.TileFrameY % (TileFrameSize * 2) >= TileFrameSize ? 1 : 0);
            return new Point16(i, top);
        }

        public static Vector2 GetRotorCenterWorld(Point16 topLeft)
        {
            // Keep the rotor centered horizontally and shifted 3 pixels higher than the current placement.
            return new Vector2(topLeft.X * 16f + 8f, topLeft.Y * 16f + 11f);
        }

        private static void EnsureFanProjectile(Point16 topLeft)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || FindLinkedFan(topLeft) != -1)
            {
                return;
            }

            Vector2 center = GetRotorCenterWorld(topLeft);
            int projectileIndex = Projectile.NewProjectile(
                new EntitySource_TileBreak(topLeft.X, topLeft.Y),
                center,
                Vector2.Zero,
                ModContent.ProjectileType<Projectiles.FanCooler>(),
                0,
                0f,
                Main.myPlayer,
                topLeft.X,
                topLeft.Y);

            if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
            {
                Main.projectile[projectileIndex].netUpdate = true;
            }
        }

        private static void KillLinkedFan(Point16 topLeft)
        {
            int projectileIndex = FindLinkedFan(topLeft);
            if (projectileIndex == -1)
            {
                return;
            }

            Main.projectile[projectileIndex].Kill();
        }

        private static int FindLinkedFan(Point16 topLeft)
        {
            int projectileType = ModContent.ProjectileType<Projectiles.FanCooler>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.type != projectileType)
                {
                    continue;
                }

                if ((int)projectile.ai[0] == topLeft.X && (int)projectile.ai[1] == topLeft.Y)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
