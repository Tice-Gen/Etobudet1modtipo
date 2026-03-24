using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaProj : ModProjectile
    {
        private const int FrameCount = 3;
        private const int FrameTicks = 5;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FrameCount;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 22;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.ArmorPenetration = 999999999;
        }

        public override bool? CanHitNPC(NPC target)
        {
            int fishMobType = ModContent.NPCType<FishMob>();
            int urchinType = ModContent.NPCType<DeepSeaUrchin>();

            if (target.type == fishMobType || target.type == urchinType)
            {
                return false;
            }

            return null;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= FrameTicks)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= FrameCount)
                {
                    Projectile.frame = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, 0.15f, 0.9f, 1.25f);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.FireworkFountain_Blue,
                    -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(0.8f, 0.8f),
                    120,
                    default,
                    Main.rand.NextFloat(0.95f, 1.35f)
                );

                dust.noGravity = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float bonusMultiplier = 1f + target.statDefense * 0.001f;
            modifiers.FinalDamage *= bonusMultiplier;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 28; i++)
            {
                Vector2 burstVelocity = Main.rand.NextVector2Circular(4.6f, 4.6f);
                Dust burstDust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.FireworkFountain_Blue,
                    burstVelocity,
                    80,
                    default,
                    Main.rand.NextFloat(1.05f, 1.45f)
                );

                burstDust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, FrameCount, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }

                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Color trailColor = new Color(70, 220, 255, 0) * (0.55f * progress);
                float scale = Projectile.scale * (0.9f + progress * 0.45f);

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
