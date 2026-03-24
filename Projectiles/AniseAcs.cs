using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class AniseAcs : ModProjectile
    {
        private const int BaseSize = 22;
        private const float EmpoweredOrbitRadiusMultiplier = 1.1f;
        private const float BaseOrbitRotationSpeed = MathHelper.TwoPi / (60f * 5f);
        private const float EmpoweredRotationSpeedMultiplier = 5f;
        private const float BaseSpriteSpinSpeed = 0.15f;
        private const int EmpoweredDurationTicks = 120;
        private const float BoostBlendSpeed = 0.12f;

        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = BaseSize;
            Projectile.height = BaseSize;
            Projectile.scale = 1f;

            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 2;

            Projectile.DamageType = DamageClass.Generic;


            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.active && !player.dead)
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
                return;
            }


            float radius = Projectile.ai[1];
            float index = Projectile.ai[0];
            float totalCount = 8f;
            float baseAngle = MathHelper.TwoPi * (index / totalCount);
            if (Projectile.localAI[2] == 0f)
            {
                Projectile.localAI[0] = baseAngle;
                Projectile.localAI[2] = 1f;
            }

            if (Projectile.ai[2] > 0f)
            {
                Projectile.ai[2]--;
            }

            float targetBoost = Projectile.ai[2] > 0f ? 1f : 0f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], targetBoost, BoostBlendSpeed);
            if (Math.Abs(Projectile.localAI[1] - targetBoost) < 0.01f)
            {
                Projectile.localAI[1] = targetBoost;
            }

            float smoothBoost = MathHelper.SmoothStep(0f, 1f, Projectile.localAI[1]);
            float radiusMultiplier = MathHelper.Lerp(1f, EmpoweredOrbitRadiusMultiplier, smoothBoost);
            float speedMultiplier = MathHelper.Lerp(1f, EmpoweredRotationSpeedMultiplier, smoothBoost);

            radius *= radiusMultiplier;
            Projectile.localAI[0] += BaseOrbitRotationSpeed * speedMultiplier;
            float currentAngle = Projectile.localAI[0];


            Projectile.Center = player.Center + radius * new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle));


            Projectile.rotation += BaseSpriteSpinSpeed * speedMultiplier;


            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EmpowerAllOwnedProjectiles();
        }

        private void EmpowerAllOwnedProjectiles()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (!other.active || other.owner != Projectile.owner || other.type != Projectile.type)
                {
                    continue;
                }

                other.ai[2] = EmpoweredDurationTicks;
                other.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;


            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                
                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                

                Color color = Projectile.GetAlpha(new Color(150, 255, 150, 0)) * progress * 0.6f;
                
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * progress, SpriteEffects.None, 0);
            }


            return true;
        }
    }
}
