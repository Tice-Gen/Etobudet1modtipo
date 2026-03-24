using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class PlatinumShard : BaseMetalLineShard
    {
        protected override int ParentProjectileType => ModContent.ProjectileType<PlatinumYoyo>();
        protected override int TotalShardCount => 3;
        protected override float OrbitRadius => 48f;
        protected override float OrbitSpeed => 0.14f;
        protected override string HitSoundPath => "Etobudet1modtipo/Sounds/ShardHit_PlatinumOrGold";
        protected override int HitDustType => DustID.GemDiamond;
        protected override Color LightColor => new Color(225, 240, 255);
    }
}
