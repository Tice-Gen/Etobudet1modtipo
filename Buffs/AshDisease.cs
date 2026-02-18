using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.Buffs
{
    public class AshDisease : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {

            LossOfConsciousness.Enabled = true;


            player.breath -= 4;

            if (player.breath <= 0)
            {
                player.breath = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 30;

                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(
                        player.position,
                        player.width,
                        player.height,
                        DustID.Smoke,
                        0f,
                        -2f,
                        100,
                        default,
                        1.5f
                    );
                }
            }
        }
    }
}
