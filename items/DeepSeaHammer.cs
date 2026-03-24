using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class DeepSeaHammer : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 635;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();

            Item.width = 44;
            Item.height = 44;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.shoot = ModContent.ProjectileType<DeepSeaHammerProj>();
            Item.shootSpeed = 10f;

            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 27;
            Item.noMelee = true;
        }

        public override bool MeleePrefix() => true;
    }
}
