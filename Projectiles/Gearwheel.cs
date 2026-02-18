using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;

namespace Etobudet1modtipo.Projectiles
{
    public class Gearwheel : ModProjectile
    {
        private bool hitNpc;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = 2;
            Projectile.DamageType = DamageClass.Melee;
            AIType = ProjectileID.StarAnise;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitNpc = true;

            target.AddBuff(BuffID.Dazed, 60);


            if (Projectile.owner == Main.myPlayer && Main.netMode != NetmodeID.Server)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(
                    Projectile.Center,
                    direction,
                    8f,
                    6f,
                    60,
                    1000f
                ));
            }

            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/MetalHit")
            {
                Volume = 0.85f,
                Pitch = Main.rand.NextFloat(-0.15f, 0f),
                PitchVariance = 0f
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);

            SpawnMercuryDust(20, 2.0f);
            SpawnTorchDust(12, 1.4f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float multiplier = 1f;

            if (target.boss)
                multiplier *= 1.5f;

            if (target.life == target.lifeMax)
                multiplier *= 2f;

            if (multiplier != 1f)
                modifiers.SourceDamage *= multiplier;
        }

        public override void OnKill(int timeLeft)
        {
            if (!hitNpc)
            {
                SoundStyle failSound = new SoundStyle("Etobudet1modtipo/Sounds/MetalHit_fail")
                {
                    Volume = 0.85f,
                    Pitch = Main.rand.NextFloat(-0.12f, 0f),
                    PitchVariance = 0f,
                    MaxInstances = 3
                };
                SoundEngine.PlaySound(failSound, Projectile.Center);
            }

            SpawnMercuryDust(12, 1.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float t = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color color = Projectile.GetAlpha(lightColor) * (0.55f * t);
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    effects,
                    0
                );
            }

            return true;
        }

        private void SpawnMercuryDust(int count, float velocityScale)
        {
            for (int i = 0; i < count; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Mercury,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    default,
                    1.2f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= velocityScale;
            }
        }

        private void SpawnTorchDust(int count, float velocityScale)
        {
            for (int i = 0; i < count; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    Projectile.velocity.X * 0.15f,
                    Projectile.velocity.Y * 0.15f,
                    80,
                    default,
                    1.15f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= velocityScale;
                Main.dust[dust].scale *= 1.1f;
                Main.dust[dust].fadeIn = 1.1f;
            }
        }
    }
}
