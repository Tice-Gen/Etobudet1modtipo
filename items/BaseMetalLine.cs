using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public abstract class BaseMetalLine : ModItem
    {
        protected abstract int BarItemType { get; }
        protected abstract int YoyoProjectileType { get; }
        protected abstract int BaseDamage { get; }
        protected abstract int ItemRarity { get; }

        protected virtual int BarCount => 12;
        protected virtual int CobwebCount => 10;
        protected virtual int UseTime => 24;
        protected virtual float KnockBack => 2.5f;
        protected virtual int ItemValue => Terraria.Item.buyPrice(silver: 70);
        protected virtual float ShootSpeed => 13f;
        protected virtual int ItemWidth => 30;
        protected virtual int ItemHeight => 30;
        protected virtual string OrbitalDescription => null;
        protected virtual Color OrbitalTagStartColor => new Color(0, 120, 180);
        protected virtual Color OrbitalTagEndColor => Color.LimeGreen;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.width = ItemWidth;
            Item.height = ItemHeight;
            Item.useTime = UseTime;
            Item.useAnimation = UseTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = KnockBack;
            Item.value = ItemValue;
            Item.rare = ItemRarity;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = YoyoProjectileType;
            Item.shootSpeed = ShootSpeed;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
                .AddIngredient(BarItemType, BarCount)
                .AddIngredient(ItemID.Cobweb, CobwebCount)
                .AddTile(TileID.Anvils);

            recipe.AddOnCraftCallback(static (_, item, _, _) =>
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    return;
                }

                Player localPlayer = Main.LocalPlayer;
                if (localPlayer == null || !localPlayer.active)
                {
                    return;
                }

                localPlayer.GetModPlayer<OrbitAchievementPlayer>().RegisterOrbitalYoyoCraft(item.type);
            });

            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damageIndex = tooltips.FindIndex(t => t.Name == "Damage");
            TooltipLine orbitalLine = new TooltipLine(Mod, "OrbitalYoyoTag", "-Orbital Yoyo-");
            float wave = (float)((Math.Sin(Main.GlobalTimeWrappedHourly * 2f) + 1.0) * 0.5);
            orbitalLine.OverrideColor = Color.Lerp(OrbitalTagStartColor, OrbitalTagEndColor, wave);

            if (damageIndex >= 0)
            {
                tooltips.Insert(damageIndex, orbitalLine);
            }
            else
            {
                tooltips.Add(orbitalLine);
            }

            if (!string.IsNullOrWhiteSpace(OrbitalDescription))
            {
                tooltips.Add(new TooltipLine(Mod, "Description", OrbitalDescription));
            }
        }
    }
}
