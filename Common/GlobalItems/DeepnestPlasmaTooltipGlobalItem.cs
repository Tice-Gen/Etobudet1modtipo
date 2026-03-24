using Etobudet1modtipo.items;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class DeepnestPlasmaTooltipGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.type != ModContent.ItemType<DeepnestPlasma>())
                return true;

            if (line.Mod != "Etobudet1modtipo" || line.Name != "DeepnestPlasmaDamage")
                return true;

            DrawGradientLine(
                line,
                new Color(255, 60, 60),
                new Color(60, 130, 255),
                new Color(60, 130, 255),
                new Color(255, 60, 60)
            );

            return false;
        }

        private static void DrawGradientLine(DrawableTooltipLine line, Color textStart, Color textEnd, Color outlineStart, Color outlineEnd)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 basePosition = new Vector2(line.X, line.Y);
            float time = (float)Main.GlobalTimeWrappedHourly;
            float gradientOffset = time * 1.2f;
            float pulse = 0.18f + 0.12f * (float)System.Math.Sin(time * 6f);
            Vector2[] offsets =
            {
                new Vector2(-1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, -1f),
                new Vector2(0f, 1f),
                new Vector2(-1f, -1f),
                new Vector2(-1f, 1f),
                new Vector2(1f, -1f),
                new Vector2(1f, 1f)
            };

            int visibleChars = System.Math.Max(1, line.Text.Length - 1);

            for (int i = 0; i < line.Text.Length; i++)
            {
                string character = line.Text[i].ToString();
                float progress = i / (float)visibleChars;
                float animatedProgress = (progress + gradientOffset) % 1f;
                float wave = animatedProgress <= 0.5f ? animatedProgress * 2f : (1f - animatedProgress) * 2f;
                Color textColor = Color.Lerp(textStart, textEnd, wave);
                Color outlineColor = Color.Lerp(outlineStart, outlineEnd, wave);
                textColor = Color.Lerp(textColor, Color.White, pulse);
                outlineColor = Color.Lerp(outlineColor, Color.White, pulse * 0.35f);
                float offsetX = font.MeasureString(line.Text.Substring(0, i)).X * line.BaseScale.X;
                Vector2 charPosition = basePosition + new Vector2(offsetX, 0f);

                if (!char.IsWhiteSpace(line.Text[i]))
                {
                    foreach (Vector2 offset in offsets)
                    {
                        ChatManager.DrawColorCodedString(
                            Main.spriteBatch,
                            font,
                            character,
                            charPosition + offset,
                            outlineColor,
                            line.Rotation,
                            Vector2.Zero,
                            line.BaseScale);
                    }

                    ChatManager.DrawColorCodedString(
                        Main.spriteBatch,
                        font,
                        character,
                        charPosition,
                        textColor,
                        line.Rotation,
                        Vector2.Zero,
                        line.BaseScale);
                }
            }
        }
    }
}
