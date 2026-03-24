using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Etobudet1modtipo.Tiles
{
    public class IndastrilCoolerTile : ModTile
    {
        private const int TileFrameSize = 16;
        private const int TileFootprintSize = 32;
        private const int DrawTextureSize = 24;

        public override string Texture => "Etobudet1modtipo/Tiles/IndustrialCooler";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateWidth = TileFrameSize;
            TileObjectData.newTile.CoordinateHeights = new[] { TileFrameSize, TileFrameSize };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(115, 155, 190));
            RegisterItemDrop(ModContent.ItemType<items.IndastrilCooler>());

            DustType = DustID.Ice;
            HitSound = SoundID.Tink;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            EnsureCoolerProjectile(GetTopLeft(i, j));
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

            EnsureCoolerProjectile(topLeft);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<items.IndastrilCooler>();
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point16 topLeft = GetTopLeft(i, j);
            if (topLeft.X != i || topLeft.Y != j)
            {
                return false;
            }

            Vector2 drawPosition = new Vector2(
                topLeft.X * 16f + (TileFootprintSize - DrawTextureSize) * 0.5f,
                topLeft.Y * 16f + (TileFootprintSize - DrawTextureSize) * 0.5f) - Main.screenPosition;

            if (!Main.drawToScreen)
            {
                drawPosition += new Vector2(Main.offScreenRange);
            }

            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPosition,
                new Rectangle(0, 0, DrawTextureSize, DrawTextureSize),
                Lighting.GetColor(i, j));

            return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 topLeft = new Point16(i - (frameX / TileFrameSize) % 2, j - (frameY / TileFrameSize) % 2);
            KillLinkedCooler(topLeft);
        }

        public static Point16 GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int left = i - (tile.TileFrameX % (TileFrameSize * 2) >= TileFrameSize ? 1 : 0);
            int top = j - (tile.TileFrameY % (TileFrameSize * 2) >= TileFrameSize ? 1 : 0);
            return new Point16(left, top);
        }

        public static Vector2 GetCenterWorld(Point16 topLeft)
        {
            return new Vector2(topLeft.X * 16f + TileFootprintSize * 0.5f, topLeft.Y * 16f + TileFootprintSize * 0.5f);
        }

        private static void EnsureCoolerProjectile(Point16 topLeft)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || FindLinkedCooler(topLeft) != -1)
            {
                return;
            }

            Vector2 center = GetCenterWorld(topLeft);
            int projectileIndex = Projectile.NewProjectile(
                new EntitySource_TileBreak(topLeft.X, topLeft.Y),
                center,
                Vector2.Zero,
                ModContent.ProjectileType<Projectiles.Cooler>(),
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

        private static void KillLinkedCooler(Point16 topLeft)
        {
            int projectileIndex = FindLinkedCooler(topLeft);
            if (projectileIndex == -1)
            {
                return;
            }

            Main.projectile[projectileIndex].Kill();
        }

        private static int FindLinkedCooler(Point16 topLeft)
        {
            int projectileType = ModContent.ProjectileType<Projectiles.Cooler>();
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
