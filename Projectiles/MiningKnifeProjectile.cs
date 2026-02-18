using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class MiningKnifeProjectile : ModProjectile
    {
        private const int TileSize = 16;
        private const int BreakIntervalTicks = 2;
        private const int ExtraScanPixels = 6;
        private const int MaxFailedCollisions = 6;

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;

            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60000;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= BreakIntervalTicks)
            {
                Projectile.localAI[0] = 0f;

                if (TryBreakTilesAroundProjectile())
                {
                    Projectile.ai[1] = 0f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool brokeAnyTile = TryBreakTilesAroundProjectile();

            if (brokeAnyTile)
            {
                Projectile.ai[1] = 0f;
                Projectile.velocity = oldVelocity;
                return false;
            }


            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;

            if (Projectile.velocity.LengthSquared() < 1f)
                Projectile.velocity = oldVelocity * 0.8f;

            Projectile.ai[1]++;
            if (Projectile.ai[1] >= MaxFailedCollisions)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                return true;
            }

            return false;
        }

        private bool TryBreakTilesAroundProjectile()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return false;

            bool brokeAnyTile = false;

            int minX = (int)((Projectile.position.X - ExtraScanPixels) / TileSize);
            int maxX = (int)((Projectile.position.X + Projectile.width + ExtraScanPixels) / TileSize);
            int minY = (int)((Projectile.position.Y - ExtraScanPixels) / TileSize);
            int maxY = (int)((Projectile.position.Y + Projectile.height + ExtraScanPixels) / TileSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile == null || !tile.HasTile)
                        continue;

                    WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: false);

                    if (!Main.tile[x, y].HasTile)
                    {
                        brokeAnyTile = true;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                        }
                    }
                }
            }

            return brokeAnyTile;
        }
    }
}
