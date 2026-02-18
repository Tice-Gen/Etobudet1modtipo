using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class AnisePoisonGas : ModProjectile
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/AnisePoisonGas";

        private const int DEFAULT_TIME = 240;
        private const int FADE_TIME = 60;
        private const int INITIAL_ALPHA = 100;
        private const float SLOWDOWN = 0.985f;
        private const float UP_DRIFT = 0.015f;
        private const float ROT_MIN = 0.06f;
        private const float ROT_MAX = 0.18f;
        private const float START_SCALE = 1f;
        private const float END_SCALE = 0.6f;

        private const int STATIC_IMMUNITY_FRAMES = 15;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = DEFAULT_TIME;
            Projectile.alpha = INITIAL_ALPHA;
            Projectile.scale = START_SCALE;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = STATIC_IMMUNITY_FRAMES;

            Projectile.DamageType = DamageClass.Ranged;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;


                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
                

                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                


                Color color = Projectile.GetAlpha(new Color(100, 255, 100, 0)) * progress * 0.15f;
                

                float trailScale = Projectile.scale * (0.8f + progress * 0.2f);

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, trailScale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.damage = (int)(Projectile.originalDamage * 0.5f);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.localAI[1] = Main.rand.NextFloat(ROT_MIN, ROT_MAX) * (Main.rand.NextBool() ? 1f : -1f);
            }

            Projectile.rotation += Projectile.localAI[1];
            Projectile.velocity *= SLOWDOWN;
            Projectile.velocity.Y -= UP_DRIFT;

            Lighting.AddLight(Projectile.Center, 0.04f, 0.15f, 0.04f);

            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.GreenTorch, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 100, default, 0.8f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].alpha = Projectile.alpha;
            }

            if (Projectile.timeLeft <= FADE_TIME)
            {
                float t = 1f - (Projectile.timeLeft / (float)FADE_TIME);
                Projectile.alpha = (int)MathHelper.Lerp(INITIAL_ALPHA, 255f, t);
                Projectile.scale = MathHelper.Lerp(START_SCALE, END_SCALE, t);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly) return false;

            float radius = 30f * Projectile.scale;
            return Vector2.Distance(Projectile.Center, target.Center) <= radius;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 300);

            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(target.position, target.width, target.height, DustID.GreenTorch);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.3f;
            }
        }
    }
}