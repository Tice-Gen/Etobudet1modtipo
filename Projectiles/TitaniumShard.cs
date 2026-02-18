using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class TitaniumShard : ModProjectile
    {
        private const float OrbitRadius = 50f;
        private const float OverdriveRadiusBonus = 15f;
        private const float OrbitSpeed = 0.09f;
        private const float OrbitSpeedMultiplierOnYoyoHit = 3f;
        private float currentOrbitRadius = OrbitRadius;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[parentIndex];
            if (!parent.active || parent.type != ModContent.ProjectileType<TitaniumYoyoProj>() || parent.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            int slot = (int)Projectile.ai[1];
            float angleOffset = MathHelper.TwoPi * slot / 4f;
            float orbitSpeed = OrbitSpeed;
            float targetOrbitRadius = OrbitRadius;
            if (parent.ModProjectile is TitaniumYoyoProj yoyo && yoyo.ShardOverdriveTimer > 0)
            {
                orbitSpeed *= OrbitSpeedMultiplierOnYoyoHit;
                targetOrbitRadius = OrbitRadius + OverdriveRadiusBonus;
            }

            currentOrbitRadius = MathHelper.Lerp(currentOrbitRadius, targetOrbitRadius, 0.2f);

            float angle = Main.GameUpdateCount * orbitSpeed + angleOffset;
            Vector2 offset = angle.ToRotationVector2() * currentOrbitRadius;

            Projectile.Center = parent.Center + offset;
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.3f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex >= 0 && parentIndex < Main.maxProjectiles)
            {
                Projectile parent = Main.projectile[parentIndex];
                if (parent.active && parent.ModProjectile is TitaniumYoyoProj yoyo && yoyo.ShardOverdriveTimer > 0)
                    modifiers.FinalDamage *= 0.5f;
            }


            modifiers.ScalingArmorPenetration += 0.35f;


            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                float currentMultiplier = target.takenDamageMultiplier;
                float targetMultiplier = 1f - (1f - currentMultiplier) * 0.5f;
                modifiers.FinalDamage *= targetMultiplier / currentMultiplier;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle failSound = new SoundStyle("Etobudet1modtipo/Sounds/ShardHit_fail")
            {
                Volume = 0.85f,
                Pitch = Main.rand.NextFloat(-0.04f, 0.04f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(failSound, Projectile.Center);

            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles)
                return;

            Projectile parent = Main.projectile[parentIndex];
            if (parent.active && parent.ModProjectile is TitaniumYoyoProj yoyo)
                yoyo.ShardOverdriveTimer = 30;
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
                Color color = lightColor * (0.35f * progress);
                float scale = Projectile.scale * (0.85f + 0.15f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
