using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Gores
{

    public class DeadAniseKing1 : ModGore
    {
        public override bool Update(Gore gore)
        {

            gore.velocity.Y += 0.18f;
            gore.velocity.X *= 0.995f;
            gore.rotation += gore.velocity.X * 0.04f;
            gore.scale *= 0.999f;


            if (gore.position.Y > Main.maxTilesY * 16 + 2000f)
                gore.active = false;

            return true;
        }
    }


    public class DeadAniseKing2 : ModGore
    {
        public override bool Update(Gore gore)
        {
            gore.velocity.Y += 0.16f;
            gore.velocity.X *= 0.997f;
            gore.rotation += gore.velocity.X * 0.02f;

            if (gore.position.Y > Main.maxTilesY * 16 + 2000f)
                gore.active = false;

            return true;
        }
    }
}
