using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaPressure : ModProjectile
    {
        private const int PressureBuffDuration = 300;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 99;
            Projectile.height = 99;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.12f, 0.95f, 1.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                Color trailColor = new Color(70, 220, 255, 0) * (0.45f * progress);
                Vector2 trailScale = new Vector2(
                    Projectile.scale * (0.5f + progress * 0.4f),
                    Projectile.scale * (1.2f + progress * 2.4f)
                );

                Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.rotation, origin, trailScale, SpriteEffects.None, 0);
            }

            Color glyphGlow = new Color(90, 240, 255, 0) * 0.42f;
            for (int i = 0; i < 5; i++)
            {
                float angle = MathHelper.TwoPi * i / 5f + Main.GlobalTimeWrappedHourly * 3f;
                Vector2 offset = angle.ToRotationVector2() * 2.2f;
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition + offset,
                    null,
                    glyphGlow,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.04f,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Pressure>(), PressureBuffDuration);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int particleType = ModContent.ProjectileType<DeepSeaPressureParticle>();
                int particleCount = 8;

                for (int i = 0; i < particleCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / particleCount + Main.rand.NextFloat(-0.2f, 0.2f);
                    Vector2 spawnOffset = angle.ToRotationVector2() * Main.rand.NextFloat(80f, 130f);
                    Vector2 spawnPos = target.Center + spawnOffset;
                    Vector2 velocity = (target.Center - spawnPos).SafeNormalize(Vector2.UnitY) * 2.2f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_OnHit(target),
                        spawnPos,
                        velocity,
                        particleType,
                        0,
                        0f,
                        Projectile.owner,
                        target.whoAmI
                    );
                }
            }
        }
    }
}
