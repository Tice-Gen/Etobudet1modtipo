using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class NA : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 11;
            Item.height = 11;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.crit = 10;
        }
    }
}
