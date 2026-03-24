using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public abstract class BaseMetalLineShard : ModProjectile
    {
        protected abstract int ParentProjectileType { get; }
        protected abstract int TotalShardCount { get; }
        protected abstract float OrbitRadius { get; }
        protected abstract float OrbitSpeed { get; }
        protected abstract string HitSoundPath { get; }
        protected abstract int HitDustType { get; }
        protected abstract Color LightColor { get; }

        protected virtual int TrailCacheLength => 5;
        protected virtual int ProjectileSize => 14;
        protected virtual int LocalNpcCooldown => 12;
        protected virtual float FollowLerp => 0.28f;
        protected virtual float SnapDistanceSq => 900f;
        protected virtual bool UseDepthVisuals => true;
        protected virtual bool UseInstantOrbitFollow => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailCacheLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = ProjectileSize;
            Projectile.height = ProjectileSize;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = LocalNpcCooldown;
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
            if (!parent.active || parent.type != ParentProjectileType || parent.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.2f);

            int slot = ((int)Projectile.ai[1] % TotalShardCount + TotalShardCount) % TotalShardCount;
            float angleOffset = MathHelper.TwoPi * slot / TotalShardCount;
            float angle = Main.GameUpdateCount * OrbitSpeed + angleOffset;
            Vector2 targetOffset = angle.ToRotationVector2() * OrbitRadius;
            Vector2 targetCenter = parent.Center + targetOffset;

            if (UseInstantOrbitFollow)
            {
                Projectile.Center = targetCenter;
            }
            else if (Vector2.DistanceSquared(Projectile.Center, targetCenter) > SnapDistanceSq)
            {
                Projectile.Center = targetCenter;
            }
            else
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter, FollowLerp);
            }

            if (UseDepthVisuals)
            {
                float depth = MathHelper.Clamp(targetOffset.Y / OrbitRadius * 0.5f + 0.5f, 0f, 1f);
                Projectile.scale = MathHelper.Lerp(0.9f, 1.08f, depth);
                Projectile.alpha = (int)MathHelper.Lerp(70f, 10f, depth);
            }
            else
            {
                Projectile.scale = 1f;
                Projectile.alpha = 0;
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.33f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle hitSound = new SoundStyle(HitSoundPath)
            {
                Volume = 0.9f,
                Pitch = Main.rand.NextFloat(-0.04f, 0.04f),
                PitchVariance = 0f,
                MaxInstances = 24
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);

            SpawnDustBurst(7, 2.4f);
        }

        public override void OnKill(int timeLeft)
        {
            SpawnDustBurst(5, 1.6f);
        }

        private void SpawnDustBurst(int amount, float speed)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(speed, speed);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, HitDustType, velocity, 100, default, 0.95f);
                dust.noGravity = true;
            }
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
                Color color = lightColor * (0.28f * progress);
                float scale = Projectile.scale * (0.88f + 0.12f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
