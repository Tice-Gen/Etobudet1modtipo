using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class ChlorophyteYoyoProj : ModProjectile
    {
        private const int MinOrbitingShards = 3;
        private const int MaxOrbitingShards = 12;
        private const int ShardJoinInterval = 10;
        private const float BurstSpeed = 11f;
        private const int BurstLifetime = 45;
        private bool spawnedShards;
        private bool launchedOnRelease;
        private Vector2 cachedVelocity;
        private int shardJoinTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 500f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.velocity.LengthSquared() > 0.0001f)
                cachedVelocity = Projectile.velocity;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
                return;

            if (!spawnedShards)
            {
                spawnedShards = true;
                SpawnOrbitingShards(MinOrbitingShards);
                shardJoinTimer = 0;
                launchedOnRelease = false;
                return;
            }

            if (!owner.channel)
            {
                if (launchedOnRelease)
                    return;

                BurstCurrentRing();
                launchedOnRelease = true;
                return;
            }

            launchedOnRelease = false;

            int orbitingCount = CountOrbitingShards();
            if (orbitingCount <= 0)
            {
                SpawnOrbitingShards(MinOrbitingShards);
                shardJoinTimer = 0;
                return;
            }

            if (orbitingCount >= MaxOrbitingShards)
                return;

            shardJoinTimer++;
            if (shardJoinTimer < ShardJoinInterval)
                return;

            shardJoinTimer = 0;
            SpawnOrbitingShards(1);
        }

        private void SpawnOrbitingShards(int amount)
        {
            int shardDamage = Projectile.damage / 2;
            if (shardDamage < 1)
                shardDamage = 1;

            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ChlorophyteShard>(),
                    shardDamage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI,
                    Main.rand.Next(MaxOrbitingShards),
                    0f
                );
            }
        }

        private void BurstCurrentRing()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<ChlorophyteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                Vector2 dir = p.Center - Projectile.Center;
                if (dir.LengthSquared() < 0.001f)
                {
                    float fallbackAngle = MathHelper.TwoPi * ((int)p.ai[1] % MaxOrbitingShards) / MaxOrbitingShards;
                    dir = fallbackAngle.ToRotationVector2();
                }
                else
                {
                    dir.Normalize();
                }

                p.ai[2] = 1f;
                p.velocity = dir * BurstSpeed;
                p.timeLeft = BurstLifetime;
                p.netUpdate = true;
            }
        }

        private int CountOrbitingShards()
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<ChlorophyteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                count++;
            }

            return count;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<ChlorophyteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI)
                    continue;

                p.Kill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.ScalingArmorPenetration += 0.99f;


            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                float currentMultiplier = target.takenDamageMultiplier;
                float targetMultiplier = 1f - (1f - currentMultiplier) * 0.01f;
                modifiers.FinalDamage *= targetMultiplier / currentMultiplier;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/MetalHit_Mega")
            {
                Volume = 0.95f,
                Pitch = Main.rand.NextFloat(-0.03f, 0.03f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);


            if (cachedVelocity.LengthSquared() > 0.0001f)
                Projectile.velocity = cachedVelocity;
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
                Color color = lightColor * (0.22f * progress);
                float scale = Projectile.scale * (0.92f + 0.08f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
