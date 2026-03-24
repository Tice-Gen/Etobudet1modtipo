using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
using Etobudet1modtipo.Systems;

namespace Etobudet1modtipo.items
{
    public class DeepSeaRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 5000;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 24;

            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;

            Item.value = Item.buyPrice(gold: 50);
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.UseSound = new Terraria.Audio.SoundStyle("Etobudet1modtipo/Sounds/DeepShoot")
            {
                Volume = 1f,
                PitchVariance = 0f,
                MaxInstances = 16
            };
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<DeepGarp>();
            Item.shootSpeed = 18f;
            Item.useAmmo = CustomAmmoID.DeepSeaHook;
        }

        public override Vector2? HoldoutOffset()
        {
            // Move the held rifle 30 pixels closer to the player.
            return new Vector2(-30f, 0f);
        }
    }
}
