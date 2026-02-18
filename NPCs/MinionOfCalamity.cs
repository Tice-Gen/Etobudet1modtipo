using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.NPCs
{
    public class MinionOfCalamity : ModNPC
    {
        private int laserTimer = 0;
        private const int laserInterval = 60 * 2;

        public override void SetStaticDefaults()
        {

            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 20;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.8f;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = 0;
            NPC.friendly = false;
            NPC.netAlways = true;


            NPC.color = Color.White;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity.Y -= 0.1f;
                    return;
                }
            }


            Vector2 direction = player.Center - NPC.Center;
            float speed = 6f;
            float inertia = 20f;
            direction.Normalize();
            direction *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            NPC.rotation = NPC.velocity.X * 0.05f;


            laserTimer++;
            if (laserTimer >= laserInterval)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shootDir = Vector2.Normalize(player.Center - NPC.Center) * 12f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootDir,
                        ProjectileID.EyeLaser, 10, 0f, Main.myPlayer);
                }
                laserTimer = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {

            NPC.frameCounter++;
            if (NPC.frameCounter >= 8)
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
                NPC.frameCounter = 0;
            }
        }
    }
}
