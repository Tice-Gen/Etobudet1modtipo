using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class AdamantiteYoyoProj : ModProjectile
    {
        private const int MaxOrbitingShards = 10;
        private const int ShardSpawnInterval = 35;
        private const float ScatterSpeed = 18f;
        private const float EnemyLaunchSpeed = 20f;

        private int shardSpawnTimer;
        private bool launchedToCursor;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 480f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 19f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
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
            TryLaunchShardsOnRecall();

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.channel)
                return;

            shardSpawnTimer++;
            if (shardSpawnTimer < ShardSpawnInterval)
                return;

            shardSpawnTimer = 0;
            if (CountOrbitingShards() >= MaxOrbitingShards)
                return;

            int damage = Projectile.damage / 2;
            if (damage < 1)
                damage = 1;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<AdamantiteShard>(),
                damage,
                Projectile.knockBack,
                Projectile.owner,
                Projectile.whoAmI,
                0f,
                0f
            );
        }

        private void TryLaunchShardsOnRecall()
        {
            if (launchedToCursor)
                return;
            if (Projectile.owner != Main.myPlayer)
                return;

            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || owner.channel)
                return;

            int orbitingCount = CountOrbitingShards();
            if (orbitingCount <= 0)
                return;

            bool launchedAny = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<AdamantiteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                NPC target = FindNearestTarget(p.Center);
                if (target != null)
                {
                    Vector2 toTarget = target.Center - p.Center;
                    Vector2 direction = toTarget.SafeNormalize(Vector2.UnitY);

                    p.ai[2] = 1f;
                    p.ai[1] = orbitingCount;
                    p.velocity = direction * EnemyLaunchSpeed;
                    p.timeLeft = 180;
                    p.netUpdate = true;
                    launchedAny = true;
                }
                else
                {
                    SpawnAdamantiteDustAt(p.Center, 10);
                    p.Kill();
                }
            }

            if (launchedAny)
                launchedToCursor = true;
        }

        public override void OnKill(int timeLeft)
        {
            SpawnAdamantiteDust(20);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int orbitingCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<AdamantiteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                orbitingCount++;
            }

            if (orbitingCount <= 0)
                return;

            int launched = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<AdamantiteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                float angle = MathHelper.TwoPi * launched / orbitingCount;
                Vector2 dir = angle.ToRotationVector2();

                p.ai[2] = 1f;
                p.ai[1] = orbitingCount;
                p.velocity = dir * ScatterSpeed;
                p.timeLeft = 180;
                p.netUpdate = true;
                launched++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/YoyoHit")
            {
                Volume = 0.9f,
                Pitch = Main.rand.NextFloat(-0.04f, 0.04f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);

            SpawnAdamantiteDust(14);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.ScalingArmorPenetration += 0.85f;


            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                float currentMultiplier = target.takenDamageMultiplier;
                float targetMultiplier = 1f - (1f - currentMultiplier) * 0.5f;
                modifiers.FinalDamage *= targetMultiplier / currentMultiplier;
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
                float scale = Projectile.scale * (0.95f + 0.05f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        private int CountOrbitingShards()
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<AdamantiteShard>())
                    continue;
                if ((int)p.ai[0] != Projectile.whoAmI || p.ai[2] != 0f)
                    continue;

                count++;
            }

            return count;
        }

        private void SpawnAdamantiteDust(int amount)
        {
            Vector2 dustPos = Projectile.Center - new Vector2(8f, 8f);
            for (int i = 0; i < amount; i++)
            {
                Dust.NewDust(dustPos, 16, 16, DustID.Adamantite);
            }
        }

        private static void SpawnAdamantiteDustAt(Vector2 center, int amount)
        {
            Vector2 dustPos = center - new Vector2(6f, 6f);
            for (int i = 0; i < amount; i++)
            {
                Dust.NewDust(dustPos, 12, 12, DustID.Adamantite);
            }
        }

        private static NPC FindNearestTarget(Vector2 from)
        {
            NPC bestTarget = null;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;

                float distSq = Vector2.DistanceSquared(from, npc.Center);
                if (distSq >= bestDistSq)
                    continue;

                bestDistSq = distSq;
                bestTarget = npc;
            }

            return bestTarget;
        }
    }
}
