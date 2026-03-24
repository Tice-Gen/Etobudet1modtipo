using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class BlueExpl : ModProjectile
    {
        private const int TotalFrames = 5;
        private const int FrameWidth = 177;
        private const int FrameHeight = 210;
        private const int LifetimeTicks = 25;
        private const int DamageStartTick = 2;
        private const int DamageEndTick = 11;
        private static readonly int[] FrameSequence = { 0, 1, 1, 2, 3, 4 };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = TotalFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = FrameWidth;
            Projectile.height = FrameHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = LifetimeTicks;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 1;
            Projectile.knockBack = 0f;
            Projectile.alpha = 255;
            Projectile.scale = 1.55f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = LifetimeTicks;
        }

        public override bool? CanDamage()
        {
            int age = GetAge();
            return age >= DamageStartTick && age <= DamageEndTick;
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Projectile.damage = 1;
            Projectile.originalDamage = 1;
            Projectile.knockBack = 0f;
            SyncBeamBounds(GetBloomStrength(0f));
            SpawnBurstDust();

            if (Main.dedServ)
            {
                return;
            }

            try
            {
                SoundEngine.PlaySound(new SoundStyle("Etobudet1modtipo/Sounds/SHOCK"), Projectile.Center);
            }
            catch
            {
                // Ignore missing sound asset; effect is optional.
            }
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            float progress = GetProgress();
            float bloom = GetBloomStrength(progress);
            Projectile.scale = MathHelper.Lerp(1.35f, 2.4f, bloom);
            SyncBeamBounds(bloom);

            Lighting.AddLight(Projectile.Center, 0.75f * bloom, 1.1f * bloom, 1.45f * bloom);
            AddAmbientDust(progress);

            float glowStrength = 1f - progress;
            AddBlockPiercingGlow(glowStrength);

            int age = GetAge();

            int sequenceStep = (int)((long)age * FrameSequence.Length / LifetimeTicks);
            if (sequenceStep >= FrameSequence.Length)
            {
                sequenceStep = FrameSequence.Length - 1;
            }

            Projectile.frame = FrameSequence[sequenceStep];

            TryApplyPercentDamage();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Hitbox;
        }

        private void TryApplyPercentDamage()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int age = GetAge();
            if (age < DamageStartTick || age > DamageEndTick)
                return;

            Rectangle hitbox = Projectile.Hitbox;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.lifeMax <= 0)
                    continue;

                if (target.friendly || target.dontTakeDamage || target.immortal)
                    continue;

                if (Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (!hitbox.Intersects(target.Hitbox))
                    continue;

                ApplyPercentDamage(target);
                Projectile.localNPCImmunity[i] = Projectile.localNPCHitCooldown;
            }
        }

        private void ApplyPercentDamage(NPC target)
        {
            int maxLifeDamage = System.Math.Max(1, target.lifeMax);
            int currentLifeDamage = System.Math.Max(1, target.life);
            int percentDamage = maxLifeDamage + currentLifeDamage;
            int hitDirection = Projectile.Center.X < target.Center.X ? 1 : -1;
            target.life -= percentDamage;

            if (target.life <= 0)
            {
                target.life = 0;
                target.HitEffect(hitDirection, percentDamage);
                target.checkDead();
            }
            else
            {
                target.HitEffect(hitDirection, percentDamage);
            }

            target.netUpdate = true;
        }

        private void AddBlockPiercingGlow(float glowStrength)
        {
            const int ringLights = 12;
            float radius = 96f * glowStrength;
            float r = 0.65f * glowStrength;
            float g = 1.05f * glowStrength;
            float b = 1.3f * glowStrength;

            for (int i = 0; i < ringLights; i++)
            {
                float t = MathHelper.TwoPi * i / ringLights;
                Vector2 offset = new Vector2(radius, 0f).RotatedBy(t);
                Lighting.AddLight(Projectile.Center + offset, r, g, b);
            }
        }

        private void SpawnBurstDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 18; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3.4f, 8.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, velocity, 120, new Color(90, 220, 255), Main.rand.NextFloat(1.2f, 1.9f));
                dust.noGravity = true;
            }
        }

        private void AddAmbientDust(float progress)
        {
            if (Main.dedServ || Main.rand.NextBool(3))
                return;

            float radius = MathHelper.Lerp(38f, 120f, progress) * Projectile.scale;
            Vector2 spawnOffset = Main.rand.NextVector2CircularEdge(radius, radius);
            Vector2 velocity = spawnOffset.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.8f, 2.2f);

            Dust dust = Dust.NewDustPerfect(Projectile.Center + spawnOffset, DustID.DungeonSpirit, velocity, 150, new Color(120, 240, 255), Main.rand.NextFloat(0.8f, 1.3f));
            dust.noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float progress = GetProgress();
            float bloom = GetBloomStrength(progress);
            float pulse = 0.9f + 0.1f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 8f + Projectile.whoAmI * 0.35f);
            float beamScale = MathHelper.Lerp(0.92f, 1.28f, bloom);

            DrawAnnihilationRays(drawPos, bloom, pulse, progress, beamScale);
            DrawColumnAura(drawPos, bloom, progress, beamScale);
            DrawColumnEdges(drawPos, bloom, pulse, beamScale);
            DrawImpactCore(drawPos, bloom, pulse, progress, beamScale);
            DrawColumnCore(drawPos, bloom, pulse, progress, beamScale);

            return false;
        }

        private void SyncBeamBounds(float bloom)
        {
            float beamScale = GetBeamScale(bloom);
            int width = (int)System.MathF.Ceiling(GetBeamHitboxWidth(bloom, beamScale));
            int height = (int)System.MathF.Ceiling(GetBeamHitboxHeight(beamScale));

            width = System.Math.Max(width, 1);
            height = System.Math.Max(height, 1);

            if (Projectile.width == width && Projectile.height == height)
                return;

            Vector2 center = Projectile.Center;
            Projectile.Resize(width, height);
            Projectile.Center = center;
        }

        private void DrawAnnihilationRays(Vector2 drawPos, float bloom, float pulse, float progress, float beamScale)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float maxLength = MathHelper.Lerp(380f, 1120f, bloom) * beamScale;
            float baseThickness = MathHelper.Lerp(9f, 22f, bloom) * beamScale;
            float spin = Main.GlobalTimeWrappedHourly * 0.06f;

            const int rayCount = 22;
            for (int i = 0; i < rayCount; i++)
            {
                float t = i / (float)rayCount;
                float angle = MathHelper.TwoPi * t + spin;
                float oscillation = 0.82f + 0.18f * (float)System.Math.Sin(i * 1.37f + Projectile.whoAmI * 0.41f);
                float length = maxLength * oscillation;
                float thickness = baseThickness * (0.8f + 0.6f * (1f - System.MathF.Abs((t * 2f) - 1f))) * pulse;
                Color rayColor = Color.Lerp(new Color(35, 95, 210, 0), new Color(165, 235, 255, 0), (i % 4) / 3f) * (0.12f * bloom);

                Main.EntitySpriteDraw(
                    pixel,
                    drawPos,
                    null,
                    rayColor,
                    angle,
                    new Vector2(0f, 0.5f),
                    new Vector2(length, thickness),
                    SpriteEffects.None,
                    0
                );
            }
        }

        private void DrawColumnAura(Vector2 drawPos, float bloom, float progress, float beamScale)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float beamHalfLength = Main.screenHeight * 0.78f + 220f * beamScale;
            float outerWidth = MathHelper.Lerp(95f, 170f, bloom) * beamScale;
            float innerWidth = outerWidth * 0.62f;

            Main.EntitySpriteDraw(
                pixel,
                drawPos,
                null,
                new Color(24, 88, 215, 0) * (0.16f * bloom),
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(outerWidth, beamHalfLength * 2f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                pixel,
                drawPos,
                null,
                new Color(115, 225, 255, 0) * (0.2f * bloom),
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(innerWidth, beamHalfLength * 1.92f),
                SpriteEffects.None,
                0
            );
        }

        private void DrawColumnEdges(Vector2 drawPos, float bloom, float pulse, float beamScale)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float beamHalfLength = Main.screenHeight * 0.72f + 180f * beamScale;
            float edgeOffset = MathHelper.Lerp(44f, 62f, bloom) * beamScale;
            float edgeWidth = MathHelper.Lerp(14f, 22f, bloom) * beamScale * pulse;

            for (int direction = -1; direction <= 1; direction += 2)
            {
                Main.EntitySpriteDraw(
                    pixel,
                    drawPos + new Vector2(edgeOffset * direction, 0f),
                    null,
                    new Color(80, 230, 255, 0) * (0.26f * bloom),
                    0f,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(edgeWidth, beamHalfLength * 1.8f),
                    SpriteEffects.None,
                    0
                );
            }
        }

        private void DrawImpactCore(Vector2 drawPos, float bloom, float pulse, float progress, float beamScale)
        {
            float flashRadiusX = MathHelper.Lerp(54f, 94f, bloom) * beamScale;
            float flashRadiusY = MathHelper.Lerp(46f, 80f, bloom) * beamScale;
            float ringRadius = MathHelper.Lerp(110f, 170f, bloom) * beamScale;

            DrawFilledEllipse(
                drawPos,
                ringRadius,
                ringRadius * 0.64f,
                new Color(36, 106, 220, 0),
                new Color(6, 18, 48, 0),
                0.16f * bloom
            );

            DrawFilledEllipse(
                drawPos,
                flashRadiusX,
                flashRadiusY,
                new Color(165, 240, 255, 0),
                new Color(40, 145, 255, 0),
                0.7f * bloom
            );

            DrawFilledEllipse(
                drawPos + new Vector2(0f, -2f) * beamScale,
                flashRadiusX * 0.58f * pulse,
                flashRadiusY * 0.5f * pulse,
                new Color(255, 255, 255, 0),
                new Color(205, 250, 255, 0),
                0.9f * bloom
            );

            float crossHeight = MathHelper.Lerp(26f, 52f, bloom) * beamScale;
            float crossWidth = MathHelper.Lerp(180f, 260f, bloom) * beamScale;
            DrawBeamRect(drawPos, crossWidth, crossHeight, new Color(180, 235, 255, 0) * (0.22f * bloom));
        }

        private void DrawColumnCore(Vector2 drawPos, float bloom, float pulse, float progress, float beamScale)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float beamHalfLength = Main.screenHeight * 0.72f + 160f * beamScale;
            float coreWidth = MathHelper.Lerp(26f, 46f, bloom) * beamScale * pulse;
            float whiteWidth = coreWidth * 0.58f;
            float flareHeight = MathHelper.Lerp(120f, 220f, bloom) * beamScale;

            Main.EntitySpriteDraw(
                pixel,
                drawPos,
                null,
                new Color(170, 238, 255, 0) * (0.34f * bloom),
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(coreWidth, beamHalfLength * 2f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                pixel,
                drawPos,
                null,
                new Color(255, 255, 255, 0) * (0.88f * bloom),
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(whiteWidth, beamHalfLength * 1.86f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                pixel,
                drawPos,
                null,
                new Color(215, 250, 255, 0) * (0.22f * bloom),
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(coreWidth * 1.75f, flareHeight),
                SpriteEffects.None,
                0
            );
        }

        private static void DrawFilledEllipse(Vector2 center, float radiusX, float radiusY, Color topColor, Color bottomColor, float opacity)
        {
            if (opacity <= 0f || radiusX <= 0f || radiusY <= 0f)
                return;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int steps = (int)System.MathF.Max(18f, radiusY * 1.35f);
            float lineHeight = System.MathF.Max(1.6f, radiusY * 2f / steps + 0.25f);

            for (int i = 0; i <= steps; i++)
            {
                float lerp = i / (float)steps;
                float normalizedY = lerp * 2f - 1f;
                float halfWidth = radiusX * System.MathF.Sqrt(System.MathF.Max(0f, 1f - normalizedY * normalizedY));
                if (halfWidth <= 0.5f)
                    continue;

                float y = MathHelper.Lerp(-radiusY, radiusY, lerp);
                Color lineColor = Color.Lerp(topColor, bottomColor, lerp) * opacity;

                Main.EntitySpriteDraw(
                    pixel,
                    center + new Vector2(0f, y),
                    null,
                    lineColor,
                    0f,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(halfWidth * 2f, lineHeight),
                    SpriteEffects.None,
                    0
                );
            }
        }

        private static void DrawBeamRect(Vector2 center, float width, float height, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Main.EntitySpriteDraw(
                pixel,
                center,
                null,
                color,
                0f,
                new Vector2(0.5f, 0.5f),
                new Vector2(width, height),
                SpriteEffects.None,
                0
            );
        }

        private int GetAge()
        {
            int age = LifetimeTicks - Projectile.timeLeft;
            return age < 0 ? 0 : age;
        }

        private float GetProgress()
        {
            return GetAge() / (float)(LifetimeTicks - 1);
        }

        private static float GetBloomStrength(float progress)
        {
            float expand = Utils.GetLerpValue(0f, 0.25f, progress, true);
            float fade = Utils.GetLerpValue(1f, 0.45f, progress, true);
            return expand * fade;
        }

        private static float GetBeamScale(float bloom)
        {
            return MathHelper.Lerp(0.92f, 1.28f, bloom);
        }

        private static float GetBeamHitboxWidth(float bloom, float beamScale)
        {
            float outerWidth = MathHelper.Lerp(95f, 170f, bloom) * beamScale;
            float impactWidth = MathHelper.Lerp(110f, 170f, bloom) * beamScale * 2f;
            return System.MathF.Max(outerWidth, impactWidth);
        }

        private static float GetBeamHitboxHeight(float beamScale)
        {
            float beamHalfLength = Main.screenHeight * 0.78f + 220f * beamScale;
            return beamHalfLength * 2f;
        }
    }
}
