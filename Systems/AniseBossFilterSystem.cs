using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Etobudet1modtipo.NPCs;

namespace Etobudet1modtipo.Systems
{
    public class AniseBossFilterSystem : ModSystem
    {
        private const string FilterKey = "Etobudet1modtipo:AniseBossTint";


        private static readonly Vector3 TintColor = new Vector3(179f / 255f, 204f / 255f, 99f / 255f);

        private float current = 0f;
        private float target = 0f;
        private const int TransitionTicks = 60;

        private bool filterActivated = false;

        public override void Load()
        {
            if (!Main.dedServ)
            {

                var shader = new ScreenShaderData("FilterMiniTower")
                    .UseColor(TintColor)
                    .UseOpacity(0f);

                Filters.Scene[FilterKey] = new Filter(shader, EffectPriority.VeryHigh);
                Filters.Scene.Load();
            }
        }

        public override void Unload()
        {

            try
            {
                if (Filters.Scene.IsLoaded)
                {
                    Filters.Scene.Deactivate(FilterKey);
                }
            }
            catch
            {

            }
        }

        public override void PostUpdateEverything()
        {

            bool bossAlive = NPC.AnyNPCs(ModContent.NPCType<AniseKingSlime>());
            target = bossAlive ? 1f : 0f;


            float step = 1f / TransitionTicks;
            if (current < target)
                current = Math.Min(current + step, target);
            else if (current > target)
                current = Math.Max(current - step, target);


            float multiplier = 0.4f;
            float appliedOpacity = current * multiplier;



            try
            {
                if (Filters.Scene.IsLoaded && Filters.Scene[FilterKey] != null)
                {
                    var filter = Filters.Scene[FilterKey];


                    filter.GetShader().UseOpacity(appliedOpacity);


                    if (appliedOpacity > 0f)
                    {
                        if (!filterActivated)
                        {

                            Filters.Scene.Activate(FilterKey, Main.LocalPlayer.Center);
                            filterActivated = true;
                        }
                    }
                    else
                    {
                        if (filterActivated)
                        {
                            Filters.Scene.Deactivate(FilterKey);
                            filterActivated = false;
                        }
                    }
                }
            }
            catch (Exception)
            {


            }
        }
    }
}
