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
    public class ToxicSmoke2 : ModProjectile
    {
        public override string Texture => "Etobudet1modtipo/Projectiles/ToxicSmoke2";

        private const int DEFAULT_TIME = 600;
        private const int FADE_TIME = 160;
        private const int INITIAL_ALPHA = 90;
        private const float SLOWDOWN = 0.99f;
        private const float UP_DRIFT = 0.02f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = INITIAL_ALPHA;
            Projectile.knockBack = -1f;
            Projectile.damage = 0;
            Projectile.noEnchantmentVisuals = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero)
                    continue;

                Vector2 drawPos =
                    Projectile.oldPos[k]
                    - Main.screenPosition
                    + Projectile.Size / 2f
                    + new Vector2(0f, Projectile.gfxOffY);

                float progress = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                float scale = Projectile.scale * (1f + k * 0.12f);


                Color baseColor = new Color(30, 20, 25, 0) * progress * 0.25f;
                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    Projectile.GetAlpha(baseColor),
                    Projectile.oldRot[k],
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );


                Color innerGlow = new Color(120, 25, 25) * (0.14f * progress);
                Color outerGlow = new Color(160, 40, 40) * (0.06f * progress);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Additive,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    null,
                    Main.GameViewMatrix.TransformationMatrix
                );


                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    innerGlow,
                    Projectile.oldRot[k],
                    origin,
                    scale * 1.02f,
                    SpriteEffects.None,
                    0
                );


                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    outerGlow,
                    Projectile.oldRot[k],
                    origin,
                    scale * 1.15f,
                    SpriteEffects.None,
                    0
                );

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    null,
                    Main.GameViewMatrix.TransformationMatrix
                );
            }

            return true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.localAI[1] = Main.rand.NextFloat(0.01f, 0.03f) *
                                        (Main.rand.NextBool() ? 1f : -1f);
            }

            Projectile.rotation += Projectile.localAI[1];
            Projectile.velocity *= SLOWDOWN;
            Projectile.velocity.Y -= UP_DRIFT;


            Lighting.AddLight(Projectile.Center, 0.08f, 0.01f, 0.01f);

            if (Main.rand.NextBool(5))
            {
                int dustType = Main.rand.NextBool(3) ? DustID.Torch : DustID.Ash;
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0, 0, 200, default, 1.2f);
                Main.dust[d].noGravity = true;
            }

            if (Projectile.timeLeft <= FADE_TIME)
            {
                Projectile.alpha += 1;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity = Vector2.Zero;
            target.AddBuff(ModContent.BuffType<AshDisease>(), 600);
            info.Damage = 0;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                int smoke = Dust.NewDust(
                    Projectile.Center,
                    0,
                    0,
                    DustID.Smoke,
                    velocity.X,
                    velocity.Y,
                    200,
                    Color.Black,
                    Main.rand.NextFloat(1f, 2f)
                );
                Main.dust[smoke].noGravity = true;
            }
        }
    }
}
