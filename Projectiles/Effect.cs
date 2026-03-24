using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class Effect : ModProjectile
    {
        private const int LifetimeTicks = 40;
        private const int FadeOutTicks = 28;
        private bool playedSpawnSound;

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = LifetimeTicks;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.scale = 0.72f;
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            if (!playedSpawnSound && !Main.dedServ)
            {
                playedSpawnSound = true;
                SoundStyle bell = new SoundStyle("Etobudet1modtipo/Sounds/BellOfProphecy")
                {
                    Volume = 1f,
                    Pitch = -0.05f,
                    PitchVariance = 0f,
                    MaxInstances = 8
                };
                SoundEngine.PlaySound(bell, Projectile.Center);
            }

            int age = LifetimeTicks - Projectile.timeLeft;
            float lifeProgress = age / (float)LifetimeTicks;
            Projectile.scale = MathHelper.Lerp(0.72f, 1.22f, lifeProgress);

            float fade = 1f;
            if (Projectile.timeLeft < FadeOutTicks)
            {
                fade = Projectile.timeLeft / (float)FadeOutTicks;
            }

            Projectile.alpha = (int)MathHelper.Lerp(195f, 255f, 1f - fade);
            Projectile.rotation += 0.01f;
            Lighting.AddLight(Projectile.Center, 0.45f * fade, 0.16f * fade, 0.08f * fade);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = new Color(255, 180, 120, 0) * ((255 - Projectile.alpha) / 255f);
            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Type].Value,
                Projectile.Center - Main.screenPosition,
                null,
                color,
                Projectile.rotation,
                TextureAssets.Projectile[Type].Value.Size() * 0.5f,
                Projectile.scale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                0f);

            return false;
        }

        public override void DrawBehind(
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}
