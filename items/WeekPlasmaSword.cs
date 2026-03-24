using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class WeekPlasmaSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 54;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WeekPlasmaSwingProjectile>();
            Item.shootSpeed = 8f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.FieryGreatsword)
                .AddIngredient(ItemID.MeteoriteBar, 20)
                .AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 45)
                .AddIngredient(ItemID.HellstoneBar, 25)
                .AddCondition(Condition.NearShimmer)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.WeekPlasmaSword.Description"))
            {
                OverrideColor = Color.MediumPurple
            });
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 drawPos = Item.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            const float appearTicks = 10f;
            const float growTicks = 16f;
            const float fadeTicks = 46f;
            float cycleTicks = appearTicks + growTicks + fadeTicks;
            float timer = (Main.GlobalTimeWrappedHourly * 60f + whoAmI * 7f) % cycleTicks;

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

            Color reflectionColor = new Color(120, 240, 255, 0) * (0.45f * glowStrength);
            float radius = 1.2f + glowScale * 1.6f;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 4f + Main.GlobalTimeWrappedHourly * 0.8f) * radius;
                spriteBatch.Draw(texture, drawPos + offset, null, reflectionColor, rotation, origin, scale * glowScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, reflectionColor * 0.8f, rotation, origin, scale * (glowScale * 1.06f), SpriteEffects.None, 0f);
        }
    }
}
