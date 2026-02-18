using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class AngryAniseProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 33;
            Projectile.height = 33;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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
                

                Color color = Projectile.GetAlpha(new Color(50, 255, 50, 0)) * progress * 0.6f;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * (0.5f + progress * 0.5f), SpriteEffects.None, 0);
            }
            return true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.5f;

            if (Main.rand.NextFloat() < 0.03f)
            {
                int gasType = ModContent.ProjectileType<AngryGas>();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, gasType, Projectile.damage / 2, 0f, Main.myPlayer);
            }

            if (Main.rand.NextFloat() < 0.2f)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}