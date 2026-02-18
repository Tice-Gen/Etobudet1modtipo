using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class AniseForestSpearProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }


            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.1f);
        }

        public override bool PreDraw(ref Color lightColor)
{
    Texture2D texture = TextureAssets.Projectile[Type].Value;
    

    int frameHeight = texture.Height / Main.projFrames[Type];
    Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
    

    Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

    for (int k = 0; k < Projectile.oldPos.Length; k++)
    {
        if (Projectile.oldPos[k] == Vector2.Zero) continue;




        Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
        
        float progress = (float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length;
        Color color = Projectile.GetAlpha(new Color(100, 255, 100, 150)) * progress * 0.5f;
        
        Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);
    }

    return true;
}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 900);

            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 1.2f);

            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 1.2f);

            Projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextFloat() > 0.4f)
                return;

            Vector2 center = Projectile.Center;
            int leafType = ProjectileID.Leaf;
            int spawnedDamage = Projectile.damage / 10;
            float speed = 8f;

            Vector2[] dirs = new Vector2[8]
            {
                new Vector2(0f, -1f), new Vector2(0f, 1f),
                new Vector2(1f, 0f), new Vector2(-1f, 0f),
                new Vector2(1f, -1f), new Vector2(1f, 1f),
                new Vector2(-1f, -1f), new Vector2(-1f, 1f)
            };

            for (int i = 0; i < dirs.Length; i++)
            {
                Vector2 dir = Vector2.Normalize(dirs[i]) * speed;

                Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    center,
                    dir,
                    leafType,
                    spawnedDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            SoundEngine.PlaySound(SoundID.Item14, center);

            for (int i = 0; i < 18; i++)
                Dust.NewDust(center - new Vector2(8, 8), 16, 16, DustID.Grass, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 1.2f);
        }
    }
}