using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class PlatinumLine : BaseMetalLine
    {
        protected override int BarItemType => ItemID.PlatinumBar;
        protected override int YoyoProjectileType => ModContent.ProjectileType<PlatinumYoyo>();
        protected override int BaseDamage => 16;
        protected override int ItemRarity => ItemRarityID.Green;
        protected override int ItemValue => Terraria.Item.buyPrice(gold: 1);
        protected override float ShootSpeed => 14f;
        protected override float KnockBack => 3f;
        protected override string OrbitalDescription => "Three shards orbit the yoyo.";
        protected override Color OrbitalTagStartColor => new Color(115, 155, 205);
        protected override Color OrbitalTagEndColor => new Color(235, 245, 255);
    }
}
