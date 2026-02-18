using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Players;
using Etobudet1modtipo.Global;

namespace Etobudet1modtipo.items
{
    public class Shroomyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 135;
            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<ShroomyoProj>();
            Item.shootSpeed = 16f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
                return base.CanUseItem(player);


            if (Main.myPlayer != player.whoAmI)
                return false;

            ShroomyoPlayer shroomyoPlayer = player.GetModPlayer<ShroomyoPlayer>();
            if (shroomyoPlayer.ShroomyoEmpowerCooldown > 0)
            {
                ShowCooldownMessage(player, shroomyoPlayer.ShroomyoEmpowerCooldown);
                return false;
            }

            if (shroomyoPlayer.HasEmpoweredYoyoActive())
            {
                Main.NewText("Shroomyo is already empowered.", Color.LightGray);
                return false;
            }

            if (shroomyoPlayer.HasQueuedEmpower())
            {
                Main.NewText("Shroomyo empower is already prepared.", Color.LightGray);
                return false;
            }

            int spentPoints = ShroomyoInfectionGlobalNPC.ConsumeOneInfectionFromAllNPCs();
            if (spentPoints <= 0)
            {
                Main.NewText("No infected targets available for Shroomyo empower.", Color.LightGray);
                return false;
            }

            Projectile activeYoyo = shroomyoPlayer.FindActiveShroomyo();
            if (activeYoyo == null)
            {
                shroomyoPlayer.QueueEmpowerForNextShroomyo();
                CombatText.NewText(player.Hitbox, new Color(170, 255, 170), "Empower prepared");
                Main.NewText($"Shroomyo empower prepared (+15%, spent: {spentPoints})", new Color(170, 255, 170));
                return false;
            }

            shroomyoPlayer.ActivateEmpower(activeYoyo);
            CombatText.NewText(player.Hitbox, new Color(170, 255, 170), "Shroomyo +15%");
            Main.NewText($"Shroomyo empowered (+15%, spent: {spentPoints})", new Color(170, 255, 170));
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("The yoyo creates a powerful aura that can infect enemies.\nPierces all armor.")));
            int damageIndex = tooltips.FindIndex(t => t.Name == "Damage");
            TooltipLine auraLine = new TooltipLine(Mod, "AuraYoyoTag", "-Aura Yoyo-");
            float wave = (float)((System.Math.Sin(Main.GlobalTimeWrappedHourly * 3.2f) + 1.0) * 0.5);
            Color c1 = new Color(118, 67, 170);
            Color c2 = new Color(181, 236, 132);
            auraLine.OverrideColor = Color.Lerp(c1, c2, wave);

            if (damageIndex >= 0)
                tooltips.Insert(damageIndex, auraLine);
            else
                tooltips.Add(auraLine);
        }

        private static void ShowCooldownMessage(Player player, int cooldownTicks)
        {
            float secondsLeft = cooldownTicks / 60f;
            string msg = $"Shroomyo cooldown: {secondsLeft:0.0}s";
            CombatText.NewText(player.Hitbox, Color.OrangeRed, msg);
            Main.NewText(msg, Color.OrangeRed);
        }
    }
}

