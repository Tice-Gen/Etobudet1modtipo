using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Etobudet1modtipo.Common.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Projectiles
{
    public class PalladiumKunaiProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                float progress = (float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length;
                Color color = Projectile.GetAlpha(new Color(80, 220, 120, 120)) * progress;

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            PlayBrokenKunaiSound();

            for (int i = 0; i < 12; i++)
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Palladium,
                    Projectile.velocity.X * 0.3f,
                    Projectile.velocity.Y * 0.3f,
                    0,
                    default,
                    Main.rand.NextFloat(0.9f, 1.2f)
                );

                Main.dust[dustIndex].velocity *= 1.5f;
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].fadeIn = 0.8f;
                Main.dust[dustIndex].alpha = 0;
            }
        }

        private void PlayBrokenKunaiSound()
        {
            SoundStyle breakSound = new SoundStyle("Etobudet1modtipo/Sounds/BrokenKunai")
            {
                Volume = 0.62f,
                Pitch = 0.06f,
                PitchVariance = 0.1f,
                MaxInstances = 2,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            };
            ProjectileBreakSoundLimiter.TryPlayBrokenKunai(breakSound, Projectile.Center);
        }
    }
}
