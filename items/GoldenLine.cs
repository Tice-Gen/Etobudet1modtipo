using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class GoldenLine : BaseMetalLine
    {
        protected override int BarItemType => ItemID.GoldBar;
        protected override int YoyoProjectileType => ModContent.ProjectileType<GoldenYoyo>();
        protected override int BaseDamage => 12;
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override int ItemValue => Terraria.Item.buyPrice(silver: 80);
        protected override float ShootSpeed => 13.5f;
        protected override float KnockBack => 2.75f;
        protected override string OrbitalDescription => "Two fast shards orbit the yoyo.";
        protected override Color OrbitalTagStartColor => new Color(145, 92, 10);
        protected override Color OrbitalTagEndColor => new Color(255, 220, 80);
    }
}
