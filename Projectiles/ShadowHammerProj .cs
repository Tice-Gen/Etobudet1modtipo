using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio; 
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Projectiles
{
    public class ShadowHammerProj : ModProjectile
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/ShadowHammerProj";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 79;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1; 
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
            Projectile.tileCollide = false; 
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600; 
            Projectile.aiStyle = 0;
            Projectile.scale = 1f;
            

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {

            Projectile.rotation += 0.3f;
            Projectile.spriteDirection = Projectile.velocity.X < 0f ? -1 : 1;


            Lighting.AddLight(Projectile.Center, 0.6f, 0.1f, 0.9f);


            Vector2[] offsets =
            {
                new Vector2( Projectile.width / 2f, -Projectile.height / 2f ),
                new Vector2(-Projectile.width / 2f, -Projectile.height / 2f ),
                new Vector2( Projectile.width / 2f,  Projectile.height / 2f ),
                new Vector2(-Projectile.width / 2f,  Projectile.height / 2f )
            };

            foreach (var off in offsets)
            {
                Vector2 rotated = off.RotatedBy(Projectile.rotation);
                Vector2 dustPos = Projectile.Center + rotated;

                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(dustPos, 4, 4, DustID.PurpleTorch, 0f, 0f, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Projectile.velocity * 0.2f;
                    Main.dust[dust].fadeIn = 1f;
                }
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
                


                Color color = new Color(150, 0, 255, 0) * progress * 0.5f;
                

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool(10)) 
            {
                modifiers.FinalDamage *= 10; 
                SoundEngine.PlaySound(SoundID.Item105, Projectile.Center);

                for (int i = 0; i < 35; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(7f, 7f);
                    int dustType = Main.rand.NextBool() ? DustID.Shadowflame : DustID.PurpleTorch;
                    int d = Dust.NewDust(target.Center, 0, 0, dustType, speed.X, speed.Y, 0, default, 2.5f);
                    Main.dust[d].noGravity = true; 
                    Main.dust[d].velocity *= 1.4f;
                }

                for (int j = 0; j < 15; j++)
                {
                     Vector2 speed = Main.rand.NextVector2Circular(4f, 4f);
                     int d = Dust.NewDust(target.Center, 0, 0, DustID.Smoke, speed.X, speed.Y, 0, new Color(100, 0, 200), 1.5f);
                     Main.dust[d].velocity *= 0.8f;
                     Main.dust[d].noGravity = true;
                }

                CombatText.NewText(target.getRect(), new Color(160, 30, 255), "BOOOM!", true);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 120);

            Dust.NewDust(target.position, target.width, target.height, DustID.Shadowflame, 0f, 0f, 100, default, 1f);
        }
    }
}