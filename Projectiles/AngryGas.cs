using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class AngryGas : ModProjectile
    {
        private const int FadeTime = 60; 

        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300; 
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.alpha = 0; 
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);


            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                

                float trailAlpha = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(new Color(100, 255, 100, 0)) * trailAlpha * 0.3f;
                

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * (1f + k * 0.05f), SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {

            Projectile.rotation += 0.02f; 


            if (Main.rand.NextFloat() < 0.3f)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 100);
                Main.dust[d].velocity *= 0.2f;
                Main.dust[d].noGravity = true;
            }


            if (Projectile.timeLeft <= FadeTime)
            {
                float factor = Projectile.timeLeft / (float)FadeTime;
                factor = MathHelper.Clamp(factor, 0f, 1f);

                Projectile.alpha = (int)(255f * (1f - factor));
                Projectile.scale = factor;

                if (Main.rand.NextFloat() < 0.15f)
                {
                    int d2 = Dust.NewDust(Projectile.position, Math.Max(1, (int)(Projectile.width * Projectile.scale)), Math.Max(1, (int)(Projectile.height * Projectile.scale)), DustID.GreenTorch, 0f, 0f, 100);
                    Main.dust[d2].velocity *= 0.1f;
                    Main.dust[d2].noGravity = true;
                }
            }
            else
            {
                Projectile.alpha = 0;
                Projectile.scale = 1f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Venom, 8);
        }
    }
}