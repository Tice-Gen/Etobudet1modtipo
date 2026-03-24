using System;
using System.IO;
using Etobudet1modtipo.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class SpytheSwingProjectile : ModProjectile
    {
        private const float SwingRange = 1.67f * (float)Math.PI;
        private const float FirstHalfSwing = 0.45f;
        private const float Windup = 0.18f;
        private const float Unwind = 0.4f;
        private const float BaseReachMultiplier = 1.9f;
        private const float ExecuteReachMultiplier = 2.7f;
        private const float TipReachFactor = 0.56f;
        private const float HitboxThickness = 90f;
        private const float HitboxLengthBonus = 50f;
        private const float PrepareBaseTime = 14f;
        private const float ExecuteBaseTime = 12f;
        private const float UnwindBaseTime = 18f;
        private const float NoiseTrailWidth = 52f;
        private const int NoiseBandHeight = 24;
        private static readonly Color TrailColor = new(180, 110, 255, 0);

        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }

        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0f;
            }
        }

        private Player Owner => Main.player[Projectile.owner];
        private ref float InitialAngle => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[1];
        private ref float Progress => ref Projectile.localAI[1];
        private ref float Size => ref Projectile.localAI[2];

        public override string Texture => "Etobudet1modtipo/items/Spythe";

        private float PrepareTime => PrepareBaseTime / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float ExecuteTime => ExecuteBaseTime / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float UnwindTime => UnwindBaseTime / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 89;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                targetAngle = MathHelper.Clamp(targetAngle, -MathHelper.Pi / 3f, MathHelper.Pi / 6f);
            }
            else
            {
                if (targetAngle < 0f)
                    targetAngle += MathHelper.TwoPi;

                targetAngle = MathHelper.Clamp(targetAngle, MathHelper.Pi * 5f / 6f, MathHelper.Pi * 4f / 3f);
            }

            InitialAngle = targetAngle - FirstHalfSwing * SwingRange * Projectile.spriteDirection;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<items.Spythe>())
            {
                Projectile.Kill();
                return;
            }

            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetScythePosition();
            EmitSwingEffects();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(12f, texture.Height - 9f);
                rotationOffset = MathHelper.ToRadians(42f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(texture.Width - 12f, texture.Height - 9f);
                rotationOffset = MathHelper.ToRadians(138f);
                effects = SpriteEffects.FlipHorizontally;
            }

            DrawNoiseTrail();

            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                if (oldPos == Vector2.Zero)
                    continue;

                float progress = i / (float)Projectile.oldPos.Length;
                float alpha = (1f - progress) * 0.32f;
                float scale = Projectile.scale * (0.88f + (1f - progress) * 0.12f);
                float rotation = Projectile.oldRot[i] + rotationOffset;

                Main.EntitySpriteDraw(texture, oldPos + Projectile.Size * 0.5f - Main.screenPosition, null, TrailColor * alpha, rotation, origin, scale, effects, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, TrailColor * 0.15f, Projectile.rotation + rotationOffset, origin, Projectile.scale * 1.04f, effects, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * GetTipReach();
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, HitboxThickness * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * GetTipReach();
            Utils.PlotTileLine(start, end, HitboxThickness * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;

            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<global::Etobudet1modtipo.Buffs.CosmolFire>(), 300);
        }

        private void SetScythePosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;
            Owner.ChangeDir(Projectile.spriteDirection);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 handPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = -Projectile.rotation;
                handPosition.Y = Owner.Bottom.Y + (Owner.position.Y - handPosition.Y);
            }

            handPosition.Y += Owner.gfxOffY;
            Projectile.Center = handPosition;

            float reachMultiplier = BaseReachMultiplier;
            if (CurrentStage == AttackStage.Execute)
            {
                reachMultiplier = MathHelper.SmoothStep(BaseReachMultiplier, ExecuteReachMultiplier, Timer / ExecuteTime);
            }
            else if (CurrentStage == AttackStage.Unwind)
            {
                reachMultiplier = MathHelper.SmoothStep(ExecuteReachMultiplier, BaseReachMultiplier, Timer / UnwindTime);
            }

            Projectile.scale = Size * reachMultiplier * Owner.GetAdjustedItemScale(Owner.HeldItem);
            Owner.heldProj = Projectile.whoAmI;
        }

        private void PrepareStrike()
        {
            float progress = Timer / PrepareTime;
            Progress = Windup * SwingRange * (1f - progress);
            Size = MathHelper.SmoothStep(0f, 1f, progress);

            if (Timer >= PrepareTime)
            {
                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.12f, Volume = 0.7f }, Owner.Center);
                CurrentStage = AttackStage.Execute;
            }
        }

        private void ExecuteStrike()
        {
            Progress = MathHelper.SmoothStep(0f, SwingRange, (1f - Unwind) * Timer / ExecuteTime);
            Size = 1f;

            if (Timer >= ExecuteTime)
                CurrentStage = AttackStage.Unwind;
        }

        private void UnwindStrike()
        {
            float unwindProgress = Timer / UnwindTime;
            Progress = MathHelper.SmoothStep(0f, SwingRange, (1f - Unwind) + Unwind * unwindProgress);

            if (unwindProgress < 0.82f)
            {
                Size = 1f;
            }
            else
            {
                float fadeProgress = (unwindProgress - 0.82f) / 0.18f;
                Size = 1f - MathHelper.SmoothStep(0f, 1f, fadeProgress);
            }

            if (Timer >= UnwindTime)
                Projectile.Kill();
        }

        private float GetTipReach()
        {
            return Projectile.Size.Length() * Projectile.scale * TipReachFactor + HitboxLengthBonus;
        }

        private Vector2 GetTrailPoint(Vector2 center, float rotation)
        {
            return center + rotation.ToRotationVector2() * GetTipReach() * 0.96f;
        }

        private void EmitSwingEffects()
        {
            Lighting.AddLight(Projectile.Center, 0.22f, 0.06f, 0.32f);

            int dustChance = CurrentStage == AttackStage.Execute ? 1 : 3;
            if (!Main.rand.NextBool(dustChance))
                return;

            Vector2 dustPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(18f, 70f) * Projectile.scale;
            Vector2 dustVelocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 2.2f);
            int dustType = Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.PurpleTorch;
            Dust dust = Dust.NewDustPerfect(dustPosition, dustType, dustVelocity, 110, new Color(210, 150, 255), Main.rand.NextFloat(1f, 1.35f));
            dust.noGravity = true;
        }

        private void DrawNoiseTrail()
        {
            if (CurrentStage == AttackStage.Prepare || SpytheTrailShaderSystem.NoiseTexture == null)
                return;

            Texture2D noiseTexture = SpytheTrailShaderSystem.NoiseTexture.Value;
            int pointCount = CollectTrailPoints(out Vector2[] points);
            if (pointCount < 2)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Additive,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.TransformationMatrix
            );

            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector2 start = points[i];
                Vector2 end = points[i + 1];
                Vector2 between = end - start;
                float length = between.Length();
                if (length <= 1f)
                    continue;

                float progress = i / (float)(pointCount - 1);
                float fade = 1f - Utils.GetLerpValue(0.55f, 1f, progress, true);
                float width = MathHelper.Lerp(NoiseTrailWidth, NoiseTrailWidth * 0.2f, progress) * Projectile.scale;
                float opacity = fade * Projectile.Opacity;
                float time = Main.GlobalTimeWrappedHourly * 70f + i * 19f;
                int sourceY = (int)(time % (noiseTexture.Height - NoiseBandHeight));
                Rectangle source = new Rectangle(0, sourceY, noiseTexture.Width, NoiseBandHeight);
                Vector2 drawPosition = (start + end) * 0.5f - Main.screenPosition;
                float rotation = between.ToRotation();

                DrawNoiseSegment(noiseTexture, source, drawPosition, rotation, length, width * 1.24f, new Color(62, 18, 110, 0) * (opacity * 0.05f));
                DrawNoiseSegment(noiseTexture, source, drawPosition, rotation, length, width * 0.88f, new Color(132, 74, 214, 0) * (opacity * 0.11f));
                DrawNoiseSegment(noiseTexture, source, drawPosition, rotation, length, width * 0.5f, new Color(245, 228, 255, 0) * (opacity * 0.07f));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.TransformationMatrix
            );
        }

        private static void DrawNoiseSegment(Texture2D noiseTexture, Rectangle source, Vector2 drawPosition, float rotation, float length, float width, Color color)
        {
            if (color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0)
                return;

            Main.EntitySpriteDraw(
                noiseTexture,
                drawPosition,
                source,
                color,
                rotation,
                new Vector2(source.Width * 0.5f, source.Height * 0.5f),
                new Vector2((length + width * 0.8f) / source.Width, width / source.Height),
                SpriteEffects.None,
                0f
            );
        }

        private int CollectTrailPoints(out Vector2[] points)
        {
            points = new Vector2[Projectile.oldPos.Length + 1];
            int count = 0;

            points[count++] = GetTrailPoint(Projectile.Center, Projectile.rotation);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    break;

                points[count++] = GetTrailPoint(Projectile.oldPos[i] + Projectile.Size * 0.5f, Projectile.oldRot[i]);
            }

            return count;
        }
    }
}
