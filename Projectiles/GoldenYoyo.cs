using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class GoldenYoyo : BaseMetalLineYoyo
    {
        protected override int ShardProjectileType => ModContent.ProjectileType<GoldenShard>();
        protected override int ShardCount => 2;
        protected override float YoyoLifeTimeMultiplier => 7f;
        protected override float YoyoMaximumRange => 230f;
        protected override float YoyoTopSpeed => 14f;
        protected override string HitSoundPath => "Etobudet1modtipo/Sounds/MetalHit_Gold";
        protected override int HitDustType => DustID.GoldCoin;
        protected override Color LightColor => new Color(255, 208, 90);
    }
}
