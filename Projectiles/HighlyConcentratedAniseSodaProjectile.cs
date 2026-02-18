using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using System;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Projectiles
{
    public class HighlyConcentratedAniseSodaProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 28;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 3600;
            Projectile.aiStyle = 2;
            AIType = ProjectileID.StarAnise;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero)
                    continue;

                Vector2 drawPos =
                    Projectile.oldPos[k]
                    - Main.screenPosition
                    + Projectile.Size / 2f
                    + new Vector2(0f, Projectile.gfxOffY);

                float opacity = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                Color color = Projectile.GetAlpha(lightColor) * opacity * 0.5f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.oldRot[k],
                    drawOrigin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            int spawnedDamage = Projectile.damage / 2;
            float baseSpeed = 1f;
            int gasType = ModContent.ProjectileType<AnisePoisonGas>();

            Vector2[] dirs =
            {
                new Vector2(0f, -1f),
                new Vector2(0f, 1f),
                new Vector2(1f, 0f),
                new Vector2(-1f, 0f),
                new Vector2(1f, -1f),
                new Vector2(1f, 1f),
                new Vector2(-1f, -1f),
                new Vector2(-1f, 1f)
            };

            for (int i = 0; i < dirs.Length; i++)
            {
                Vector2 dir = Vector2.Normalize(dirs[i]);
                float angleOffset = MathHelper.ToRadians(Main.rand.NextFloat(-10f, 10f));
                dir = dir.RotatedBy(angleOffset);

                float speed = baseSpeed + Main.rand.NextFloat(-2f, 2f);
                if (speed < 0.2f)
                    speed = 0.2f;

                Vector2 velocity = dir * speed;

                Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    center,
                    velocity,
                    gasType,
                    spawnedDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }


            try
            {
                SoundStyle sound = new SoundStyle("Etobudet1modtipo/Sounds/AniseBurst")
                {
                    PitchVariance = 0.17f,
                    Volume = 1f
                };

                SoundEngine.PlaySound(sound, center);
            }
            catch
            {
                SoundEngine.PlaySound(SoundID.Item14, center);
            }

            for (int i = 0; i < 18; i++)
            {
                Dust.NewDust(
                    center - new Vector2(8, 8),
                    16,
                    16,
                    DustID.FireworkFountain_Yellow
                );
            }
        }
    }
}
