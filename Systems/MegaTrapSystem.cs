using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Systems
{
    public class MegaTrapSystem : ModSystem
    {
        private const int ActivationDurationTicks = 60;
        private const int ActivationCooldownTicks = 300;
        private const int FireIntervalTicks = 6;
        private const int TrapDamage = 10;
        private const float TrapShotSpeed = 8f;
        private const float TrapSpreadDegrees = 12f;
        private const float TrapMuzzleOffset = 14f;

        private sealed class MegaTrapState
        {
            public int ActiveTimeLeft;
            public int CooldownTimeLeft;
            public int FireTimer;
        }

        private static readonly Dictionary<Point16, MegaTrapState> ActiveTraps = new();

        public override void OnWorldUnload()
        {
            ActiveTraps.Clear();
        }

        public override void PreUpdateWorld()
        {
            if (ActiveTraps.Count == 0)
                return;

            List<Point16> toRemove = null;

            foreach ((Point16 topLeft, MegaTrapState state) in ActiveTraps)
            {
                if (!IsValidMegaTrap(topLeft))
                {
                    toRemove ??= new List<Point16>();
                    toRemove.Add(topLeft);
                    continue;
                }

                if (state.CooldownTimeLeft > 0)
                    state.CooldownTimeLeft--;

                if (state.ActiveTimeLeft <= 0)
                    continue;

                if (state.FireTimer % FireIntervalTicks == 0)
                    FireTrap(topLeft);

                state.FireTimer++;
                state.ActiveTimeLeft--;
            }

            if (toRemove == null)
                return;

            foreach (Point16 point in toRemove)
                ActiveTraps.Remove(point);
        }

        public static bool TryActivate(int i, int j)
        {
            Point16 topLeft = Tiles.MegaTrapTile.GetTopLeft(i, j);
            if (!IsValidMegaTrap(topLeft))
                return false;

            if (!ActiveTraps.TryGetValue(topLeft, out MegaTrapState state))
            {
                state = new MegaTrapState();
                ActiveTraps[topLeft] = state;
            }

            if (state.ActiveTimeLeft > 0 || state.CooldownTimeLeft > 0)
                return false;

            state.ActiveTimeLeft = ActivationDurationTicks;
            state.CooldownTimeLeft = ActivationCooldownTicks;
            state.FireTimer = 0;
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(topLeft.X * 16f, topLeft.Y * 16f));
            return true;
        }

        private static bool IsValidMegaTrap(Point16 topLeft)
        {
            Tile left = Framing.GetTileSafely(topLeft.X, topLeft.Y);
            Tile right = Framing.GetTileSafely(topLeft.X + 1, topLeft.Y);

            return left.HasTile
                && right.HasTile
                && left.TileType == ModContent.TileType<Tiles.MegaTrapTile>()
                && right.TileType == ModContent.TileType<Tiles.MegaTrapTile>();
        }

        private static void FireTrap(Point16 topLeft)
        {
            Vector2 aimDirection = Tiles.MegaTrapTile.GetAimDirection(topLeft).SafeNormalize(Vector2.UnitX);
            Vector2 center = new Vector2(topLeft.X * 16f + 15.5f, topLeft.Y * 16f + 10f);
            Vector2 spawnPosition = center + aimDirection * TrapMuzzleOffset;
            Vector2 velocity = aimDirection
                .RotatedByRandom(MathHelper.ToRadians(TrapSpreadDegrees))
                * TrapShotSpeed;

            IEntitySource source = new EntitySource_TileInteraction(Main.LocalPlayer, topLeft.X, topLeft.Y);
            int projectileIndex = Projectile.NewProjectile(source, spawnPosition, velocity, ModContent.ProjectileType<TrapDart>(), TrapDamage, 1f, Main.myPlayer);
            Projectile projectile = Main.projectile[projectileIndex];
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.trap = false;
            projectile.DamageType = DamageClass.Default;

            SoundEngine.PlaySound(SoundID.Item17, spawnPosition);
        }
    }
}
