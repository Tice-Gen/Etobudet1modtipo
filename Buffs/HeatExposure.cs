using Etobudet1modtipo.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class HeatExposure : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.OnFire}";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float strength = MathHelper.Clamp(player.GetModPlayer<TemperaturePlayer>().HeatPenaltyStrength, 0f, 1.8f);
            if (strength <= 0f)
            {
                return;
            }

            player.GetDamage(DamageClass.Generic) -= 0.04f + 0.04f * strength;
            player.GetCritChance(DamageClass.Generic) -= 2f + 3f * strength;
            player.moveSpeed -= 0.02f + 0.05f * strength;
            player.endurance -= 0.015f + 0.02f * strength;
            player.manaCost += 0.04f + 0.05f * strength;
            player.statDefense -= 1 + (int)System.Math.Round(3f * strength);
        }
    }
}
