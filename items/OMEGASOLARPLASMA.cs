using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class OMEGASOLARPLASMA : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 999;
            Item.width = 26;
            Item.height = 30;
            Item.useTime = 1;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = -9999;
            Item.value = Item.buyPrice(platinum: 999);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.crit = -9999;
            Item.shoot = ModContent.ProjectileType<Sun>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity = 0f;
            maxFallSpeed = 0f;

            float time = Main.GlobalTimeWrappedHourly * 60f + Item.whoAmI * 13f;
            Vector2 targetVelocity = new Vector2(
                (float)System.Math.Sin(time * 0.09f) * 0.38f,
                (float)System.Math.Sin(time * 0.12f + 1.3f) * 0.2f
            );
            Item.velocity = Vector2.Lerp(Item.velocity, targetVelocity, 0.06f);

            Lighting.AddLight(Item.Center, 0.38f, 0.22f, 0.08f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 drawPos = Item.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            const float appearTicks = 10f;
            const float growTicks = 16f;
            const float fadeTicks = 54f;
            float cycleTicks = appearTicks + growTicks + fadeTicks;
            float timer = (Main.GlobalTimeWrappedHourly * 60f + whoAmI * 8f) % cycleTicks;

            float glowStrength;
            float glowScale;

            if (timer < appearTicks)
            {
                float t = timer / appearTicks;
                glowStrength = MathHelper.SmoothStep(0f, 0.55f, t);
                glowScale = MathHelper.Lerp(0.86f, 1f, t);
            }
            else if (timer < appearTicks + growTicks)
            {
                float t = (timer - appearTicks) / growTicks;
                glowStrength = MathHelper.SmoothStep(0.55f, 1f, t);
                glowScale = MathHelper.Lerp(1f, 1.26f, t);
            }
            else
            {
                float t = (timer - appearTicks - growTicks) / fadeTicks;
                glowStrength = MathHelper.SmoothStep(1f, 0f, t);
                glowScale = MathHelper.Lerp(1.26f, 1.34f, t);
            }

            if (glowStrength <= 0f)
                return;

            Color reflectionColor = new Color(255, 176, 62, 0) * (0.5f * glowStrength);
            float radius = 1.5f + glowScale * 1.8f;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 0.8f) * radius;
                spriteBatch.Draw(texture, drawPos + offset, null, reflectionColor, rotation, origin, scale * glowScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, reflectionColor * 0.8f, rotation, origin, scale * (glowScale * 1.06f), SpriteEffects.None, 0f);
        }
    }
}
