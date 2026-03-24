using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{
    public class MegaTrap : ModItem
    {
        private const int TrapDamage = 10;
        private const float TrapShotSpeed = 15f;
        private const float TrapSpreadDegrees = 5f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 31;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack; 
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(silver: 90);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Dig;

            Item.damage = TrapDamage;
            Item.knockBack = 1f;
            Item.noMelee = true;

            Item.createTile = ModContent.TileType<Tiles.MegaTrapTile>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useAnimation = 6;
                Item.useTime = 6;
                Item.UseSound = SoundID.Item63;
                Item.noUseGraphic = false;
                Item.shoot = ModContent.ProjectileType<TrapDart>();
                Item.shootSpeed = TrapShotSpeed;
                Item.createTile = -1;
                Item.consumable = false;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useAnimation = 10;
                Item.useTime = 10;
                Item.UseSound = SoundID.Dig;
                Item.noUseGraphic = false;
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                Item.createTile = ModContent.TileType<Tiles.MegaTrapTile>();
                Item.consumable = true;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
            int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                return false;

            Vector2 shootVelocity = velocity;
            if (shootVelocity.LengthSquared() <= 0.001f)
                shootVelocity = new Vector2(player.direction, 0f);

            shootVelocity = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction)
                .RotatedByRandom(MathHelper.ToRadians(TrapSpreadDegrees))
                * TrapShotSpeed;

            int projectileIndex = Projectile.NewProjectile(source, position, shootVelocity, type, TrapDamage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileIndex];
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.trap = false;
            projectile.DamageType = DamageClass.Ranged;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damageLineIndex = tooltips.FindIndex(line => line.Name == "Damage" && line.Mod == "Terraria");
            TooltipLine trapLine = new TooltipLine(Mod, "MegaTrapTag", "-Trap weapon-")
            {
                OverrideColor = GetTrapTagColor()
            };

            if (damageLineIndex >= 0)
            {
                tooltips.Insert(damageLineIndex, trapLine);
            }
            else
            {
                tooltips.Add(trapLine);
            }

            tooltips.Add(new TooltipLine(Mod, "MegaTrapPlaceInfo", "Turret for floors, ceilings, and walls"));
            tooltips.Add(new TooltipLine(Mod, "MegaTrapRotateInfo", "Use a hammer to rotate it by 45 degrees"));
            tooltips.Add(new TooltipLine(Mod, "MegaTrapRightClickInfo", "Hold Right click to shoot like a weapon"));
        }

        private static Color GetTrapTagColor()
        {
            float wave = (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f) * 0.5f;
            return Color.Lerp(new Color(145, 145, 145), Color.White, wave);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap, 5)
                .AddIngredient(ItemID.PoisonDart, 100)
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.FlintlockPistol)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
