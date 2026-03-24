using Etobudet1modtipo.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework.Graphics;

namespace Etobudet1modtipo.Common.GlobalItems
{
    public class VisualRateGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            bool isVisualRate = item.rare == ModContent.RarityType<VisualRate>();
            bool isDeepnestRate = item.rare == ModContent.RarityType<DeepnestRare>();
            bool isSupremeRate = item.rare == ModContent.RarityType<SupremeRare>();
            bool isCuboRate = item.rare == ModContent.RarityType<Cuborare>();
            bool isCosmalRate = item.rare == ModContent.RarityType<Cosmal>();
            if (!isVisualRate && !isDeepnestRate && !isSupremeRate && !isCuboRate && !isCosmalRate)
            {
                return true;
            }

            if (line.Mod != "Terraria" || line.Name != "ItemName")
            {
                return true;
            }

            if (isCosmalRate)
            {
                DrawCosmalLine(line);
                return false;
            }

            Color textColor;
            Color borderColor;
            if (isVisualRate)
            {
                textColor = VisualRate.GetGradientColor();
                borderColor = VisualRate.GetOutlineColor();
            }
            else if (isDeepnestRate)
            {
                textColor = DeepnestRare.GetRarityColor();
                borderColor = DeepnestRare.GetOutlineColor();
            }
            else if (isCuboRate)
            {
                textColor = Cuborare.GetRarityColor();
                borderColor = Cuborare.GetOutlineColor();
            }
            else
            {
                textColor = SupremeRare.GetRarityColor();
                borderColor = SupremeRare.GetOutlineColor();
            }
            Vector2 basePosition = new Vector2(line.X, line.Y);
            var font = FontAssets.MouseText.Value;
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

            foreach (Vector2 offset in offsets)
            {
                ChatManager.DrawColorCodedString(
                    Main.spriteBatch,
                    font,
                    line.Text,
                    basePosition + offset,
                    borderColor,
                    line.Rotation,
                    line.Origin,
                    line.BaseScale);
            }

            ChatManager.DrawColorCodedString(
                Main.spriteBatch,
                font,
                line.Text,
                basePosition,
                textColor,
                line.Rotation,
                line.Origin,
                line.BaseScale);

            if (isSupremeRate)
            {
                DrawSupremeTwinkles(line, basePosition, textColor, borderColor);
            }
            else if (isCuboRate)
            {
                DrawCuboSquares(line, basePosition, textColor, borderColor);
            }

            return false;
        }

        private static void DrawCosmalLine(DrawableTooltipLine line)
        {
            var font = FontAssets.MouseText.Value;
            Vector2 basePosition = new Vector2(line.X, line.Y);
            float time = (float)Main.GlobalTimeWrappedHourly;
            int visibleChars = System.Math.Max(1, line.Text.Length - 1);
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

            for (int i = 0; i < line.Text.Length; i++)
            {
                char currentChar = line.Text[i];
                if (char.IsWhiteSpace(currentChar))
                {
                    continue;
                }

                float progress = i / (float)visibleChars;
                float wave = 0.5f + 0.5f * (float)System.Math.Sin(time * 2.1f + progress * MathHelper.TwoPi);
                Color baseFill = Cosmal.GetRarityColor(progress * 0.8f);
                Color nebulaFill = Cosmal.GetNebulaColor(progress * 1.3f + 0.35f);
                Color textColor = Color.Lerp(baseFill, nebulaFill, 0.25f + 0.2f * wave);
                Color borderColor = Color.Lerp(Cosmal.GetOutlineColor(progress * 0.9f), Color.White, 0.08f + 0.08f * wave);

                float offsetX = font.MeasureString(line.Text.Substring(0, i)).X * line.BaseScale.X;
                Vector2 charPosition = basePosition + new Vector2(offsetX, 0f);
                string character = currentChar.ToString();

                foreach (Vector2 offset in offsets)
                {
                    ChatManager.DrawColorCodedString(
                        Main.spriteBatch,
                        font,
                        character,
                        charPosition + offset,
                        borderColor,
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

            DrawCosmalStars(line, basePosition);
        }

        private static void DrawSupremeTwinkles(DrawableTooltipLine line, Vector2 basePosition, Color textColor, Color borderColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(line.Text) * line.BaseScale;
            Vector2 textCenter = basePosition + textSize * 0.5f;
            float time = (float)Main.GlobalTimeWrappedHourly;
            int seed = line.Text.GetHashCode();

            // Several potential spark points; each one appears only in short pulse windows.
            for (int i = 0; i < 6; i++)
            {
                float phase = time * 2.9f + seed * 0.00017f + i * 1.11f;
                float pulse = (float)System.Math.Sin(phase);
                if (pulse < 0.6f)
                {
                    continue;
                }

                float xNorm = 0.5f + (float)System.Math.Sin(phase * 1.91f) * 0.58f;
                float yNorm = 0.5f + (float)System.Math.Cos(phase * 2.27f) * 0.44f;
                Vector2 pos = new Vector2(
                    basePosition.X + textSize.X * MathHelper.Clamp(xNorm, 0f, 1f),
                    basePosition.Y + textSize.Y * MathHelper.Clamp(yNorm, 0f, 1f));

                Vector2 toCenter = pos - textCenter;
                if (toCenter.LengthSquared() > (textSize.LengthSquared() * 0.55f))
                {
                    continue;
                }

                float strength = (pulse - 0.6f) / 0.4f;
                Color starCore = Color.Lerp(textColor, Color.White, 0.65f) * (0.35f + 0.65f * strength);
                Color starGlow = Color.Lerp(borderColor, Color.White, 0.35f) * (0.3f + 0.5f * strength);

                DrawBurst(pixel, pos, starCore, starGlow, 1.4f + 2.1f * strength, phase * 0.65f);
            }
        }

        private static void DrawBurst(Texture2D pixel, Vector2 position, Color core, Color glow, float size, float rotation)
        {
            Rectangle vLine = new Rectangle(0, 0, 1, (int)(8f * size));
            Rectangle hLine = new Rectangle(0, 0, (int)(8f * size), 1);
            Vector2 lineOrigin = new Vector2(0.5f, 0.5f);

            Main.spriteBatch.Draw(pixel, position, vLine, glow, rotation, lineOrigin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, position, hLine, glow, rotation, lineOrigin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, position, vLine, core, rotation + MathHelper.PiOver4, lineOrigin, 0.9f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, position, hLine, core, rotation + MathHelper.PiOver4, lineOrigin, 0.9f, SpriteEffects.None, 0f);
        }

        private static void DrawCosmalStars(DrawableTooltipLine line, Vector2 basePosition)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(line.Text) * line.BaseScale;
            Vector2 textCenter = basePosition + textSize * 0.5f;
            float time = (float)Main.GlobalTimeWrappedHourly;
            int seed = line.Text.GetHashCode();

            for (int i = 0; i < 9; i++)
            {
                float phase = time * 1.9f + seed * 0.00013f + i * 0.88f;
                float pulse = 0.5f + 0.5f * (float)System.Math.Sin(phase);
                if (pulse < 0.58f)
                {
                    continue;
                }

                float xNorm = 0.5f + (float)System.Math.Sin(phase * 1.43f) * 0.58f;
                float yNorm = 0.5f + (float)System.Math.Cos(phase * 1.91f) * 0.42f;
                Vector2 pos = new Vector2(
                    basePosition.X + textSize.X * MathHelper.Clamp(xNorm, 0f, 1f),
                    basePosition.Y + textSize.Y * MathHelper.Clamp(yNorm, 0f, 1f));

                Vector2 toCenter = pos - textCenter;
                if (toCenter.LengthSquared() > textSize.LengthSquared() * 0.52f)
                {
                    continue;
                }

                float strength = (pulse - 0.58f) / 0.42f;
                Color starCore = Color.Lerp(Cosmal.GetNebulaColor(i * 0.13f), Color.White, 0.72f) * (0.25f + 0.65f * strength);
                Color starGlow = Color.Lerp(Cosmal.GetOutlineColor(i * 0.17f), Color.White, 0.3f) * (0.18f + 0.45f * strength);

                if ((seed + i) % 2 == 0)
                {
                    DrawBurst(pixel, pos, starCore, starGlow, 0.65f + 1.1f * strength, phase * 0.17f);
                }
                else
                {
                    DrawDotStar(pixel, pos, starCore, starGlow, 1.4f + 1.8f * strength);
                }
            }
        }

        private static void DrawDotStar(Texture2D pixel, Vector2 position, Color core, Color glow, float size)
        {
            int glowSize = System.Math.Max(2, (int)size);
            Rectangle glowRect = new Rectangle(
                (int)position.X - glowSize / 2,
                (int)position.Y - glowSize / 2,
                glowSize,
                glowSize);
            Rectangle coreRect = new Rectangle((int)position.X - 1, (int)position.Y - 1, 2, 2);

            Main.spriteBatch.Draw(pixel, glowRect, glow * 0.55f);
            Main.spriteBatch.Draw(pixel, coreRect, core);
        }

        private static void DrawCuboSquares(DrawableTooltipLine line, Vector2 basePosition, Color textColor, Color borderColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(line.Text) * line.BaseScale;
            Vector2 textCenter = basePosition + textSize * 0.5f;
            float time = (float)Main.GlobalTimeWrappedHourly;
            int seed = line.Text.GetHashCode();

            for (int i = 0; i < 6; i++)
            {
                float phase = time * 2.7f + seed * 0.00019f + i * 1.23f;
                float pulse = (float)System.Math.Sin(phase);
                if (pulse < 0.55f)
                {
                    continue;
                }

                float xNorm = 0.5f + (float)System.Math.Sin(phase * 1.73f) * 0.58f;
                float yNorm = 0.5f + (float)System.Math.Cos(phase * 2.11f) * 0.44f;
                Vector2 pos = new Vector2(
                    basePosition.X + textSize.X * MathHelper.Clamp(xNorm, 0f, 1f),
                    basePosition.Y + textSize.Y * MathHelper.Clamp(yNorm, 0f, 1f));

                Vector2 toCenter = pos - textCenter;
                if (toCenter.LengthSquared() > (textSize.LengthSquared() * 0.55f))
                {
                    continue;
                }

                float strength = (pulse - 0.55f) / 0.45f;
                Color squareCore = Color.Lerp(textColor, Color.White, 0.45f) * (0.20f + 0.48f * strength);
                Color squareGlow = Color.Lerp(borderColor, Color.White, 0.22f) * (0.10f + 0.28f * strength);
                float size = 2.8f + 3.6f * strength;

                DrawHollowSquare(pixel, pos, squareGlow, size + 1.5f, 1);
                DrawHollowSquare(pixel, pos, squareCore, size, 1);
            }
        }

        private static void DrawHollowSquare(Texture2D pixel, Vector2 center, Color color, float size, int thickness)
        {
            int s = System.Math.Max(2, (int)size);
            int x = (int)center.X - s / 2;
            int y = (int)center.Y - s / 2;
            int t = System.Math.Max(1, thickness);

            Rectangle top = new Rectangle(x, y, s, t);
            Rectangle bottom = new Rectangle(x, y + s - t, s, t);
            Rectangle left = new Rectangle(x, y, t, s);
            Rectangle right = new Rectangle(x + s - t, y, t, s);

            Main.spriteBatch.Draw(pixel, top, color);
            Main.spriteBatch.Draw(pixel, bottom, color);
            Main.spriteBatch.Draw(pixel, left, color);
            Main.spriteBatch.Draw(pixel, right, color);
        }
    }
}
