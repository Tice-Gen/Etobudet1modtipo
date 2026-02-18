using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class StarArrow : ModProjectile
    {
        private const int StarBurstProjectileType = 729;
        private const float StarBurstSpeed = 10f;


        private const int FlyDustType = 57;


        private const int DeathDustType = 222;

        private const int TrailLen = 24;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLen;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, new Color(255, 220, 120).ToVector3() * 0.55f);


            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    FlyDustType,
                    -Projectile.velocity.X * 0.15f,
                    -Projectile.velocity.Y * 0.15f,
                    120,
                    default,
                    1.15f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.4f;
                Main.dust[dust].velocity += Main.rand.NextVector2Circular(0.6f, 0.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero)
                    continue;

                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                float alpha = progress * progress;

                Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size * 0.5f - Main.screenPosition;
                drawPos.Y += Projectile.gfxOffY;

                Color glowColor = new Color(255, 235, 160, 0) * (0.55f * alpha);
                Main.EntitySpriteDraw(texture, drawPos, null, glowColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);

                Vector2 halo = new Vector2(1f, 0f);
                Color haloColor = new Color(255, 240, 190, 0) * (0.25f * alpha);

                Main.EntitySpriteDraw(texture, drawPos + halo, null, haloColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos - halo, null, haloColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);

                Vector2 haloY = halo.RotatedBy(MathHelper.PiOver2);
                Main.EntitySpriteDraw(texture, drawPos + haloY, null, haloColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos - haloY, null, haloColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.45f)
                target.AddBuff(ModContent.BuffType<StarBurn>(), 10);
        }

        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 24; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DeathDustType,
                    0f,
                    0f,
                    110,
                    default,
                    1.5f
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(4.2f, 4.2f);
            }

            float spawnAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            Vector2 offset = spawnAngle.ToRotationVector2() * 150f;
            Vector2 spawnPosition = Projectile.Center + offset;
            Vector2 directionToDeathPoint = (Projectile.Center - spawnPosition).SafeNormalize(Vector2.UnitX);
            Vector2 velocity = directionToDeathPoint * (StarBurstSpeed * 1.4f);

            Projectile.NewProjectile(
                Projectile.GetSource_Death(),
                spawnPosition,
                velocity,
                StarBurstProjectileType,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner
            );
        }
    }
}
