using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Terraria.GameContent;
using Etobudet1modtipo.Systems.InfernalAwakening;

namespace Etobudet1modtipo.Systems
{
    public class InfernalHellVisuals : ModSystem
    {
        private static float strength;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(
                l => l.Name == "Vanilla: Interface Logic 1"
            );

            if (index < 0)
                return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "Etobudet1modtipo: InfernalHellTint",
                Draw,
                InterfaceScaleType.None
            ));
        }

        private bool Draw()
        {
            if (Main.gameMenu)
                return true;

            Player player = Main.LocalPlayer;

            bool active =
                InfernalAwakeningSystem.IsActive() &&
                player.ZoneUnderworldHeight;

            strength = MathHelper.Lerp(
                strength,
                active ? 1f : 0f,
                0.05f
            );

            if (strength < 0.01f)
                return true;

            Texture2D pixel = TextureAssets.MagicPixel.Value;


            Color infernalPink = new Color(255, 90, 150);

            Main.spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                infernalPink * (0.16f * strength)
            );

            return true;
        }
    }
}
