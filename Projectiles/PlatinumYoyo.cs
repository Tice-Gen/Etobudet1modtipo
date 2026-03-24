using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class PlatinumYoyo : BaseMetalLineYoyo
    {
        protected override int ShardProjectileType => ModContent.ProjectileType<PlatinumShard>();
        protected override int ShardCount => 3;
        protected override float YoyoLifeTimeMultiplier => 7.5f;
        protected override float YoyoMaximumRange => 255f;
        protected override float YoyoTopSpeed => 15f;
        protected override string HitSoundPath => "Etobudet1modtipo/Sounds/MetalHit_Platinum";
        protected override int HitDustType => DustID.GemDiamond;
        protected override Color LightColor => new Color(220, 235, 255);
    }
}
