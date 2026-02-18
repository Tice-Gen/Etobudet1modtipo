using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class CobaltKnifeStorm : ModItem
    {

        private const int TotalProjectiles = 10;
        private const float FanAngle = 60f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 18;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<CobaltKnife>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.None;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
            int type, int damage, float knockback)
        {

            float halfFanAngleRad = MathHelper.ToRadians(FanAngle / 2f);
            

            for (int i = 0; i < TotalProjectiles; i++)
            {


                float lerpT = TotalProjectiles > 1 ? (float)i / (TotalProjectiles - 1) : 0.5f;
                

                float angleOffset = MathHelper.Lerp(-halfFanAngleRad, halfFanAngleRad, lerpT);


                Vector2 perturbedSpeed = velocity.RotatedBy(angleOffset);
                

                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }


            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, 0f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.Add(new TooltipLine(Mod, "CobaltKnifeStormDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CobaltKnifeStorm.CobaltKnifeStormDesc", TotalProjectiles)));
            

            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Name == "PoisonKnifeStormDesc")
                {
                    tooltips[i].Text = Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.CobaltKnifeStorm.NoAmmo");
                    break;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}