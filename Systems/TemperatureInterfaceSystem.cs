using System.Collections.Generic;
using Etobudet1modtipo.Common.Temperature;
using Etobudet1modtipo.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Etobudet1modtipo.Systems
{
    public class TemperatureInterfaceSystem : ModSystem
    {
        private const int SegmentCount = 20;
        private const int OuterWidth = 214;
        private const int OuterHeight = 18;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (index < 0)
            {
                index = layers.FindIndex(layer => layer.Name == "Vanilla: Interface Logic 1");
            }

            if (index < 0)
            {
                return;
            }

            layers.Insert(
                index + 1,
                new LegacyGameInterfaceLayer(
                    "Etobudet1modtipo: Temperature Bar",
                    DrawTemperatureBar,
                    InterfaceScaleType.UI));
        }

        private bool DrawTemperatureBar()
        {
            if (Main.gameMenu || Main.hideUI)
            {
                return true;
            }

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
            {
                return true;
            }

            DrawTemperatureBar(Main.spriteBatch, player.GetModPlayer<TemperaturePlayer>());
            return true;
        }

        private void DrawTemperatureBar(SpriteBatch spriteBatch, TemperaturePlayer temperaturePlayer)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 origin = new Vector2(Main.screenWidth - 292f, 74f);

            Rectangle outer = new Rectangle((int)origin.X, (int)origin.Y, OuterWidth, OuterHeight);
            Rectangle inner = new Rectangle(outer.X + 3, outer.Y + 3, outer.Width - 6, outer.Height - 6);

            Color statusColor = temperaturePlayer.GetStatusColor();
            float pulse = temperaturePlayer.DamagePerSecond > 0f
                ? 0.82f + 0.18f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 10f)
                : 1f;

            DrawRect(spriteBatch, pixel, new Rectangle(outer.X + 2, outer.Y + 2, outer.Width, outer.Height), Color.Black * 0.45f);
            DrawRect(spriteBatch, pixel, outer, new Color(77, 54, 26));
            DrawRect(
                spriteBatch,
                pixel,
                new Rectangle(outer.X + 1, outer.Y + 1, outer.Width - 2, outer.Height - 2),
                Color.Lerp(new Color(188, 145, 78), statusColor, 0.28f) * pulse);
            DrawRect(spriteBatch, pixel, inner, new Color(24, 17, 12));

            DrawSegments(spriteBatch, pixel, inner, temperaturePlayer.FillRatio);
            DrawThresholdMarker(spriteBatch, pixel, inner, TemperatureRegistry.SafeMinTemperature, new Color(160, 220, 255));
            DrawThresholdMarker(spriteBatch, pixel, inner, TemperatureRegistry.SafeMaxTemperature, new Color(255, 210, 120));

            string label = Language.GetTextValue("Mods.Etobudet1modtipo.UI.Temperature.Label");
            string text = $"{label} {temperaturePlayer.CurrentTemperature:0}C | {temperaturePlayer.GetStatusText()}";
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                FontAssets.MouseText.Value,
                text,
                outer.X + 2f,
                outer.Bottom + 2f,
                Color.Lerp(Color.White, statusColor, 0.7f),
                Color.Black,
                Vector2.Zero,
                0.68f);
        }

        private void DrawSegments(SpriteBatch spriteBatch, Texture2D pixel, Rectangle area, float fillRatio)
        {
            int gap = 1;
            int segmentWidth = (area.Width - (SegmentCount - 1) * gap) / SegmentCount;
            float filledSegments = fillRatio * SegmentCount;

            for (int i = 0; i < SegmentCount; i++)
            {
                int x = area.X + i * (segmentWidth + gap);
                Rectangle segment = new Rectangle(x, area.Y, segmentWidth, area.Height);

                float segmentTemperature = MathHelper.Lerp(
                    TemperatureRegistry.DisplayMinTemperature,
                    TemperatureRegistry.DisplayMaxTemperature,
                    (i + 0.5f) / SegmentCount);

                Color segmentColor = TemperatureRegistry.GetTemperatureColor(segmentTemperature);
                DrawRect(spriteBatch, pixel, segment, segmentColor * 0.2f);

                float fillAmount = MathHelper.Clamp(filledSegments - i, 0f, 1f);
                if (fillAmount > 0f)
                {
                    int fillWidth = System.Math.Max(1, (int)System.Math.Round(segment.Width * fillAmount));
                    Rectangle fillRect = new Rectangle(segment.X, segment.Y, fillWidth, segment.Height);
                    DrawRect(spriteBatch, pixel, fillRect, segmentColor);
                    DrawRect(spriteBatch, pixel, new Rectangle(fillRect.X, fillRect.Y, fillRect.Width, 1), Color.White * 0.28f);
                }

                DrawRect(spriteBatch, pixel, new Rectangle(segment.X, segment.Bottom - 1, segment.Width, 1), Color.Black * 0.32f);
            }
        }

        private void DrawThresholdMarker(SpriteBatch spriteBatch, Texture2D pixel, Rectangle area, float threshold, Color color)
        {
            float normalized = (threshold - TemperatureRegistry.DisplayMinTemperature)
                / (TemperatureRegistry.DisplayMaxTemperature - TemperatureRegistry.DisplayMinTemperature);

            int x = area.X + (int)System.Math.Round(area.Width * normalized);
            DrawRect(spriteBatch, pixel, new Rectangle(x, area.Y - 1, 2, area.Height + 2), color * 0.85f);
        }

        private void DrawRect(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(pixel, rectangle, color);
        }
    }
}
