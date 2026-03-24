using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    public class VisualStudio : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.rare = ModContent.RarityType<VisualRate>();
            Item.value = Item.buyPrice(silver: 50);

            Item.noMelee = true;
            Item.accessory = true;
            Item.buffType = ModContent.BuffType<VisualStudioBuff>();
            Item.shoot = ModContent.ProjectileType<VisualStudioCode>();
        }
    }
}
