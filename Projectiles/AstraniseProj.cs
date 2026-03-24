using Etobudet1modtipo.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class AstraniseProj : ModProjectile
    {
        private const float DistanceBeforeSlowdown = 160f;
        private const float SlowdownFactor = 0.90f;
        private const float StopSpeed = 0.45f;
        private const int WaitBeforeBurst = 60;
        private const int ShardCount = 6;
        private const float ExplosionRadius = 96f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 2f ? false : null;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            EnterStopState();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EnterStopState();
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 1f)
            {
                Projectile.localAI[0] += Projectile.velocity.Length();
                if (Projectile.localAI[0] >= DistanceBeforeSlowdown)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[0] == 1f)
            {
                Projectile.velocity *= SlowdownFactor;
                if (Projectile.velocity.Length() <= StopSpeed)
                {
                    EnterStopState();
                }
            }
            else if (Projectile.ai[0] >= 2f)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= WaitBeforeBurst)
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.velocity.LengthSquared() > 0.001f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation += 0.18f;
            }

            Lighting.AddLight(Projectile.Center, 0.95f, 0.45f, 0.1f);
        }

        private void EnterStopState()
        {
            if (Projectile.ai[0] >= 2f)
            {
                return;
            }

            Projectile.ai[0] = 2f;
            Projectile.localAI[1] = 0f;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.netUpdate = true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 22; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(3.6f, 3.6f);
                Dust torchDust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, dustVelocity, 80, default, Main.rand.NextFloat(1.1f, 1.6f));
                torchDust.noGravity = true;

                Vector2 smokeVelocity = Main.rand.NextVector2Circular(2f, 2f);
                Dust smokeDust = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, smokeVelocity, 120, default, Main.rand.NextFloat(0.9f, 1.25f));
                smokeDust.noGravity = Main.rand.NextBool(3);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int explosionDamage = (int)(Projectile.damage * 0.85f);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                {
                    continue;
                }

                if (npc.Distance(Projectile.Center) <= ExplosionRadius)
                {
                    npc.SimpleStrikeNPC(explosionDamage, Projectile.direction, false, Projectile.knockBack);
                }
            }

            int shardDamage = (int)(Projectile.damage * 0.6f);
            for (int i = 0; i < ShardCount; i++)
            {
                float angle = MathHelper.TwoPi * i / ShardCount;
                Vector2 shardVelocity = angle.ToRotationVector2() * Main.rand.NextFloat(6f, 9f);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    shardVelocity,
                    ModContent.ProjectileType<AstraniseShard>(),
                    shardDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldDrawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(255, 130, 55, 0) * (0.35f * progress);
                float trailScale = Projectile.scale * (0.86f + 0.18f * progress);

                Main.EntitySpriteDraw(texture, oldDrawPos, frame, trailColor, Projectile.oldRot[i], origin, trailScale, SpriteEffects.None, 0);
            }

            float pulse = 0.85f + 0.15f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 8f);
            Color glowColor = new Color(255, 160, 70, 0);
            for (int i = 0; i < 4; i++)
            {
                float angle = MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 1.7f;
                Vector2 offset = angle.ToRotationVector2() * (2.2f * pulse);
                Main.EntitySpriteDraw(texture, drawPos + offset, frame, glowColor * 0.35f, Projectile.rotation, origin, Projectile.scale * (1.04f + 0.04f * pulse), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
