using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Common.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class CosmicKunai : ModProjectile
    {
        private const int FrameCount = 3;
        private const int TrailLength = 10;
        private const int ShardCount = 5;
        private const float HomingRange = 650f;
        private const float HomingSpeed = 12f;
        private const float HomingInertia = 28f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = FrameCount;
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 55;
            Projectile.height = 55;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.22f, 0.28f, 0.42f);

            NPC target = FindClosestTarget();
            if (target != null)
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * System.Math.Max(Projectile.velocity.Length(), HomingSpeed);
                Projectile.velocity = (Projectile.velocity * (HomingInertia - 1f) + desiredVelocity) / HomingInertia;
            }

            if (Projectile.velocity.LengthSquared() > 0.001f)
                Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % FrameCount;
            }
        }

        private NPC FindClosestTarget()
        {
            NPC chosenTarget = null;
            float closestDistanceSquared = HomingRange * HomingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(this))
                    continue;

                float distanceSquared = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    chosenTarget = npc;
                }
            }

            return chosenTarget;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, FrameCount, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 oldDrawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                oldDrawPos.Y += Projectile.gfxOffY;

                Color trailColor = new Color(110, 180, 255, 0) * (0.55f * progress);
                float trailScale = Projectile.scale * (0.9f + progress * 0.2f);
                Main.EntitySpriteDraw(texture, oldDrawPos, frame, trailColor, Projectile.oldRot[i], origin, trailScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                float angle = MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 2.4f;
                Vector2 glowOffset = angle.ToRotationVector2() * 1.25f;
                Main.EntitySpriteDraw(
                    texture,
                    drawPos + glowOffset,
                    frame,
                    new Color(180, 120, 255, 0) * 0.3f,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(texture, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            PlayBrokenKunaiSound();

            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3.2f, 3.2f) + Projectile.velocity * 0.1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, velocity, 90, default, Main.rand.NextFloat(0.9f, 1.2f));
                dust.noGravity = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float baseAngle = Projectile.velocity.LengthSquared() > 0.001f
                ? Projectile.velocity.ToRotation()
                : Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < ShardCount; i++)
            {
                float angle = baseAngle + MathHelper.TwoPi * i / ShardCount + Main.rand.NextFloat(-0.12f, 0.12f);
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(5.4f, 7.6f);
                int shardIndex = Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<CosmicShard>(),
                    System.Math.Max(1, Projectile.damage / 4),
                    Projectile.knockBack,
                    Projectile.owner
                );

                if (shardIndex >= 0 && shardIndex < Main.maxProjectiles)
                    Main.projectile[shardIndex].netUpdate = true;
            }
        }

        private void PlayBrokenKunaiSound()
        {
            SoundStyle breakSound = new SoundStyle("Etobudet1modtipo/Sounds/BrokenKunai")
            {
                Volume = 0.62f,
                Pitch = -0.05f,
                PitchVariance = 0.1f,
                MaxInstances = 2,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            };
            ProjectileBreakSoundLimiter.TryPlayBrokenKunai(breakSound, Projectile.Center);
        }
    }

    public class CosmicShard : AstralStoneBase
    {
        protected override int FrameCount => 1;
        protected override int ProjectileWidth => 33;
        protected override int ProjectileHeight => 33;
        protected override int TrailLength => 10;
        protected override Color TrailColor => new Color(120, 205, 255, 0);
        protected override Color GlowColor => new Color(185, 135, 255, 0);
        protected override Vector3 LightColor => new Vector3(0.12f, 0.2f, 0.32f);

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.localAI[1] = Main.rand.NextBool() ? 1f : -1f;
        }

        public override void AI()
        {
            AddLight();

            float speed = Projectile.velocity.Length();
            if (speed > 0.25f)
            {
                Projectile.velocity *= 0.94f;
                Projectile.rotation += (0.12f + speed * 0.02f) * Projectile.localAI[1];

                if (Projectile.velocity.Length() < 0.35f)
                    Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                float snappedRotation = (float)System.Math.Round(Projectile.rotation / MathHelper.PiOver4) * MathHelper.PiOver4;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, snappedRotation, 0.08f);
            }
        }
    }
}
