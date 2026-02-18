using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.Events
{
    public class CatastropheEventSystem : ModSystem
    {
        public static bool catastropheNight;

        public override void OnWorldLoad()
        {
            catastropheNight = false;
        }

        public override void OnWorldUnload()
        {
            catastropheNight = false;
        }

        public override void PreUpdateInvasions()
        {
            if (catastropheNight)
            {
                if (Main.dayTime)
                {
                    catastropheNight = false;
                }
            }
        }
    }


    public class EyeOfCalamityKillHook : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.type == ModContent.NPCType<NPCs.EyeOfCalamity>())
            {
                if (!Main.dayTime)
                {
                    CatastropheEventSystem.catastropheNight = true;

                    if (Main.netMode != NetmodeID.Server)
                        Main.NewText("Бедствие охватывает эту ночь...", 200, 0, 200);

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.position);
                }
            }
        }
    }


    public class CatastropheEventSpawns : GlobalNPC
    {

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (CatastropheEventSystem.catastropheNight)
            {
                pool.Clear();


                pool[NPCID.BloodZombie] = 1f;
                pool[NPCID.Drippler] = 1f;
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (CatastropheEventSystem.catastropheNight)
            {
                spawnRate = (int)(spawnRate * 1f);
                maxSpawns = (int)(maxSpawns * 4f);
            }
        }
    }
}