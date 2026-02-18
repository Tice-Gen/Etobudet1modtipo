using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Players
{
    public class ShroomyoPlayer : ModPlayer
    {
        public const int EmpowerCooldownTicks = 15 * 60;
        public const float EmpowerDamageMultiplier = 1.15f;

        public int ShroomyoEmpowerCooldown;
        private int empoweredProjWhoAmI = -1;
        private int empoweredProjIdentity = -1;
        private bool pendingEmpower;

        public override void PostUpdate()
        {
            if (ShroomyoEmpowerCooldown > 0)
                ShroomyoEmpowerCooldown--;

            if (HasEmpoweredYoyoActive())
                return;

            if (empoweredProjWhoAmI != -1)
            {
                ClearEmpowerTracking();
                if (ShroomyoEmpowerCooldown <= 0)
                    ShroomyoEmpowerCooldown = EmpowerCooldownTicks;
            }
        }

        public void ActivateEmpower(Projectile proj)
        {
            empoweredProjWhoAmI = proj.whoAmI;
            empoweredProjIdentity = proj.identity;
            pendingEmpower = false;
        }

        public bool HasEmpoweredYoyoActive()
        {
            if (empoweredProjWhoAmI < 0 || empoweredProjWhoAmI >= Main.maxProjectiles)
                return false;

            Projectile proj = Main.projectile[empoweredProjWhoAmI];
            return proj.active
                && proj.owner == Player.whoAmI
                && proj.type == ModContent.ProjectileType<ShroomyoProj>()
                && proj.identity == empoweredProjIdentity;
        }

        private void ClearEmpowerTracking()
        {
            empoweredProjWhoAmI = -1;
            empoweredProjIdentity = -1;
        }

        public Projectile FindActiveShroomyo()
        {
            int shroomyoProjType = ModContent.ProjectileType<ShroomyoProj>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Player.whoAmI && proj.type == shroomyoProjType)
                    return proj;
            }

            return null;
        }

        public bool IsEmpoweredProjectile(Projectile proj)
        {
            if (proj == null || !proj.active)
                return false;

            return proj.whoAmI == empoweredProjWhoAmI && proj.identity == empoweredProjIdentity;
        }

        public void QueueEmpowerForNextShroomyo()
        {
            pendingEmpower = true;
        }

        public bool ConsumeQueuedEmpower()
        {
            if (!pendingEmpower)
                return false;

            pendingEmpower = false;
            return true;
        }

        public bool HasQueuedEmpower() => pendingEmpower;
    }
}
