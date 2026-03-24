using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepGarp : ModProjectile
    {
        private const float ReturnSpeed = 24f;
        private const float ReturnCatchDistance = 20f;
        private const float ReturnSteerLerp = 0.08f;
        private const float RotationLerp = 0.22f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 28;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;

                Vector2 toOwner = owner.MountedCenter - Projectile.Center;
                float distanceToOwner = toOwner.Length();

                if (distanceToOwner <= ReturnCatchDistance)
                {
                    Projectile.Kill();
                    return;
                }

                Vector2 desiredVelocity = toOwner.SafeNormalize(Vector2.UnitX) * ReturnSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, ReturnSteerLerp);
            }

            float targetRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 1f)
            {
                // Smooth turn only while returning to the player.
                Projectile.rotation = Projectile.rotation.AngleLerp(targetRotation, RotationLerp);
            }
            else
            {
                // On initial shot, rotation snaps to velocity direction.
                Projectile.rotation = targetRotation;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Ignore 100% of target defense.
            modifiers.ScalingArmorPenetration += 1f;

            // Cancel target damage resistance multipliers below 1x.
            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                modifiers.FinalDamage /= target.takenDamageMultiplier;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
            {
                return;
            }

            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Color trailColor = new Color(40, 170, 255, 0) * (0.65f * progress);
                float scale = Projectile.scale * (0.9f + progress * 0.55f);

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    trailColor,
                    Projectile.rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }
    }
}
