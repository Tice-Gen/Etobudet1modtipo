using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class StaffOfUnquenchableLava : ModItem
    {
        public override void SetStaticDefaults()
        {


            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.UnquenchableLava>();
            Item.shootSpeed = 19f;
            Item.mana = 15;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;
        }
    }
}
