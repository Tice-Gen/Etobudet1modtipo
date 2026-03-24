using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Players
{
    public class DeepSeaYoyoPlayer : ModPlayer
    {
        public int DeepSeaChargeHits;

        public bool IsDeepSeaReady => DeepSeaChargeHits >= 9;

        public override void PostUpdate()
        {
            if (Main.dedServ || DeepSeaChargeHits <= 0)
            {
                return;
            }

            if (Player.HeldItem.type != ModContent.ItemType<DeepSeaYoyo>())
            {
                return;
            }

            int dustCount = DeepSeaChargeHits;
            float radius = 28f + 8f * DeepSeaChargeHits;
            float spin = Main.GameUpdateCount * 0.06f;

            for (int i = 0; i < dustCount; i++)
            {
                float angle = spin + MathHelper.TwoPi * i / dustCount;
                Vector2 pos = Player.Center + angle.ToRotationVector2() * radius;
                Vector2 vel = (Player.Center - pos).SafeNormalize(Vector2.Zero) * 0.25f;
                Dust d = Dust.NewDustPerfect(pos, DustID.DungeonWater, vel, 120, default, 1.1f);
                d.noGravity = true;
            }
        }
    }
}
