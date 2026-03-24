using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.Audio;
namespace Etobudet1modtipo.items
{
    public class BrokenRodOfTeleporting : ModItem
    {

        private const int CooldownTime = 60 * 60;
        
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 2);
            

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item6;
            Item.noMelee = true;
        }
        
        public override bool CanUseItem(Player player)
        {

            FogStoneArmorPlayer modPlayer = player.GetModPlayer<FogStoneArmorPlayer>();
            
            if (!modPlayer.hasBrokenRodAccess)
            {
                Main.NewText("...", Color.Red);
                return false;
            }
            

            if (player.HasBuff(ModContent.BuffType<BrokenRodCooldown>()))
            {
                Main.NewText("The rod is recharging!", Color.Yellow);
                return false;
            }
            
            return true;
        }
        
        public override bool? UseItem(Player player)
        {

            

            TeleportPlayer(player);
            

            player.AddBuff(ModContent.BuffType<BrokenRodCooldown>(), CooldownTime);
            
            return true;
        }
        
        private void TeleportPlayer(Player player)
        {
            Vector2 teleportPosition;
            

            Vector2 mousePosition = Main.MouseWorld;
            

            if (IsValidTeleportPosition(mousePosition, player))
            {
                teleportPosition = mousePosition;
            }
            else
            {

                teleportPosition = player.position;
                teleportPosition.X += player.direction * 100;
            }
            

            player.Teleport(teleportPosition, 1);
            

            for (int i = 0; i < 70; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default(Color), 1.5f);
                dust.velocity *= 0.5f;
            }
            

            SoundEngine.PlaySound(SoundID.Item6, player.Center);
        }
        
        private bool IsValidTeleportPosition(Vector2 position, Player player)
        {

            int tileX = (int)(position.X / 16);
            int tileY = (int)(position.Y / 16);
            

            if (tileX < 10 || tileX > Main.maxTilesX - 10 || tileY < 10 || tileY > Main.maxTilesY - 10)
                return false;
            

            Tile tile = Framing.GetTileSafely(tileX, tileY);
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                return false;
            

            for (int y = tileY - 2; y < tileY; y++)
            {
                Tile tileAbove = Framing.GetTileSafely(tileX, y);
                if (tileAbove.HasTile && Main.tileSolid[tileAbove.TileType])
                    return false;
            }
            
            return true;
        }
        
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.Add(new TooltipLine(Mod, "Requirement", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.BrokenRodOfTeleporting.Requirement")));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.BrokenRodOfTeleporting.Cooldown")));
        }
    }
    

    public class BrokenRodCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
        
        public override void Update(Player player, ref int buffIndex)
        {

        }
    }
}
