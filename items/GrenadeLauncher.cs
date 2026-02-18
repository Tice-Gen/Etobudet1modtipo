using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class GrenadeLauncher : ModItem
    {
        private static readonly HashSet<int> SupportedGrenades = new()
        {
            ItemID.Grenade,

            ModContent.ItemType<FragmentationGrenade>()
        };

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 90);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;

            Item.shoot = ProjectileID.Grenade;
            Item.shootSpeed = 12f;
            Item.noMelee = true;
        }


        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.Grenade, 50);
            recipe1.AddIngredient(ItemID.IronBar, 35);
            recipe1.AddIngredient(ItemID.FlintlockPistol);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Grenade, 50);
            recipe2.AddIngredient(ItemID.LeadBar, 35);
            recipe2.AddIngredient(ItemID.FlintlockPistol);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }


        public override bool CanUseItem(Player player)
        {
            foreach (Item it in player.inventory)
            {
                if (it != null && it.stack > 0 && SupportedGrenades.Contains(it.type))
                    return true;
            }
            return false;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            Item found = null;
            foreach (Item it in player.inventory)
            {
                if (it != null && it.stack > 0 && SupportedGrenades.Contains(it.type))
                {
                    found = it;
                    break;
                }
            }

            if (found == null)
            {

                return;
            }


            if (found.type == ModContent.ItemType<FragmentationGrenade>())
            {
                type = ModContent.ProjectileType<Projectiles.FragmentationGrenadeProj>();
            }
            else
            {
                type = ProjectileID.Grenade;
            }


            found.stack--;
            if (found.stack <= 0)
                found.TurnToAir();




        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
{
    tooltips.Add(new TooltipLine(Mod, "GrenadeLauncherDescription", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.GrenadeLauncher.GrenadeLauncherDescription")));
}


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
    }
}
