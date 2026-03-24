using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Players
{
    public class DeepSeaArmorPlayer : ModPlayer
    {
        private const int DashCooldownTicks = 50;
        private const int DashDurationTicks = 12;
        private const float DashSpeed = 14.5f;

        public bool deepSeaHelmetEquipped;
        public bool deepSeaSetActive;
        private int dashDir;
        private int dashDelay;
        private int dashTimer;

        private enum DashDirection
        {
            Left = -1,
            Right = 1
        }

        public override void ResetEffects()
        {
            deepSeaHelmetEquipped = false;
            deepSeaSetActive = false;
        }

        public override void UpdateDead()
        {
            dashDir = 0;
            dashDelay = 0;
            dashTimer = 0;
        }

        public override void PreUpdateMovement()
        {
            if (!deepSeaSetActive)
            {
                dashDir = 0;
                dashDelay = 0;
                dashTimer = 0;
                return;
            }

            if (dashDelay > 0)
            {
                dashDelay--;
            }

            if (dashTimer > 0)
            {
                Player.velocity.X = dashDir * DashSpeed;
                SpawnDashWaterDust();
                dashTimer--;
                if (dashTimer <= 0)
                {
                    dashDelay = DashCooldownTicks;
                }
                return;
            }

            if (dashDelay > 0 || Player.mount.Active || Player.grappling[0] != -1)
            {
                return;
            }

            if (Etobudet1modtipo.DeepSeaDashKeybind != null
                && Etobudet1modtipo.DeepSeaDashKeybind.JustPressed
                && Player.whoAmI == Main.myPlayer)
            {
                DashDirection direction = Player.direction >= 0
                    ? DashDirection.Right
                    : DashDirection.Left;
                StartDash(direction);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (!deepSeaHelmetEquipped)
            {
                return;
            }

            float pulse = 0.85f + 0.15f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 9f);
            Lighting.AddLight(Player.Center, 1.05f * pulse, 0.58f * pulse, 0.18f * pulse);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (!deepSeaSetActive || dashTimer <= 0)
            {
                return false;
            }

            Player.SetImmuneTimeForAllTypes(Player.longInvince ? 80 : 40);
            return true;
        }

        private void StartDash(DashDirection direction)
        {
            dashDir = (int)direction;
            dashTimer = DashDurationTicks;
            Player.velocity.X = dashDir * DashSpeed;
            SpawnDashWaterDust();
        }

        private void SpawnDashWaterDust()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = new Vector2(-dashDir * 2.2f, -0.8f) + Main.rand.NextVector2Circular(1.2f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Player.Center, DustID.Water, velocity, 90, default, 1.15f);
                dust.noGravity = true;
            }
        }
    }
}
