using Microsoft.Xna.Framework;
using Terraria;
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
    }
}