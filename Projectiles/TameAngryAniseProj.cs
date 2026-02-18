using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Etobudet1modtipo.Buffs;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class TameAngryAniseProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 1;


            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999999;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<TameAngryAniseBuff>());
                return;
            }

            if (player.HasBuff(ModContent.BuffType<TameAngryAniseBuff>()))
                Projectile.timeLeft = 2;

            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 60f;
            idlePosition.X += player.direction * 60f;

            Vector2 toIdle = idlePosition - Projectile.Center;
            float distanceToIdle = toIdle.Length();

            if (distanceToIdle > 1000f)
            {
                Projectile.Center = idlePosition;
            }
            else if (distanceToIdle > 20f)
            {
                toIdle.Normalize();
                toIdle *= 8f;
                Projectile.velocity = (Projectile.velocity * 20f + toIdle) / 21f;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

            Projectile.rotation += 0.05f;

            Lighting.AddLight(Projectile.Center, 0.3f, 0.9f, 0.3f);

            if (Main.rand.NextFloat() < 0.05f)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GreenFairy
                );
                Main.dust[dust].velocity *= 0.3f;
                Main.dust[dust].noGravity = true;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float fade = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color color = lightColor * fade * 0.35f;

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
    }
}
