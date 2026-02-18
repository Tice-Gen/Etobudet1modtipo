using Terraria;
using Terraria.ModLoader;

namespace Etobudet1modtipo
{
    public class Etobudet1modtipo : Mod
    {
        public static int AniseForestMusicSlot = -1;
        public static int MistyCaveMusicSlot = -1;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                string anisePath = "Sounds/Music/AniseForestTheme";
                string mistyPath = "Sounds/Music/MistyCave";

                if (MusicLoader.GetMusicSlot(this, anisePath) == -1)
                    MusicLoader.AddMusic(this, anisePath);

                if (MusicLoader.GetMusicSlot(this, mistyPath) == -1)
                    MusicLoader.AddMusic(this, mistyPath);

                AniseForestMusicSlot = MusicLoader.GetMusicSlot(this, anisePath);
                MistyCaveMusicSlot = MusicLoader.GetMusicSlot(this, mistyPath);
            }
        }

        public override void Unload()
        {
            AniseForestMusicSlot = -1;
            MistyCaveMusicSlot = -1;
        }
    }
}