using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaHammerProj : ModProjectile
    {
        private const int ReturnState = 2;
        private const int OvershootState = 1;
        private const int OvershootTicks = 8;
        private const int MaxRehits = 9;
        private const float ReturnSpeed = 17f;
        private const float ReturnInertia = 18f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = 0;
            Projectile.scale = 1f;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;
            Projectile.spriteDirection = Projectile.velocity.X < 0f ? -1 : 1;

            if (Projectile.ai[0] == OvershootState)
            {
                Projectile.localAI[0]++;
                Projectile.velocity *= 0.985f;

                if (Projectile.localAI[0] >= OvershootTicks)
                {
                    Projectile.ai[0] = ReturnState;
                    Projectile.localAI[0] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[0] == ReturnState)
            {
                int targetIndex = (int)Projectile.ai[1];
                if (targetIndex >= 0 && targetIndex < Main.maxNPCs && Main.npc[targetIndex].CanBeChasedBy(this))
                {
                    NPC target = Main.npc[targetIndex];
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * ReturnSpeed;
                    Projectile.velocity = (Projectile.velocity * (ReturnInertia - 1f) + desiredVelocity) / ReturnInertia;
                    SpawnHomingRingDust();
                }
                else
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = -1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;

                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
                Color color = new Color(40, 180, 255, 0) * progress * 0.55f;

                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = target.whoAmI;
            Projectile.ai[0] = OvershootState;
            Projectile.localAI[0] = 0f;
            Projectile.localAI[1]++;
            Projectile.netUpdate = true;

            if (Projectile.localAI[1] >= MaxRehits)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                float angle = MathHelper.TwoPi * i / 24f;
                Vector2 velocity = angle.ToRotationVector2() * 4.2f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 161, velocity, 60, default, 1.35f);
                dust.noGravity = true;
            }
        }

        private void SpawnHomingRingDust()
        {
            float baseAngle = Main.GlobalTimeWrappedHourly * 8f + Projectile.whoAmI * 0.17f;
            for (int i = 0; i < 3; i++)
            {
                float angle = baseAngle + MathHelper.TwoPi * i / 3f;
                Vector2 offset = angle.ToRotationVector2() * 18f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 160, -offset.SafeNormalize(Vector2.UnitY) * 0.45f, 80, default, 1.1f);
                dust.noGravity = true;
            }
        }
    }
}
