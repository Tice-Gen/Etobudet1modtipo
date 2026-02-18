using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Buffs
{
    public class Dehydration : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {

            player.GetDamage(DamageClass.Generic) -= 0.10f;


            player.GetDamage(DamageClass.Summon) += 0.35f;
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.35f;


            player.GetAttackSpeed(DamageClass.Generic) -= 0.10f;
            player.GetCritChance(DamageClass.Generic) -= 10f;
            player.GetKnockback(DamageClass.Generic) -= 0.10f;

            player.moveSpeed -= 0.10f;
            player.maxRunSpeed *= 0.90f;
            player.runAcceleration *= 0.90f;
            player.jumpSpeedBoost -= 0.10f;
            player.pickSpeed += 0.10f;
            player.manaCost += 0.10f;

            player.statDefense *= 0.90f;
            player.endurance -= 0.10f;
            player.statManaMax2 = (int)(player.statManaMax2 * 0.90f);
            player.luck *= 0.90f;
        }
    }
}
