using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class ObsidianDemonicScytheAwakened : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;

            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.crit = 10;

            Item.shoot = ModContent.ProjectileType<ObsidianDemonicScytheSwordProjectile_Awakened>();
            Item.shootSpeed = 12f;
            Item.shootsEveryUse = true;
        }

        public override bool CanUseItem(Player player)
        {

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, player.GetAdjustedItemScale(Item));

            var scythePlayer = player.GetModPlayer<ScytheQueuePlayer>();
            scythePlayer.StartScytheQueue(velocity, damage, knockback);

            return false;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Awakened", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ObsidianDemonicScytheAwakened.Awakened")));
        }
    }
}
