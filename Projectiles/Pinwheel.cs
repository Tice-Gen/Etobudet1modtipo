
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class Pinwheel : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            Main.projFrames[Projectile.type] = 5;


            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 19;
            Projectile.height = 19;

            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = 0;


            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {

            Projectile.frame = Main.rand.Next(0, 5);
        }

        public override void AI()
        {

            Projectile.velocity.Y += 0.1f;


            float windInfluence = 0.1f;
            Projectile.velocity.X += Main.windSpeedCurrent * windInfluence;


            Projectile.rotation += 0.4f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = frame.Size() / 2f;


            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color color = lightColor * progress * 0.6f;

                Vector2 drawPos =
                    Projectile.oldPos[i]
                    + Projectile.Size / 2f
                    - Main.screenPosition;

                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    frame,
                    color,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0f
                );
            }


            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int spawnCount = 12;

            for (int i = 0; i < spawnCount; i++)
            {
                int pick = Main.rand.Next(4);
                int dustType = pick switch
                {
                    0 => DustID.GemRuby,
                    1 => DustID.GemSapphire,
                    2 => DustID.GemEmerald,
                    _ => DustID.GemAmethyst,
                };

                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.5f, 4f);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    dustType,
                    vel,
                    150,
                    default,
                    1.2f
                );
                d.noGravity = false;
            }

            SoundStyle style = new SoundStyle("Etobudet1modtipo/Sounds/Spring")
{
    Pitch = Main.rand.NextFloat(-0f, 0.15f),
    PitchVariance = 0f
};

SoundEngine.PlaySound(style, Projectile.position);

        }
    }
}
