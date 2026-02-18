using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using System;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
    public class AngrySunflowerTile : ModTile
    {

        private static int[,] shootTimers = new int[Main.maxTilesX, Main.maxTilesY];

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 18 };
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.addTile(Type);
            

            RegisterItemDrop(ModContent.ItemType<AngrySunflower>());
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;


            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
                return;

            shootTimers[i, j]++;

            int shootInterval = 15;
            int damage = 10;
            float range = 800f;
            float speed = 10f;

            if (shootTimers[i, j] < shootInterval)
                return;

            NPC target = FindClosestEnemy(new Vector2(i * 16 + 16, j * 16 + 24), range);
            if (target == null)
                return;

            shootTimers[i, j] = 0;

            Vector2 spawn = new Vector2(
                i * 16 + 16f,
                j * 16 + 4f
            );

            Vector2 dir = (target.Center - spawn).SafeNormalize(Vector2.Zero);
            Vector2 velocity = dir * speed;

            Projectile.NewProjectile(
                new EntitySource_TileInteraction(Main.LocalPlayer, i, j),
                spawn,
                velocity,
                ProjectileID.Leaf,
                damage,
                1f,
                Main.myPlayer
            );
        }

        private NPC FindClosestEnemy(Vector2 center, float range)
        {
            NPC target = null;
            float minDist = range;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.CanBeChasedBy())
                    continue;

                float dist = Vector2.Distance(center, npc.Center);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = npc;
                }
            }
            return target;
        }




        
        /*
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

        }
        */
    }
}