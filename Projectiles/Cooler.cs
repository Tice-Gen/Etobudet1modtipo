using System.Collections.Generic;
using Etobudet1modtipo.Common.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class Cooler : ModProjectile
    {
        private const float RotationSpeed = 0.16f;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;

        public override void DrawBehind(
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            Point16 topLeft = new Point16((int)Projectile.ai[0], (int)Projectile.ai[1]);
            if (!IsAnchorValid(topLeft))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Tiles.IndastrilCoolerTile.GetCenterWorld(topLeft);
            Projectile.rotation += RotationSpeed;
            MachineAmbientSoundSystem.RefreshMachineSound(
                MachineAmbientSoundSystem.MachineSoundKind.IndustrialCooler,
                topLeft,
                Projectile.Center);
        }

        private static bool IsAnchorValid(Point16 topLeft)
        {
            int tileType = ModContent.TileType<Tiles.IndastrilCoolerTile>();

            for (int x = topLeft.X; x < topLeft.X + 2; x++)
            {
                for (int y = topLeft.Y; y < topLeft.Y + 2; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile || tile.TileType != tileType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
