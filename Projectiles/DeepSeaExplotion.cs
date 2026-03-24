using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaExplotion : ModProjectile
    {
        private const int TotalFrames = 5;
        private const int FrameWidth = 177;
        private const int FrameHeight = 210;
        private const int LifetimeTicks = 15;
        private bool secondFrameDamageApplied;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = TotalFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = FrameWidth;
            Projectile.height = FrameHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = LifetimeTicks;
            Projectile.DamageType = DamageClass.Default;
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            float glowStrength = Projectile.timeLeft / (float)LifetimeTicks;
            Lighting.AddLight(Projectile.Center, 0.9f * glowStrength, 1.45f * glowStrength, 1.8f * glowStrength);
            AddBlockPiercingGlow(glowStrength);

            int age = LifetimeTicks - Projectile.timeLeft;
            if (age < 0)
            {
                age = 0;
            }

            float unit = LifetimeTicks / 21f; // 1 + 5 + 5 + 5 + 5
            float firstTransition = unit;
            float secondTransition = unit * 6f;
            float thirdTransition = unit * 11f;
            float fourthTransition = unit * 16f;

            if (age < firstTransition)
            {
                Projectile.frame = 0;
            }
            else if (age < secondTransition)
            {
                Projectile.frame = 1;
            }
            else if (age < thirdTransition)
            {
                Projectile.frame = 2;
            }
            else if (age < fourthTransition)
            {
                Projectile.frame = 3;
            }
            else
            {
                Projectile.frame = 4;
            }

            TryDealDamageOnSecondFrame();
        }

        private void AddBlockPiercingGlow(float glowStrength)
        {
            const int ringLights = 12;
            float radius = 96f * glowStrength;
            float r = 0.65f * glowStrength;
            float g = 1.05f * glowStrength;
            float b = 1.3f * glowStrength;

            for (int i = 0; i < ringLights; i++)
            {
                float t = MathHelper.TwoPi * i / ringLights;
                Vector2 offset = new Vector2(radius, 0f).RotatedBy(t);
                Lighting.AddLight(Projectile.Center + offset, r, g, b);
            }
        }

        private void TryDealDamageOnSecondFrame()
        {
            if (secondFrameDamageApplied || Projectile.frame != 1)
            {
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            secondFrameDamageApplied = true;
            float radius = Projectile.ai[0] > 0f ? Projectile.ai[0] : 70f;
            int damage = Projectile.damage;
            if (damage <= 0)
            {
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.life <= 0)
                {
                    continue;
                }

                if (npc.Distance(Projectile.Center) > radius)
                {
                    continue;
                }

                int hitDir = npc.Center.X >= Projectile.Center.X ? 1 : -1;
                ApplyStrikeIgnoringDefenseAndDR(npc, damage, hitDir);
            }
        }

        private static void ApplyStrikeIgnoringDefenseAndDR(NPC npc, int damage, int hitDir)
        {
            int originalDefense = npc.defense;
            float originalTakenDamageMultiplier = npc.takenDamageMultiplier;

            try
            {
                npc.defense = 0;
                npc.takenDamageMultiplier = 1f;
                npc.SimpleStrikeNPC(damage, hitDir, crit: false, knockBack: 0f);
            }
            finally
            {
                npc.defense = originalDefense;
                npc.takenDamageMultiplier = originalTakenDamageMultiplier;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0, Projectile.frame * FrameHeight, FrameWidth, FrameHeight);
            Vector2 origin = new Vector2(FrameWidth * 0.5f, FrameHeight * 0.5f);
            float fade = Projectile.timeLeft / (float)LifetimeTicks;

            Color outerGlow = new Color(70, 200, 255, 255) * (0.85f * fade);
            Color innerGlow = new Color(220, 250, 255, 255) * (1f * fade);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                outerGlow,
                0f,
                origin,
                1.28f,
                SpriteEffects.None,
                0);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                innerGlow,
                0f,
                origin,
                1.12f,
                SpriteEffects.None,
                0);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                Color.White,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
