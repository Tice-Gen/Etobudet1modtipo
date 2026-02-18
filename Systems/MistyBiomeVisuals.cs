using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Etobudet1modtipo.Systems
{

    public class MistyBiomeVisuals : ModSystem
    {
        public static bool Enabled;
        private static float strength;

        private static RenderTarget2D rtSmallBiome;
        private static RenderTarget2D rtFullBiome;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;

                rtSmallBiome = new RenderTarget2D(
                    gd,
                    (int)(Main.screenWidth / 1.5f),
                    (int)(Main.screenHeight / 1.5f),
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );

                rtFullBiome = new RenderTarget2D(
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

            RenderTarget2D small = rtSmallBiome;
            RenderTarget2D full = rtFullBiome;

            rtSmallBiome = null;
            rtFullBiome = null;

            Main.QueueMainThreadAction(() =>
            {
                small?.Dispose();
                full?.Dispose();
            });
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Interface Logic 1");
            if (index < 0)
                return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "Etobudet1modtipo: MistyBiomeVisuals",
                DrawEffect,
                InterfaceScaleType.None
            ));
        }

        private bool DrawEffect()
        {
            if (Main.gameMenu || Main.screenTarget == null)
                return true;


            strength = MathHelper.Lerp(
                strength,
                Enabled ? 1f : 0f,
                0.06f
            );

            if (strength < 0.01f)
                return true;

            if (rtSmallBiome == null || rtFullBiome == null)
                return true;

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;


            sb.End();


            gd.SetRenderTarget(rtSmallBiome);
            gd.Clear(Color.Transparent);

            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            sb.Draw(
                Main.screenTarget,
                new Rectangle(0, 0, rtSmallBiome.Width, rtSmallBiome.Height),
                Color.White
            );

            sb.End();


            gd.SetRenderTarget(rtFullBiome);
            gd.Clear(Color.Transparent);

            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            float time = (float)Main.timeForVisualEffects * 0.04f;
            float offset = 8f * strength;

            sb.Draw(
                rtSmallBiome,
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


            float dimFactor = 1f - 0.06f * strength;
            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            sb.Draw(rtFullBiome, Vector2.Zero, Color.White * dimFactor);


            float desatAmount = 0.18f * strength;
            if (desatAmount > 0f)
            {
                Texture2D pixel = TextureAssets.MagicPixel.Value;
                Color gray = new Color(128, 128, 128) * desatAmount;
                sb.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), gray);
            }

            sb.End();


            sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.UIScaleMatrix
            );

            return true;
        }
    }
}
