using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class AniseAcs : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
{
    Projectile.width = 22;
    Projectile.height = 22;

    Projectile.friendly = true;
    Projectile.penetrate = -1;

    Projectile.tileCollide = false;
    Projectile.ignoreWater = true;

    Projectile.timeLeft = 2;

    Projectile.DamageType = DamageClass.Generic;


    Projectile.usesLocalNPCImmunity = true;
    Projectile.localNPCHitCooldown = 10;
}


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            

            if (player.active && !player.dead)
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
                return;
            }


            float radius = Projectile.ai[1];
            float index = Projectile.ai[0];
            float totalCount = 8f;
            float baseAngle = MathHelper.TwoPi * (index / totalCount);
            

            float rotationSpeed = MathHelper.TwoPi / (60f * 5f);
            float currentAngle = baseAngle + Main.GameUpdateCount * rotationSpeed;


            Projectile.Center = player.Center + radius * new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle));


            Projectile.rotation += 0.15f; 
            

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);


            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                
                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(new Color(150, 255, 150, 0)) * progress * 0.6f;
                
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * progress, SpriteEffects.None, 0);
            }


            return true;
        }
    }
}