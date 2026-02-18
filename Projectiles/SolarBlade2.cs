using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class SolarBlade2 : ModProjectile
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
            Projectile.aiStyle = -1; 
            Projectile.friendly = true; 
            Projectile.hostile = false; 
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600; 
            Projectile.tileCollide = true; 
            Projectile.ignoreWater = true; 
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;


            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.6f);


            if (Main.rand.NextBool(2)) 
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.5f;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                

                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(Color.Orange) * progress;
                

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 3600); 
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++) 
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 1.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}