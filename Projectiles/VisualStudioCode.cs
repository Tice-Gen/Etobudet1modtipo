using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class VisualStudioCode : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabySkeletronHead);
            AIType = ProjectileID.BabySkeletronHead;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<VisualStudioBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<VisualStudioBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }
    }
}
