using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class WeekPlasmaSwingProjectile : ModProjectile
    {
        private const float SWINGRANGE = 1.67f * (float)Math.PI;
        private const float FIRSTHALFSWING = 0.45f;
        private const float WINDUP = 0.15f;
        private const float UNWIND = 0.4f;
        private const float REACH_SCALE = 1.7f;
        private const float BASE_REACH_MULTIPLIER = 1.2f * REACH_SCALE;
        private const float EXECUTE_REACH_MULTIPLIER = 1.78f * REACH_SCALE;
        private const int PULSE_DELAY_TICKS = 10;
        private const float PULSE_SPEED = 14f;
        private const float PULSE_DAMAGE_MULTIPLIER = 0.85f;

        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }

        private AttackStage CurrentStage {
            get => (AttackStage)Projectile.localAI[0];
            set {
                Projectile.localAI[0] = (float)value;
                Timer = 0;
            }
        }

        private ref float InitialAngle => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];
        private ref float Progress => ref Projectile.localAI[1];
        private ref float Size => ref Projectile.localAI[2];
        private bool pulseShot;

        private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        public override string Texture => "Etobudet1modtipo/items/WeekPlasmaSword";
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (Projectile.spriteDirection == 1) {
                targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI / 3, (float)Math.PI / 6);
            }
            else {
                if (targetAngle < 0f)
                    targetAngle += 2f * (float)Math.PI;

                targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5f / 6f, (float)Math.PI * 4f / 3f);
            }

            InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection;
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

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }

            switch (CurrentStage) {
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

            TryShootPulseWithDelay();
            SetSwordPosition();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0) {
                origin = new Vector2(0f, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color trailColor = new Color(90, 220, 255, 0);

            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--) {
                Vector2 oldPos = Projectile.oldPos[i];
                if (oldPos == Vector2.Zero)
                    continue;

                float progress = i / (float)Projectile.oldPos.Length;
                float alpha = (1f - progress) * 0.4f;
                float scale = Projectile.scale * (0.82f + (1f - progress) * 0.18f);
                float rotation = Projectile.oldRot[i] + rotationOffset;

                Main.spriteBatch.Draw(texture, oldPos + Projectile.Size * 0.5f - Main.screenPosition, null, trailColor * alpha, rotation, origin, scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, trailColor * 0.22f, Projectile.rotation + rotationOffset, origin, Projectile.scale * 1.08f, effects, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15f * Projectile.scale, DelegateMethods.CutTiles);
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

        private void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (Owner.gravDir == -1f) {
                Projectile.rotation = -Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            float reachMultiplier = BASE_REACH_MULTIPLIER;
            if (CurrentStage == AttackStage.Execute) {
                reachMultiplier = MathHelper.SmoothStep(BASE_REACH_MULTIPLIER, EXECUTE_REACH_MULTIPLIER, Timer / execTime);
            }
            else if (CurrentStage == AttackStage.Unwind) {
                reachMultiplier = MathHelper.SmoothStep(EXECUTE_REACH_MULTIPLIER, BASE_REACH_MULTIPLIER, Timer / hideTime);
            }

            Projectile.scale = Size * reachMultiplier * Owner.GetAdjustedItemScale(Owner.HeldItem);
            Owner.heldProj = Projectile.whoAmI;
        }

        private void PrepareStrike()
        {
            Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime);
            Size = MathHelper.SmoothStep(0f, 1f, Timer / prepTime);

            if (Timer >= prepTime) {
                SoundEngine.PlaySound(SoundID.Item1);
                CurrentStage = AttackStage.Execute;
            }
        }

        private void ExecuteStrike()
        {
            Progress = MathHelper.SmoothStep(0f, SWINGRANGE, (1f - UNWIND) * Timer / execTime);
            Size = 1f;

            if (Timer >= execTime)
                CurrentStage = AttackStage.Unwind;
        }

        private void UnwindStrike()
        {
            Progress = MathHelper.SmoothStep(0f, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
            Size = 1f - MathHelper.SmoothStep(0f, 1f, Timer / hideTime);

            if (Timer >= hideTime)
                Projectile.Kill();
        }

        private void TryShootPulseWithDelay()
        {
            if (pulseShot || Timer < PULSE_DELAY_TICKS || Main.myPlayer != Projectile.owner)
                return;

            pulseShot = true;

            Vector2 pulseVelocity = Owner.DirectionTo(Main.MouseWorld);
            if (pulseVelocity == Vector2.Zero)
                pulseVelocity = Projectile.rotation.ToRotationVector2();

            int pulseDamage = (int)(Projectile.damage * PULSE_DAMAGE_MULTIPLIER);
            if (pulseDamage < 1)
                pulseDamage = 1;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Owner.MountedCenter,
                pulseVelocity * PULSE_SPEED,
                ModContent.ProjectileType<WeekPlasmaPulse>(),
                pulseDamage,
                Projectile.knockBack,
                Projectile.owner
            );
        }
    }
}
