using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.items
{
    public class DeepSeaCutter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 2100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 112;
            Item.height = 112;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(gold: 60);
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DeepSeaPressure>();
            Item.shootSpeed = 15f;
        }

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
    }
}
