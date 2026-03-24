using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;


namespace Etobudet1modtipo.items
{
    public class CosmicDevourer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 4400;
            Item.DamageType = DamageClass.Melee;
            Item.width = 112;
            Item.height = 112;
            Item.useTime = 20;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(gold: 99);
            Item.rare = ModContent.RarityType<Cosmal>();
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shootsEveryUse = true;
            Item.shoot = ModContent.ProjectileType<CosmicDevourerSwingProjectile>();
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<CosmicDevourerSwingProjectile>()] <= 0;
        }

        public override bool MeleePrefix() => true;

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                int dustIndex = Dust.NewDust(
                    hitbox.TopLeft(),
                    hitbox.Width,
                    hitbox.Height,
                    DustID.FireworkFountain_Blue,
                    player.direction * 1.5f,
                    -1.2f,
                    120,
                    default,
                    1.35f
                );

                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.55f;
            }

            Lighting.AddLight(hitbox.Center.ToVector2(), 0.08f, 0.55f, 0.8f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
            int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            DrawPulse(spriteBatch, texture, position, frame, origin, 0f, scale, Type * 6f);
            spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 drawPos = Item.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            DrawPulse(spriteBatch, texture, drawPos, null, origin, rotation, scale, whoAmI * 6f);
        }

        private static void DrawPulse(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle? sourceRect, Vector2 origin, float rotation, float scale, float timeOffset)
        {
            const float appearTicks = 12f;
            const float growTicks = 18f;
            const float fadeTicks = 58f;
            float cycleTicks = appearTicks + growTicks + fadeTicks;
            float timer = (Main.GlobalTimeWrappedHourly * 60f + timeOffset) % cycleTicks;

            float glowStrength;
            float glowScale;

            if (timer < appearTicks)
            {
                float t = timer / appearTicks;
                glowStrength = MathHelper.SmoothStep(0f, 0.48f, t);
                glowScale = MathHelper.Lerp(0.9f, 1f, t);
            }
            else if (timer < appearTicks + growTicks)
            {
                float t = (timer - appearTicks) / growTicks;
                glowStrength = MathHelper.SmoothStep(0.48f, 0.95f, t);
                glowScale = MathHelper.Lerp(1f, 1.22f, t);
            }
            else
            {
                float t = (timer - appearTicks - growTicks) / fadeTicks;
                glowStrength = MathHelper.SmoothStep(0.95f, 0f, t);
                glowScale = MathHelper.Lerp(1.22f, 1.3f, t);
            }

            if (glowStrength <= 0f)
                return;

            Color reflectionColor = new Color(222, 205, 255, 0) * (0.4f * glowStrength);
            float radius = 1.35f + glowScale * 1.65f;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 0.75f) * radius;
                spriteBatch.Draw(texture, drawPos + offset, sourceRect, reflectionColor, rotation, origin, scale * glowScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, sourceRect, reflectionColor * 0.78f, rotation, origin, scale * (glowScale * 1.05f), SpriteEffects.None, 0f);
        }
    }
}
