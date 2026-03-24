using Etobudet1modtipo.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class ColdExposure : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.Chilled}";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float strength = MathHelper.Clamp(player.GetModPlayer<TemperaturePlayer>().ColdPenaltyStrength, 0f, 1.8f);
            if (strength <= 0f)
            {
                return;
            }

            player.GetAttackSpeed(DamageClass.Generic) -= 0.04f + 0.04f * strength;
            player.moveSpeed -= 0.05f + 0.05f * strength;
            player.maxRunSpeed -= 0.2f + 0.35f * strength;
            player.jumpSpeedBoost -= 0.4f + 0.3f * strength;
            player.pickSpeed += 0.03f + 0.04f * strength;
            player.statDefense -= 2 + (int)System.Math.Round(4f * strength);
        }
    }
}
