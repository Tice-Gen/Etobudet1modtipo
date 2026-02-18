using Terraria;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using System.Collections.Generic;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Tiles;
using Terraria.Localization;

namespace Etobudet1modtipo.Common.Systems
{
    public class MistyCaveWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (index != -1)
            {
                tasks.Insert(index + 1, new MistyCaveGenPass("Misty Cave Gen", 320f));
            }
        }
    }

    public class MistyCaveGenPass : GenPass
    {
        public MistyCaveGenPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Etobudet1modtipo.CreatingMistyCave", "Creating Misty Cave...");


            int centerX = Main.spawnTileX;
            int centerY = Main.spawnTileY + 80;


            int minY = (int)Main.worldSurface + 50;
            int maxY = Main.maxTilesY - 300;
            if (centerY < minY) centerY = minY;
            if (centerY > maxY) centerY = maxY;
            if (centerX < 200) centerX = 200;
            if (centerX > Main.maxTilesX - 200) centerX = Main.maxTilesX - 200;

            MistyCaveSystem.MistyCaveCenter = new Vector2(centerX, centerY);

            for (int i = 0; i < 5; i++)
            {
                WorldGen.TileRunner(
                    centerX + WorldGen.genRand.Next(-20, 20),
                    centerY + WorldGen.genRand.Next(-20, 20),
                    WorldGen.genRand.Next(40, 60),
                    WorldGen.genRand.Next(30, 50),
                    -1,
                    false
                );
            }


            int oreCount = 400;
            for (int k = 0; k < oreCount; k++)
            {
                int x = centerX + WorldGen.genRand.Next(-120, 120);
                int y = centerY + WorldGen.genRand.Next(-120, 120);

                if (!WorldGen.InWorld(x, y)) continue;


                if (!Main.tile[x, y].HasTile) continue;

                int oreType = Utils.SelectRandom(WorldGen.genRand, new int[] {
                    TileID.Copper, TileID.Tin,
                    TileID.Iron, TileID.Lead
                });

                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), oreType);
            }


            int fogTile = ModContent.TileType<FogStoneTile>();
            int fogPatches = 30;
            for (int p = 0; p < fogPatches; p++)
            {
                int x = centerX + WorldGen.genRand.Next(-140, 140);
                int y = centerY + WorldGen.genRand.Next(-120, 120);
                if (!WorldGen.InWorld(x, y)) continue;


                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(6, 14), WorldGen.genRand.Next(4, 8), fogTile);
            }


            int fogGemType = ModContent.TileType<FogStone_Gem>();
            int gemAttempts = 150;
            for (int g = 0; g < gemAttempts; g++)
            {
                int x = centerX + WorldGen.genRand.Next(-150, 150);
                int y = centerY + WorldGen.genRand.Next(-130, 130);
                if (!WorldGen.InWorld(x, y)) continue;


                if (!Main.tile[x, y].HasTile) continue;
                int current = Main.tile[x, y].TileType;

                if (current != TileID.Stone && current != TileID.Granite && current != TileID.Marble && current != TileID.IceBlock) continue;


                WorldGen.KillTile(x, y, noItem: true);
                WorldGen.PlaceTile(x, y, fogGemType, mute: true, forced: true);
            }
        }
    }
}
