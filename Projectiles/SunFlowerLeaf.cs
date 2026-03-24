using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class SunFlowerLeaf : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Кол-во кадров
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 26;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 300;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {

            Projectile.aiStyle = 44;
            AIType = ProjectileID.Leaf;

            // 🎞️ Анимация
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6) 
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= 5)
                    Projectile.frame = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
{
    int damage = (int)(target.lifeMax * 0.01f);

    if (damage < 15)
        damage = 15;

    modifiers.SourceDamage.Base = damage;
}
    }
}