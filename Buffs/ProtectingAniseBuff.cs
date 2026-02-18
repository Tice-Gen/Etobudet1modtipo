using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Buffs
{
    public class ProtectingAniseBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ProtectingAnise>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
                player.DelBuff(buffIndex);
        }
    }
}
