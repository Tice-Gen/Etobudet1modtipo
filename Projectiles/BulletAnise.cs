using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class BulletAnise : ModProjectile
    {
        public override void SetStaticDefaults() 
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 6000;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
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
                

                Color color = Projectile.GetAlpha(new Color(139, 69, 19, 0)) * progress * 0.5f;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * progress, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 600);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 600);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 150, default, Main.rand.NextFloat(0.6f, 1.0f));
                Main.dust[d].noGravity = false;
                Main.dust[d].fadeIn = 0.8f;
            }
        }
    }
}