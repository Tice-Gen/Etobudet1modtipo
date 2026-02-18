using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;

namespace Etobudet1modtipo.Systems
{
    public class BlackoutScreen : ModSystem
    {
        public static bool Enabled;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Interface Logic 1");
            if (index < 0)
                return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "Etobudet1modtipo: BlackoutScreen",
                DrawBlackout,
                InterfaceScaleType.None
            ));
        }

        private bool DrawBlackout()
        {
            if (!Enabled || Main.gameMenu)
                return true;

            SpriteBatch sb = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            sb.End();
            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            sb.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);

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
