using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Classes;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Body)]
    public class FloweringChlorophyteBreastplate : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.defense = 25;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage<EndlessThrower>() += 0.09f;
            player.GetCritChance<EndlessThrower>() += 0.09f;


            player.GetJumpState<FloweringJump>().Enable();



            Lighting.AddLight(player.Center, 1f, 2f, 1f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "FloweringChlorophyteBreastplateDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FloweringChlorophyteBreastplate.FloweringChlorophyteBreastplateDesc")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FloweringChlorophyteBar>(), 21)
                .AddIngredient(ItemID.ChlorophyteBar, 18)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }


    public class FloweringJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(CloudInABottle);

        public override float GetDurationMultiplier(Player player) => 1.2f;


        public override void ShowVisuals(Player player)
        {
            for (int i = 0; i < 2; i++)
            {

                int dustType = Main.rand.NextBool() ? DustID.PinkFairy : DustID.PinkTorch;

                Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, player.height - 10), player.width, 10, dustType);
                dust.velocity.Y = 2f; 
                dust.velocity.X *= 0.5f; 
                dust.scale = 1.2f;
                dust.noGravity = true; 
            }
        }


        public override void OnStarted(Player player, ref bool playSound)
        {
            for (int i = 0; i < 15; i++)
            {

                int dustType = Main.rand.NextBool() ? DustID.PinkFairy : DustID.PinkTorch;
                
                Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, player.height - 6), player.width, 10, dustType);
                dust.velocity *= 2f; 
                dust.scale = 1.5f;
                dust.noGravity = true;
            }
        }
    }
}