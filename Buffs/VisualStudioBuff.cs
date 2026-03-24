using Terraria;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Buffs
{
    public class VisualStudioBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.vanityPet[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VisualStudioCode>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    player.GetSource_Buff(buffIndex),
                    player.Center,
                    Microsoft.Xna.Framework.Vector2.Zero,
                    ModContent.ProjectileType<VisualStudioCode>(),
                    0,
                    0f,
                    player.whoAmI);
            }

            player.buffTime[buffIndex] = 18000;
        }
    }
}
