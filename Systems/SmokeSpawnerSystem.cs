using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Systems.InfernalAwakening;
using System;

namespace Etobudet1modtipo.Systems
{
    public class SmokeSpawnerSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {

            if (!InfernalAwakeningSystem.IsActive())
                return;

            if (!Main.rand.NextBool(9))
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (player == null ||
                    !player.active ||
                    player.dead ||
                    !player.ZoneUnderworldHeight)
                    continue;

                int tx = (int)(player.Center.X / 16f) + Main.rand.Next(-50, 51);
                int ty = (int)(player.Center.Y / 16f) + Main.rand.Next(-30, 31);

                if (!WorldGen.InWorld(tx, ty) || !WorldGen.InWorld(tx, ty - 1))
                    continue;

                Tile tile = Main.tile[tx, ty];

                if (tile.LiquidAmount <= 0 || tile.LiquidType != LiquidID.Lava)
                    continue;

                if (Main.tile[tx, ty - 1].HasTile)
                    continue;

                Vector2 spawnPos = new Vector2(
                    tx * 16f + 8f,
                    (ty - 2) * 16f + 8f
                );

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    continue;

                int smokeCount = Main.rand.Next(3, 6);

                for (int j = 0; j < smokeCount; j++)
                {
                    float speed = Main.rand.NextFloat(1f, 2f);
                    float angle = MathHelper.ToRadians(Main.rand.NextFloat(-90f, 90f));

                    Vector2 velocity = new Vector2(
                        (float)Math.Cos(angle) * speed,
                        (float)Math.Sin(angle) * speed
                    );

                    Projectile.NewProjectile(
                        player.GetSource_Misc("InfernalSmoke"),
                        spawnPos,
                        velocity,
                        ModContent.ProjectileType<ToxicSmoke>(),
                        10,
                        0f,
                        player.whoAmI
                    );
                }
            }
        }
    }
}
