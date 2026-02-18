using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class ObsidianDemonicScytheSwordProjectile_Awakened : ModProjectile
    {
        private Asset<Texture2D> awakenedProjTexture;
        private bool triedLoadAwakenedProj = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.aiStyle = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        private Asset<Texture2D> GetAwakenedProjTexture()
        {
            if (!triedLoadAwakenedProj)
            {
                triedLoadAwakenedProj = true;
                try
                {
                    awakenedProjTexture = ModContent.Request<Texture2D>(Texture + "", AssetRequestMode.ImmediateLoad);
                    if (awakenedProjTexture == null || awakenedProjTexture.Value == null) awakenedProjTexture = null;
                }
                catch
                {
                    awakenedProjTexture = null;
                }
            }
            return awakenedProjTexture;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            Player player = Main.player[Projectile.owner];
            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1];
            float direction = Projectile.ai[0];
            float velocityRotation = Projectile.velocity.ToRotation();
            float adjustedRotation = MathHelper.Pi * direction * percentageOfLife + velocityRotation + direction * MathHelper.Pi + player.fullRotation;
            Projectile.rotation = adjustedRotation;

            float scaleMulti = 0.6f;
            float scaleAdder = 1f;

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
            Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;


            float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();

            if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
            {
                Color dustColor = Color.Lerp(Color.Purple, Color.MediumPurple, Main.rand.NextFloat());
                Dust coloredDust = Dust.NewDustPerfect(
                    Projectile.Center + dustRotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale),
                    DustID.FireworksRGB,
                    dustVelocity * 1f,
                    100,
                    dustColor,
                    0.4f
                );
                coloredDust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                coloredDust.noGravity = true;
            }

            if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
            {
                Dust.NewDustPerfect(dustPosition, DustID.ShadowbeamStaff, dustVelocity, 100, Color.Purple * Projectile.Opacity, 1.2f * Projectile.Opacity).noGravity = true;
            }

            Projectile.scale *= Projectile.ai[2];

            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.Kill();
            }

            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2)
            {
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
                Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 spawnPos = Main.rand.NextVector2FromRectangle(target.Hitbox);
                Vector2 dir = (spawnPos - target.Center);
                if (dir.LengthSquared() < 0.001f)
                    dir = Main.rand.NextVector2Unit();
                else
                    dir.Normalize();

                Vector2 vel = dir * Main.rand.NextFloat(1.5f, 4.5f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.2f, 1.2f);

                int dustType = Main.rand.NextBool(2) ? DustID.ShadowbeamStaff : DustID.FireworksRGB;
                Dust d = Dust.NewDustPerfect(spawnPos, dustType, vel, 100, Color.Purple, Main.rand.NextFloat(0.6f, 1.4f));
                d.noGravity = true;
                d.fadeIn = 0.4f + Main.rand.NextFloat() * 0.6f;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;
            Asset<Texture2D> awakened = GetAwakenedProjTexture();
            Texture2D texToUse = awakened != null && awakened.Value != null ? awakened.Value : baseTexture;

            int framesVert = Math.Max(1, Main.projFrames[Type]);
            Rectangle sourceRectangle = texToUse.Frame(1, framesVert, 0, Projectile.frame);
            Vector2 origin = new Vector2(sourceRectangle.Width * 0.5f, sourceRectangle.Height * 0.5f);
            Vector2 position = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 1.1f;
            SpriteEffects spriteEffects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);

            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1];
            float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

            Color backDarkColor = new Color(70, 0, 150);
            Color middleMediumColor = new Color(180, 40, 255);
            Color frontLightColor = new Color(230, 180, 255);

            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            Main.EntitySpriteDraw(texToUse, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texToUse, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texToUse, position, sourceRectangle, middleMediumColor * lightingColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texToUse, position, sourceRectangle, frontLightColor * lightingColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);

            Rectangle lastFrameRect = texToUse.Frame(1, framesVert, 0, Math.Min(framesVert - 1, 3));
            Main.EntitySpriteDraw(texToUse, position, lastFrameRect, Color.White * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            Main.EntitySpriteDraw(texToUse, position, lastFrameRect, Color.White * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texToUse, position, lastFrameRect, Color.White * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = Projectile.rotation + Projectile.ai[0] * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texToUse.Width * 0.5f - 6f) * scale;
                DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            Vector2 drawPos2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * ((float)texToUse.Width * 0.5f - 4f) * scale;
            DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

            return false;
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D sparkleTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Color bigColor = shineColor * opacity * 0.5f;
            bigColor.A = 0;
            Vector2 origin = new Vector2(sparkleTexture.Width * 0.5f, sparkleTexture.Height * 0.5f);
            Color smallColor = drawColor * 0.5f;
            float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
            Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
            bigColor *= lerpValue;
            smallColor *= lerpValue;
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
        }
    }
}
