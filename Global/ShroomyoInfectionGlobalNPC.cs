using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Global
{
    public class ShroomyoInfectionGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private const int InfectionThreshold = 30;
        private const int InfectionDecayDelay = 300;
        private const int InfectionDecayInterval = 60;
        private const int InfectionTier1Points = 5;
        private const int InfectionTier2Points = 10;
        private const int InfectionTier3Points = 15;
        private const int InfectionTier1LifeRegenPenalty = 10;
        private const int InfectionTier2LifeRegenPenalty = 20;
        private const int InfectionTier3LifeRegenPenalty = 24;
        private int infectionPoints;
        private int ticksSinceLastIncrease;
        private int decayTimer;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            infectionPoints = 0;
            ticksSinceLastIncrease = 0;
            decayTimer = 0;
        }

        public override void AI(NPC npc)
        {
            if (!npc.active || npc.life <= 0 || infectionPoints <= 0)
            {
                ticksSinceLastIncrease = 0;
                decayTimer = 0;
                return;
            }

            ticksSinceLastIncrease++;

            if (ticksSinceLastIncrease <= InfectionDecayDelay)
                return;

            decayTimer++;
            if (decayTimer < InfectionDecayInterval)
                return;

            decayTimer = 0;
            infectionPoints--;
            if (infectionPoints <= 0)
            {
                infectionPoints = 0;
                ticksSinceLastIncrease = 0;
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.active || npc.life <= 0 || infectionPoints <= 0 || npc.hide)
                return;

            DrawInfectionBar(npc, spriteBatch, screenPos);
        }

        public void AddInfection(NPC npc)
        {
            if (!npc.active || npc.life <= 0 || npc.friendly || npc.dontTakeDamage)
                return;

            infectionPoints++;
            ticksSinceLastIncrease = 0;
            decayTimer = 0;

            if (infectionPoints < InfectionThreshold)
                return;

            infectionPoints = 0;
            TriggerThresholdEffect(npc);
        }

        public bool TryConsumeOneInfection()
        {
            if (infectionPoints <= 0)
                return false;

            infectionPoints--;
            ticksSinceLastIncrease = 0;
            decayTimer = 0;
            return true;
        }

        public static int ConsumeOneInfectionFromAllNPCs()
        {
            int spent = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.life <= 0 || npc.friendly)
                    continue;

                ShroomyoInfectionGlobalNPC g = npc.GetGlobalNPC<ShroomyoInfectionGlobalNPC>();
                if (g.TryConsumeOneInfection())
                    spent++;
            }

            return spent;
        }

        private static void DrawInfectionBar(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            ShroomyoInfectionGlobalNPC g = npc.GetGlobalNPC<ShroomyoInfectionGlobalNPC>();
            float progress = MathHelper.Clamp(g.infectionPoints / (float)InfectionThreshold, 0f, 1f);

            int barWidth = 44;
            int barHeight = 5;
            int x = (int)(npc.Center.X - screenPos.X - barWidth * 0.5f);
            int y = (int)(npc.Top.Y - screenPos.Y - 16f);

            Rectangle bgRect = new Rectangle(x, y, barWidth, barHeight);
            Rectangle fillRect = new Rectangle(x + 1, y + 1, (int)((barWidth - 2) * progress), barHeight - 2);

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, bgRect, new Color(20, 20, 20, 200));
            spriteBatch.Draw(pixel, fillRect, new Color(155, 95, 200));
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!npc.active || npc.life <= 0 || infectionPoints <= 0)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            if (infectionPoints >= InfectionTier3Points)
            {
                npc.lifeRegen -= InfectionTier3LifeRegenPenalty;
                if (damage < 1)
                    damage = 1;
            }
            else if (infectionPoints >= InfectionTier2Points)
            {
                npc.lifeRegen -= InfectionTier2LifeRegenPenalty;
                if (damage < 10)
                    damage = 10;
            }
            else if (infectionPoints >= InfectionTier1Points)
            {
                npc.lifeRegen -= InfectionTier1LifeRegenPenalty;
                if (damage < 5)
                    damage = 5;
            }
        }

        private static void TriggerThresholdEffect(NPC npc)
        {
            SpawnBurstDust(npc);

            if (npc.life < 800)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemID.GlowingMushroom, Main.rand.Next(2, 6));
                    Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemID.Mushroom, Main.rand.Next(2, 6));
                }

                npc.life = 0;
                npc.HitEffect();
                npc.checkDead();
                return;
            }

            int hitDir = npc.direction == 0 ? 1 : npc.direction;
            npc.SimpleStrikeNPC(500, hitDir, crit: false, knockBack: 0f);
        }

        private static void SpawnBurstDust(NPC npc)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                Dust d = Dust.NewDustPerfect(npc.Center, 56, vel, 50, default, 1.3f);
                d.noGravity = true;
            }
        }
    }
}
