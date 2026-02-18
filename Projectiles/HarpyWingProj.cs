using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class HarpyWingProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 3;
            Projectile.timeLeft = 20;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = 0;
        }


        public override void AI()
        {
            Projectile.rotation =
                Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= 7)
                    Projectile.frame = 0;
            }

            Projectile.velocity.Y +=
                (float)System.Math.Sin(Projectile.timeLeft * 0.15f) * 0.03f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / 7;

            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = frame.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                Vector2 drawPos =
                    Projectile.oldPos[i]
                    - Main.screenPosition
                    + origin
                    + new Vector2(0f, Projectile.gfxOffY);

                float fade =
                    (Projectile.oldPos.Length - i)
                    / (float)Projectile.oldPos.Length;

                Color color =
                    Projectile.GetAlpha(Color.White)
                    * fade * 0.5f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    color,
                    Projectile.oldRot[i],
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }


        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {

            if (Main.rand.NextFloat() < 0.30f)
            {
                target.AddBuff(
                    ModContent.BuffType<SevereСuts>(),
                    180
                );
            }
        }


        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 25; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Harpy
                );

                Dust d = Main.dust[dust];
                d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.1f, 1.6f);
            }
        }
    }
}
