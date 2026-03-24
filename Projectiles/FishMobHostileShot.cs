using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Projectiles
{
    public class FishMobHostileShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bullet;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }

            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active || target.life <= 0 || target.ModNPC is not FishMob fish)
            {
                Projectile.Kill();
                return;
            }

            if (!Projectile.Hitbox.Intersects(target.Hitbox))
            {
                return;
            }

            int damage = Math.Max(1, Projectile.damage);
            int sourceNpc = (int)Projectile.ai[1];
            int hitDirection = target.Center.X >= Projectile.Center.X ? 1 : -1;

            target.SimpleStrikeNPC(damage, hitDirection, crit: false, knockBack: 0.4f);
            fish.RegisterExternalHostileDamage(sourceNpc, damage);
            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }
    }
}
