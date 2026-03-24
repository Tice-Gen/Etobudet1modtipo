using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class Astranise : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 86;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<AstraniseProj>();
            Item.shootSpeed = 17f;
            Item.useAmmo = AmmoID.None;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AniseBombard>(), 1)
                .AddIngredient(ModContent.ItemType<SkySteel>(), 1)
                .AddIngredient(ModContent.ItemType<DeepSeaBar>(), 10)
                .AddIngredient(ModContent.ItemType<FloweringChlorophyteBar>(), 77)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 77)
                .AddIngredient(ItemID.FragmentSolar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity = 0f;
            maxFallSpeed = 0f;

            float time = Main.GlobalTimeWrappedHourly * 60f + Item.whoAmI * 11f;
            Vector2 targetVelocity = new Vector2(
                (float)System.Math.Sin(time * 0.08f) * 0.35f,
                (float)System.Math.Sin(time * 0.11f + 1.7f) * 0.18f
            );
            Item.velocity = Vector2.Lerp(Item.velocity, targetVelocity, 0.06f);

            Lighting.AddLight(Item.Center, 0.34f, 0.2f, 0.07f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 drawPos = Item.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            const float appearTicks = 10f;
            const float growTicks = 16f;
            const float fadeTicks = 52f;
            float cycleTicks = appearTicks + growTicks + fadeTicks;
            float timer = (Main.GlobalTimeWrappedHourly * 60f + whoAmI * 9f) % cycleTicks;

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

            Color reflectionColor = new Color(255, 190, 70, 0) * (0.46f * glowStrength);
            float radius = 1.4f + glowScale * 1.7f;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 0.8f) * radius;
                spriteBatch.Draw(texture, drawPos + offset, null, reflectionColor, rotation, origin, scale * glowScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, reflectionColor * 0.8f, rotation, origin, scale * (glowScale * 1.06f), SpriteEffects.None, 0f);
        }
    }
}
