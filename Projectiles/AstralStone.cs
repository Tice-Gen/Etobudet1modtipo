using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Etobudet1modtipo.Common.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public abstract class AstralStoneBase : ModProjectile
    {
        protected const int DeathDustType = 272;

        protected abstract int FrameCount { get; }
        protected abstract int ProjectileWidth { get; }
        protected abstract int ProjectileHeight { get; }
        protected abstract int TrailLength { get; }
        protected abstract Color TrailColor { get; }
        protected abstract Color GlowColor { get; }
        protected abstract Vector3 LightColor { get; }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = FrameCount;
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = FrameCount > 1 ? Main.rand.Next(FrameCount) : 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnKill(int timeLeft)
        {
            SpawnDeathDust();
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

                Color trail = TrailColor * (0.55f * progress);
                float scale = Projectile.scale * (0.88f + progress * 0.24f);
                Main.EntitySpriteDraw(texture, oldDrawPos, frame, trail, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                float angle = MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 2.2f;
                Vector2 glowOffset = angle.ToRotationVector2() * 1.5f;
                Main.EntitySpriteDraw(
                    texture,
                    drawPos + glowOffset,
                    frame,
                    GlowColor * 0.35f,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.05f,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(texture, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        protected void AddLight()
        {
            Lighting.AddLight(Projectile.Center, LightColor);
        }

        private void SpawnDeathDust()
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3.6f, 3.6f) + Projectile.velocity * 0.12f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DeathDustType, velocity, 90, default, Main.rand.NextFloat(0.9f, 1.35f));
                dust.noGravity = true;
            }
        }
    }

    public class AstralStone : AstralStoneBase
    {
        protected override int FrameCount => 1;
        protected override int ProjectileWidth => 76;
        protected override int ProjectileHeight => 55;
        protected override int TrailLength => 14;
        protected override Color TrailColor => new Color(120, 120, 255, 0);
        protected override Color GlowColor => new Color(190, 120, 255, 0);
        protected override Vector3 LightColor => new Vector3(0.28f, 0.15f, 0.42f);

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            AddLight();
            if (Projectile.velocity.LengthSquared() > 0.001f)
                Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            PlayBrokenStoneSound();
            AddDeathShake();

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float baseAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 5; i++)
            {
                float angle = baseAngle + MathHelper.TwoPi * i / 5f + Main.rand.NextFloat(-0.12f, 0.12f);
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(6.2f, 8.8f);
                int index = Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<AstralStone2>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );

                if (index >= 0 && index < Main.maxProjectiles)
                {
                    Main.projectile[index].frame = Main.rand.Next(3);
                    Main.projectile[index].netUpdate = true;
                }
            }
        }

        private void PlayBrokenStoneSound()
        {
            SoundStyle breakSound = new SoundStyle("Etobudet1modtipo/Sounds/BrokenStone")
            {
                Volume = 0.7f,
                Pitch = -0.08f,
                PitchVariance = 0.12f,
                MaxInstances = 2,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            };
            ProjectileBreakSoundLimiter.TryPlayBrokenStone(breakSound, Projectile.Center);
        }

        private void AddDeathShake()
        {
            if (Main.dedServ)
                return;

            Player localPlayer = Main.LocalPlayer;
            float maxDistance = 240f * 16f;
            float distance = Vector2.Distance(localPlayer.Center, Projectile.Center);
            if (distance > maxDistance)
                return;

            float distanceLerp = MathHelper.Clamp(distance / maxDistance, 0f, 1f);
            float strength = MathHelper.Lerp(28f, 8f, distanceLerp);
            Vector2 direction = Main.rand.NextVector2Unit();
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(
                Projectile.Center,
                direction,
                strength,
                9f,
                28,
                1000f,
                "AstralStoneDeath"));
        }
    }

    public class AstralStone2 : AstralStoneBase
    {
        protected override int FrameCount => 3;
        protected override int ProjectileWidth => 26;
        protected override int ProjectileHeight => 16;
        protected override int TrailLength => 10;
        protected override Color TrailColor => new Color(150, 110, 255, 0);
        protected override Color GlowColor => new Color(215, 165, 255, 0);
        protected override Vector3 LightColor => new Vector3(0.2f, 0.12f, 0.34f);

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 2f;
        }

        public override void OnSpawn(IEntitySource source)
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
