using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class Spythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 4500;
            Item.DamageType = DamageClass.Melee;
            Item.width = 96;
            Item.height = 89;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(gold: 50);
            Item.rare = ModContent.RarityType<Cosmal>();
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shootsEveryUse = true;
            Item.shoot = ModContent.ProjectileType<SpytheSwingProjectile>();
            Item.shootSpeed = 12f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SpytheSwingProjectile>()] <= 0;
        }

        public override bool MeleePrefix() => true;

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!Main.rand.NextBool(3))
                return;

            int dustIndex = Dust.NewDust(
                hitbox.TopLeft(),
                hitbox.Width,
                hitbox.Height,
                DustID.PurpleTorch,
                player.direction * 1.2f,
                -0.8f,
                100,
                new Color(190, 120, 255),
                1.2f
            );

            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0.45f;
            Lighting.AddLight(hitbox.Center.ToVector2(), 0.18f, 0.06f, 0.28f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
            int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
