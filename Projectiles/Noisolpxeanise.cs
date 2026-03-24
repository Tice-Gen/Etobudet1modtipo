using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class Noisolpxeanise : ModProjectile
    {
        private const float DistanceBeforeSlowdown = 320f;
        private const float SlowdownFactor = 0.90f;
        private const float StopSpeed = 0.45f;
        private const int WaitBeforeBurst = 60;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 2f ? false : null;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 1f)
            {
                Projectile.localAI[0] += Projectile.velocity.Length();
                if (Projectile.localAI[0] >= DistanceBeforeSlowdown)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.velocity *= SlowdownFactor;
                if (Projectile.velocity.Length() <= StopSpeed)
                {
                    EnterStopState();
                }
            }
            else if (Projectile.ai[0] >= 2f)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1]++;
                int extraDelay = Math.Max(0, (int)Projectile.ai[1]);
                if (Projectile.localAI[1] >= WaitBeforeBurst + extraDelay)
                {
                    Projectile.Kill();
                    return;
                }
            }

            if (Projectile.velocity.LengthSquared() > 0.001f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation += 0.18f;
            }

            Lighting.AddLight(Projectile.Center, 0.95f, 0.22f, 0.08f);

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Torch,
                    -Projectile.velocity * 0.07f + Main.rand.NextVector2Circular(0.8f, 0.8f),
                    100,
                    default,
                    Main.rand.NextFloat(0.95f, 1.2f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PlayLowMetalHit();
            AddImpactShake();
            EnterStopState();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            PlayLowMetalHit();
            EnterStopState();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Projectile.NewProjectile(
                Projectile.GetSource_Death(),
                Projectile.Center,
                new Vector2(0f, -1f),
                978,
                Projectile.damage,
                0f,
                Projectile.owner);
        }

        private void PlayLowMetalHit()
        {
            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/MetalHit")
            {
                Volume = 0.45f,
                Pitch = -0.42f,
                PitchVariance = 0.05f,
                MaxInstances = 16
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);
        }

        private void EnterStopState()
        {
            if (Projectile.ai[0] >= 2f)
            {
                return;
            }

            Projectile.ai[0] = 2f;
            Projectile.localAI[1] = 0f;
            Projectile.velocity = Vector2.Zero;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
        }

        private void AddImpactShake()
        {
            if (Main.dedServ || Projectile.owner != Main.myPlayer)
            {
                return;
            }

            float angle = Main.rand.NextFloat(MathHelper.TwoPi);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(
                Projectile.Center,
                direction,
                9f,
                7f,
                28,
                1000f,
                "NoisolpxeaniseHit"));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(255, 110, 45, 0) * (0.5f * progress);
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float scale = Projectile.scale * (0.85f + 0.2f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, trailColor, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
