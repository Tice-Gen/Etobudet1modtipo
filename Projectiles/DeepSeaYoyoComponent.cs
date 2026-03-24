using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaYoyoComponent : ModProjectile
    {
        private const float BaseOrbitRadius = 82f;
        private const float BaseOrbitSpeed = 0.155f;
        private const int ComponentCount = 3;
        private const float RadiusDeviationAmplitude = 4f;
        private const float AngleDeviationAmplitude = 0.08f;
        private bool initialized;
        private float randomPhaseA;
        private float randomPhaseB;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[parentIndex];
            if (!parent.active || parent.type != ModContent.ProjectileType<DeepSeaYoyoProj>() || parent.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            if (!initialized)
            {
                initialized = true;
                uint seed = (uint)(parent.identity * 73428767) ^ (uint)(Projectile.owner * 912931) ^ 0xA1F34D5u;
                randomPhaseA = HashToUnit(seed) * MathHelper.TwoPi;
                randomPhaseB = HashToUnit(seed ^ 0x9E3779B9u) * MathHelper.TwoPi;
                Projectile.Center = parent.Center;
            }

            float time = Main.GameUpdateCount;
            float baseAngle = time * BaseOrbitSpeed;
            int slot = (int)Projectile.ai[1] % ComponentCount;
            float slotOffset = MathHelper.TwoPi * slot / ComponentCount;

            float routeTime = time * 0.028f;
            float radiusDeviation =
                ((float)System.Math.Sin(routeTime + randomPhaseA) * 0.65f +
                 (float)System.Math.Sin(routeTime * 0.63f + randomPhaseB) * 0.35f) * RadiusDeviationAmplitude;
            float angleDeviation =
                ((float)System.Math.Cos(routeTime * 0.74f + randomPhaseA * 0.7f) * 0.6f +
                 (float)System.Math.Sin(routeTime * 0.41f + randomPhaseB * 1.2f) * 0.4f) * AngleDeviationAmplitude;

            Vector2 radialDir = (baseAngle + slotOffset + angleDeviation).ToRotationVector2();
            Vector2 targetPos = parent.Center + radialDir * (BaseOrbitRadius + radiusDeviation);

            Vector2 newVelocity = targetPos - Projectile.Center;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = targetPos;
            if (newVelocity.LengthSquared() > 0.001f)
            {
                Projectile.rotation = newVelocity.ToRotation() + MathHelper.PiOver2;
            }

            Lighting.AddLight(Projectile.Center, 0.08f, 0.36f, 0.52f);
        }

        private static float HashToUnit(uint x)
        {
            x ^= x >> 16;
            x *= 0x7FEB352Du;
            x ^= x >> 15;
            x *= 0x846CA68Bu;
            x ^= x >> 16;
            return (x & 0x00FFFFFFu) / 16777216f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 0.25f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item85 with
            {
                Volume = 0.95f,
                Pitch = Main.rand.NextFloat(-0.06f, 0.06f)
            }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(90, 220, 255, 0) * (0.45f * progress);
                float scale = Projectile.scale * (0.8f + 0.2f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, trailColor, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            Color glow = new Color(155, 245, 255, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, glow, Projectile.rotation, origin, Projectile.scale * 1.12f, SpriteEffects.None, 0);
            return true;
        }
    }
}
