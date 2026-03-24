using System;
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
    public class CosmicDevourerSwingProjectile : ModProjectile
    {
        private const float SwingRange = 1.67f * (float)Math.PI;
        private const float FirstHalfSwing = 0.45f;
        private const float WindupRatio = 0.18f;
        private const float RecoveryRatio = 0.18f;
        private const float BaseReachMultiplier = 0.92f;
        private const float ExecuteReachMultiplier = 1.44f;
        private const float DownStrikeProgress = SwingRange * 0.74f;
        private const float FollowThroughProgress = SwingRange * 0.84f;

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
        private ref float InitialAngle => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];
        private ref float Progress => ref Projectile.localAI[1];
        private ref float Size => ref Projectile.localAI[2];

        private bool kunaiShot;

        public override string Texture => "Etobudet1modtipo/items/CosmicDevourer";

        private float TotalSwingTime
        {
            get
            {
                int baseUseTime = Math.Max(Owner.HeldItem.useTime, Owner.HeldItem.useAnimation);
                return Math.Max(10f, baseUseTime) / Owner.GetTotalAttackSpeed(Projectile.DamageType);
            }
        }

        private float PrepareTime => Math.Max(2f, TotalSwingTime * WindupRatio);
        private float ExecuteTime => Math.Max(6f, TotalSwingTime * (1f - WindupRatio - RecoveryRatio));
        private float UnwindTime => Math.Max(3f, TotalSwingTime * RecoveryRatio);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 112;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
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

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<items.CosmicDevourer>())
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

            TryFireKunaiBurst();
            SetSwordPosition();
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
                origin = new Vector2(0f, texture.Height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(texture.Width, texture.Height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Color trailColor = new Color(196, 170, 255, 0);
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                if (oldPos == Vector2.Zero)
                    continue;

                float progress = i / (float)Projectile.oldPos.Length;
                float alpha = (1f - progress) * 0.34f;
                float scale = Projectile.scale * (0.88f + (1f - progress) * 0.12f);
                float rotation = Projectile.oldRot[i] + rotationOffset;

                Main.EntitySpriteDraw(texture, oldPos + Projectile.Size * 0.5f - Main.screenPosition, null, trailColor * alpha, rotation, origin, scale, effects, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, trailColor * 0.18f, Projectile.rotation + rotationOffset, origin, Projectile.scale * 1.05f, effects, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale * 0.55f);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 18f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale * 0.55f);
            Utils.PlotTileLine(start, end, 18f * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;

            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.Center.X > Owner.MountedCenter.X ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<global::Etobudet1modtipo.Buffs.CosmolFire>(), 300);
        }

        private void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;
            Owner.ChangeDir(Projectile.spriteDirection);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = -Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;

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
            Progress = MathHelper.Lerp(WindupRatio * SwingRange, 0f, progress);
            Size = progress;

            if (Timer >= PrepareTime)
            {
                SoundEngine.PlaySound(SoundID.Item1, Owner.Center);
                CurrentStage = AttackStage.Execute;
            }
        }

        private void ExecuteStrike()
        {
            float progress = Timer / ExecuteTime;
            Progress = MathHelper.Lerp(0f, DownStrikeProgress, progress);
            Size = 1f;

            if (Timer >= ExecuteTime)
                CurrentStage = AttackStage.Unwind;
        }

        private void UnwindStrike()
        {
            float unwindProgress = Timer / UnwindTime;
            Progress = MathHelper.Lerp(DownStrikeProgress, FollowThroughProgress, unwindProgress);
            Size = MathHelper.Lerp(1f, 0f, unwindProgress);

            if (Timer >= UnwindTime)
                Projectile.Kill();
        }

        private void TryFireKunaiBurst()
        {
            if (kunaiShot || CurrentStage != AttackStage.Execute || Main.myPlayer != Projectile.owner)
                return;

            if (Timer < ExecuteTime * 0.2f)
                return;

            kunaiShot = true;

            Vector2 baseVelocity = Owner.DirectionTo(Main.MouseWorld);
            if (baseVelocity == Vector2.Zero)
                baseVelocity = Projectile.rotation.ToRotationVector2();

            Vector2 spawnPosition = Owner.MountedCenter;
            float maxSpread = MathHelper.ToRadians(10f);
            int projectileCount = Main.rand.Next(2, 4);
            float kunaiSpeed = Math.Max(10f, Owner.HeldItem.shootSpeed) * 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 velocity = baseVelocity.RotatedByRandom(maxSpread) * kunaiSpeed;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    velocity,
                    ModContent.ProjectileType<CosmicKunai>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }

        private void EmitSwingEffects()
        {
            Lighting.AddLight(Projectile.Center, 0.08f, 0.3f, 0.52f);

            int dustChance = CurrentStage == AttackStage.Execute ? 1 : 4;
            if (!Main.rand.NextBool(dustChance))
                return;

            Vector2 dustPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(18f, 68f) * Projectile.scale;
            Vector2 dustVelocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.45f) * Main.rand.NextFloat(0.4f, 2.2f);
            int dustType = Main.rand.NextBool() ? DustID.FireworkFountain_Blue : DustID.PurpleTorch;
            float dustScale = CurrentStage == AttackStage.Execute ? Main.rand.NextFloat(1.15f, 1.6f) : Main.rand.NextFloat(0.85f, 1.3f);
            Dust dust = Dust.NewDustPerfect(dustPosition, dustType, dustVelocity, 110, default, dustScale);
            dust.noGravity = true;
        }
    }
}
