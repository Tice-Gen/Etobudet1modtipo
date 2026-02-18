using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Particles
{
    public class HeroParticle : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;

            dust.scale = 1f;
            dust.velocity *= 0.5f;

            dust.frame = new Rectangle(0, 0, 9, 9);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.rotation += dust.velocity.X * 0.15f;

            dust.scale *= 0.98f;

            float light = dust.scale * 0.8f;
            Lighting.AddLight(dust.position, light, light, light);

            if (dust.scale < 0.3f)
                dust.active = false;

            return false;
        }
    }
}
