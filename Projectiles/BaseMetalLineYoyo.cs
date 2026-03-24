using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public abstract class BaseMetalLineYoyo : ModProjectile
    {
        private bool spawnedShards;

        protected abstract int ShardProjectileType { get; }
        protected abstract int ShardCount { get; }
        protected abstract float YoyoLifeTimeMultiplier { get; }
        protected abstract float YoyoMaximumRange { get; }
        protected abstract float YoyoTopSpeed { get; }
        protected abstract string HitSoundPath { get; }
        protected abstract int HitDustType { get; }
        protected abstract Color LightColor { get; }

        protected virtual int TrailCacheLength => 4;
        protected virtual int ProjectileSize => 24;
        protected virtual int LocalNpcCooldown => 10;
        protected virtual int ShardDamageDivisor => 3;
        protected virtual float HitSoundVolume => 0.9f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = YoyoLifeTimeMultiplier;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = YoyoMaximumRange;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = YoyoTopSpeed;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailCacheLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = ProjectileSize;
            Projectile.height = ProjectileSize;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = LocalNpcCooldown;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.25f);

            if (spawnedShards || Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                return;
            }

            spawnedShards = true;
            SpawnOrbitingShards();
        }

        private void SpawnOrbitingShards()
        {
            int shardDamage = Projectile.damage / ShardDamageDivisor;
            if (shardDamage < 1)
            {
                shardDamage = 1;
            }

            for (int i = 0; i < ShardCount; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ShardProjectileType,
                    shardDamage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI,
                    i
                );
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile child = Main.projectile[i];
                if (!child.active || child.owner != Projectile.owner)
                {
                    continue;
                }

                if (child.type != ShardProjectileType || (int)child.ai[0] != Projectile.whoAmI)
                {
                    continue;
                }

                child.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PlayHitSound();
            SpawnDustBurst(10, 2.8f);
        }

        private void PlayHitSound()
        {
            SoundStyle hitSound = new SoundStyle(HitSoundPath)
            {
                Volume = HitSoundVolume,
                Pitch = Main.rand.NextFloat(-0.03f, 0.03f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);
        }

        protected void SpawnDustBurst(int amount, float speed)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(speed, speed);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, HitDustType, velocity, 100, default, 1.05f);
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
                Color color = lightColor * (0.2f * progress);
                float scale = Projectile.scale * (0.94f + 0.06f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
