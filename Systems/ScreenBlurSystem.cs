using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using System.Collections.Generic;

namespace Etobudet1modtipo.Systems
{
    public class ScreenBlurSystem : ModSystem
    {
        public static bool Enabled;
        private static float strength;

        private static RenderTarget2D rtSmall;
        private static RenderTarget2D rtFull;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;





                rtSmall = new RenderTarget2D(
                    gd,
                    Main.screenWidth / 1,
                    Main.screenHeight / 1,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );


                rtFull = new RenderTarget2D(
                    gd,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );
            });
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            RenderTarget2D small = rtSmall;
            RenderTarget2D full = rtFull;

            rtSmall = null;
            rtFull = null;

            Main.QueueMainThreadAction(() =>
            {
                small?.Dispose();
                full?.Dispose();
            });
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index < 0)
                return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "Etobudet1modtipo: ScreenBlur",
                DrawEffect,
                InterfaceScaleType.None
            ));
        }

        private bool DrawEffect()
        {
            if (Main.gameMenu)
                return true;

            if (Main.screenTarget == null)
                return true;

            if (!Enabled && strength <= 0f)
                return true;





            strength = MathHelper.Lerp(
                strength,
                Enabled ? 1f : 0f,
                0.06f
            );


            if (strength < 0.01f)
                return true;

            if (rtSmall == null || rtFull == null)
                return true;

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;

            sb.End();


            gd.SetRenderTarget(rtSmall);
            gd.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(
                Main.screenTarget,
                new Rectangle(0, 0, rtSmall.Width, rtSmall.Height),
                Color.White
            );
            sb.End();


            gd.SetRenderTarget(rtFull);
            gd.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);



            float time = (float)Main.timeForVisualEffects * 0.04f;






            float offset = 12f * strength;


            sb.Draw(
                rtSmall,
                new Rectangle(

                    (int)(MathF.Sin(time) * offset),
                    (int)(MathF.Cos(time * 1.3f) * offset),

                    Main.screenWidth,
                    Main.screenHeight
                ),
                Color.White
            );

            sb.End();


            gd.SetRenderTarget(null);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(rtFull, Vector2.Zero, Color.White);
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            return true;
        }
    }
}
