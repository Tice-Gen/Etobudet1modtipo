using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class StickyKnife : ModProjectile
    {
        private bool stuck = false;
        private int timer = 0;
        private int airTime = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; 
        }

        public override void SetDefaults()
        {
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!stuck)
            {
                StickToSurface();

                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }
            return false; 
        }

        private void StickToSurface()
        {
            stuck = true;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Projectile.friendly = false; 
        }

        public override void AI()
        {
            if (!stuck)
            {

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

                airTime++;
                if (airTime >= 60)
                {
                    Projectile.velocity.Y += 0.4f; 
                    Projectile.velocity.X *= 0.98f; 
                }
            }


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.3f, 0.1f);

            if (stuck)
            {
                timer++;

                if (timer > 40)
                {
                    Projectile.position += Main.rand.NextVector2Circular(1.5f, 1.5f);
                }

                if (timer >= 60)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!stuck)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
                Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, frameHeight * 0.5f);

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    if (Projectile.oldPos[k] == Vector2.Zero) continue;


                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                    
                    float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                    

                    Color color = Projectile.GetAlpha(lightColor) * progress * 0.5f;
                    
                    Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            int explosionRadius = 80; 


            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 1.5f);
                Main.dust[d].velocity *= 2f;
                if (Main.rand.NextBool(2))
                {
                    int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2f);
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].velocity *= 3f;
                }
            }


            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) <= explosionRadius)
                    {

                        npc.SimpleStrikeNPC(Projectile.damage / 2, Projectile.direction, false, Projectile.knockBack);
                    }
                }
            }
        }
    }
}