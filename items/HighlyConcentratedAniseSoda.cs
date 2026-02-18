using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Rarities;
namespace Etobudet1modtipo.items
{
    public class HighlyConcentratedAniseSoda : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.crit = 9;
            Item.DamageType = ModContent.GetInstance<EndlessThrower>();
            Item.width = 22;
            Item.height = 32;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ModContent.RarityType<AniseRarity>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;


            Item.shoot = ModContent.ProjectileType<HighlyConcentratedAniseSodaProjectile>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.None;
            Item.consumable = false;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }


        public override bool? UseItem(Player player)
        {
            int heal = 1;
            if (player.statLife < player.statLifeMax2)
            {
                player.statLife += heal;
                if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
                player.HealEffect(heal, true);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null, player.whoAmI, player.statLife, player.statMana);
            }

            return null;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "HighlyConcentratedAniseSodaDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.HighlyConcentratedAniseSoda.HighlyConcentratedAniseSodaDesc")));
        }
        

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AniseSoda>(), 5)
                .AddIngredient(ItemID.StarAnise, 10)
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 15)
                .Register();
        }
    }
}