using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;

namespace Etobudet1modtipo.Projectiles
{
    public class CalamityEye : ModProjectile
    {
        private const int TotalFrames = 7;

        private const int ShootEveryTicks = 30;
        private const int FadeInTicks = 30;
        private const int FadeOutTicks = 30;
        private const int LifeTimeTicks = 240;

        private const int StarDamage = 100;
        private const float StarSpeed = 12f;

        private const int FrameTicks = 6;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = TotalFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 384;
            Projectile.height = 196;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = LifeTimeTicks;

            Projectile.damage = 0;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage() => false;

        public override void DrawBehind(int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            bool dyingSoon = Projectile.timeLeft <= FadeOutTicks;

            if (dyingSoon)
                AnimateReverse();
            else
                AnimateForwardWithLoopLast3();

            FadeInOut(dyingSoon);
            ApplyUnderSupervisionDebuff();


            if (Projectile.ai[1] != 1f)
                ShootStars();
        }

        private void AnimateForwardWithLoopLast3()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter < FrameTicks)
                return;

            Projectile.frameCounter = 0;


            if (Projectile.frame < 4)
            {
                Projectile.frame++;
                if (Projectile.frame > 4)
                    Projectile.frame = 4;
            }
            else
            {
                Projectile.frame++;
                if (Projectile.frame > 6)
                    Projectile.frame = 4;
            }
        }

        private void AnimateReverse()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter < FrameTicks)
                return;

            Projectile.frameCounter = 0;

            if (Projectile.frame > 0)
                Projectile.frame--;
            else
                Projectile.frame = 0;
        }

        private void FadeInOut(bool dyingSoon)
        {

            if (Projectile.ai[0] == 0f)
            {
                int step = 255 / FadeInTicks;
                if (step < 1) step = 1;

                Projectile.alpha -= step;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[0] = 1f;
                    Projectile.frame = 6;
                }
                return;
            }


            if (dyingSoon)
            {
                int step = 255 / FadeOutTicks;
                if (step < 1) step = 1;

                Projectile.alpha += step;

                if (Projectile.alpha >= 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
        }

        private void ShootStars()
        {
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] < ShootEveryTicks)
                return;

            Projectile.localAI[0] = 0f;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int target = Player.FindClosest(Projectile.Center, 1, 1);
            if (target < 0 || target >= Main.maxPlayers)
                return;

            Player player = Main.player[target];
            if (!player.active || player.dead)
                return;

            Vector2 dir = player.Center - Projectile.Center;
            if (dir.LengthSquared() < 0.001f)
                dir = Vector2.UnitY;

            dir.Normalize();
            Vector2 velocity = dir * StarSpeed;


            Vector2 spawnPos = Projectile.Center;

            Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                spawnPos,
                velocity,
                ModContent.ProjectileType<SeraphimCalamityStarV3>(),
                StarDamage,
                0f,
                Main.myPlayer
            );
        }

        private void ApplyUnderSupervisionDebuff()
        {
            int target = Player.FindClosest(Projectile.Center, 1, 1);
            if (target < 0 || target >= Main.maxPlayers)
                return;

            Player player = Main.player[target];
            if (!player.active || player.dead)
                return;

            player.AddBuff(ModContent.BuffType<UnderSupervision>(), 2);
        }
    }
}
