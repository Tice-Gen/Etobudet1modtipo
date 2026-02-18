using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class AniseRifle : ModItem
    {
        public override string Texture => "Etobudet1modtipo/items/AniseRifle";

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.crit = -9999;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 70);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<BulletAnise>();
            Item.shootSpeed = 18f;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ConsumeItem(ItemID.StarAnise);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AniseRifleDescription", Terraria.Localization.Language.GetTextValue("Cannot deal critical hits.\nUses star anise as ammo.")));
        }


        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-5f, 0f);
        }
    }  
}
