using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class SeraphimCalamityStarV2 : ModProjectile
    {
        private Vector2 spawnCenter;
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Player player = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];
            Lighting.AddLight(Projectile.Center, 0.85f, 0.85f, 0.85f);

            if (!initialized)
            {
                spawnCenter = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                Projectile.ai[2] = 0;
                initialized = true;
            }

            if (Projectile.ai[2] == 0f)
            {
                float distanceFromCenter = Vector2.Distance(Projectile.Center, spawnCenter);

                if (distanceFromCenter >= 600f)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.ai[2] = 1f;
                    Projectile.localAI[0] = 0f;
                }
            }
            else if (Projectile.ai[2] == 1f)
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= 30f)
                {
                    Vector2 direction = player.Center - Projectile.Center;
                    direction.Normalize();
                    Projectile.velocity = direction * 15f;
                    Projectile.ai[2] = 2f;
                }
            }
            else if (Projectile.ai[2] == 5f)
            {
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 origin = frame.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * progress * 0.55f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    color,
                    Projectile.oldRot[i],
                    origin,
                    Projectile.scale * progress,
                    SpriteEffects.None,
                    0
                );
            }

            Vector2 centerDrawPos = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.White * 0.42f * Projectile.Opacity;
            float glowRadius = 3f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f) * glowRadius;
                Main.EntitySpriteDraw(
                    texture,
                    centerDrawPos + offset,
                    frame,
                    glowColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.04f,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }
    }
}
