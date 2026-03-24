using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class GoldenShard : BaseMetalLineShard
    {
        protected override int ParentProjectileType => ModContent.ProjectileType<GoldenYoyo>();
        protected override int TotalShardCount => 2;
        protected override float OrbitRadius => 44f;
        protected override float OrbitSpeed => 0.36f;
        protected override string HitSoundPath => "Etobudet1modtipo/Sounds/ShardHit_PlatinumOrGold";
        protected override int HitDustType => DustID.GoldCoin;
        protected override Color LightColor => new Color(255, 210, 80);
        protected override bool UseDepthVisuals => false;
        protected override bool UseInstantOrbitFollow => true;
    }
}
