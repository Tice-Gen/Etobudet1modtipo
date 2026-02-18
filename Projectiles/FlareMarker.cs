using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;

namespace Etobudet1modtipo.Projectiles
{
    public class FlareMarker : ModProjectile
    {
        private const float InitialScale = 2f;
        private const int TotalLifetime = 240;
        private const int ShrinkDuration = 120;

        private const float SpawnThreshold = 0.95f;
        private const int EarlyDustTicks = 10;

        private const int TrailLength = 8;

        private const float RotatingRingRadius = 70f;
        private const int RotatingRingDust = 182;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = TotalLifetime;

            Projectile.scale = InitialScale;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.06f;
            Projectile.ai[0]++;

            Lighting.AddLight(
                Projectile.Center,
                0.9f * Projectile.scale,
                0.35f * Projectile.scale,
                0.15f * Projectile.scale
            );

            float t = MathHelper.Clamp(
                Projectile.ai[0] / (float)ShrinkDuration,
                0f,
                1f
            );

            Vector2 center = Projectile.Center;

            Projectile.scale = MathHelper.Lerp(InitialScale, 0f, t);
            Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, t);

            Projectile.Center = center;

            if (Projectile.localAI[1] == 0f)
            {
                SpawnRotatingDustRing182();
            }

            if (
                t >= SpawnThreshold - (EarlyDustTicks / (float)ShrinkDuration)
                && Projectile.localAI[1] == 0f
            )
            {
                SpawnDustCircle235();
                Projectile.localAI[1] = 1f;
            }

            if (t >= SpawnThreshold && Projectile.localAI[0] == 0f)
            {
                SpawnFlares();
                SpawnExplosionDust219();
                Projectile.localAI[0] = 1f;
            }
        }

        private void SpawnRotatingDustRing182()
        {
            Projectile.localAI[2] += 0.08f;

            if (Projectile.ai[0] % 4 != 0)
                return;

            int count = 6;

            for (int i = 0; i < count; i++)
            {
                float angle =
                    Projectile.localAI[2] +
                    MathHelper.TwoPi * i / count;

                Vector2 offset =
                    angle.ToRotationVector2() *
                    RotatingRingRadius;

                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    RotatingRingDust,
                    Vector2.Zero,
                    150,
                    default,
                    1.1f
                );

                d.noGravity = true;
                d.fadeIn = 1.2f;
            }
        }

        private void SpawnDustCircle235()
        {
            int dustType = 235;
            float radius = 100f;
            int count = 36;

            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count;
                Vector2 offset = angle.ToRotationVector2() * radius;

                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    dustType,
                    offset.SafeNormalize(Vector2.UnitY) * 2.5f,
                    150,
                    default,
                    1.3f
                );

                d.noGravity = true;
            }
        }

        private void SpawnExplosionDust219()
        {
            int dustType = 219;
            int count = 40;

            for (int i = 0; i < count; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Unit() *
                    Main.rand.NextFloat(3f, 7f);

                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    dustType,
                    velocity,
                    120,
                    default,
                    1.6f
                );

                d.noGravity = true;
            }
        }




        private void SpawnFlares()
{
    int rocketType = ProjectileID.MiniNukeRocketI;
    Player owner = Main.player[Projectile.owner];


    int waves = 3;
    int rocketsPerWave = 8;
    float horizontalSpread = 520f;
    float spawnHeightMin = 600f;
    float spawnHeightMax = 900f;

    for (int w = 0; w < waves; w++)
    {
        int delay = w * 10;

        for (int i = 0; i < rocketsPerWave; i++)
        {
            Vector2 spawnPos = Projectile.Center;

            spawnPos.X += Main.rand.NextFloat(-horizontalSpread, horizontalSpread);
            spawnPos.Y -= Main.rand.NextFloat(spawnHeightMin, spawnHeightMax);

            Vector2 target =
                Projectile.Center +
                new Vector2(
                    Main.rand.NextFloat(-120f, 120f),
                    Main.rand.NextFloat(-40f, 40f)
                );

            Vector2 velocity = target - spawnPos;

            if (velocity == Vector2.Zero)
                velocity = Vector2.UnitY;

            velocity.Normalize();


            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10f));

            velocity *= Main.rand.NextFloat(13f, 18f);

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                spawnPos,
                velocity,
                rocketType,
                100,
                Projectile.knockBack,
                Projectile.owner
            );

            Projectile p = Main.projectile[proj];
            p.timeLeft = 140;
            p.extraUpdates = 1;
            p.netUpdate = true;
        }
    }

    SoundEngine.PlaySound(SoundID.Item62 with
    {
        Volume = 1.2f,
        Pitch = -0.2f
    }, Projectile.Center);
}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;

            Vector2 origin = texture.Size() * 0.5f;
            SpriteBatch sb = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float fade = 1f - i / (float)Projectile.oldPos.Length;

                Vector2 pos =
                    Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                sb.Draw(
                    texture,
                    pos,
                    null,
                    new Color(255, 200, 120) * fade * 0.4f,
                    Projectile.oldRot[i],
                    origin,
                    Projectile.scale * fade,
                    SpriteEffects.None,
                    0f
                );
            }

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            sb.Draw(
                texture,
                drawPos,
                null,
                Color.White * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            sb.Draw(
                texture,
                drawPos,
                null,
                new Color(255, 180, 80) * 0.6f,
                Projectile.rotation,
                origin,
                Projectile.scale * 1.6f,
                SpriteEffects.None,
                0f
            );

            return false;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
    }
}
