using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Waist)]
    public class MiniSandStormInVial : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {


            player.GetJumpState<MiniSandStormInVialJump>().Enable();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SandBlock, 35)
                .AddIngredient(ItemID.Glass, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }


    public class MiniSandStormInVialJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(ExtraJump.BlizzardInABottle);

        public override float GetDurationMultiplier(Player player)
        {
            return 1f; 
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            playSound = true;


            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Sand, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].noGravity = true;
            }



            for (int j = 0; j < 3; j++) 
            {


                int goreType = Main.rand.Next(61, 63);
                
                var gore = Gore.NewGoreDirect(player.GetSource_FromThis(), player.Bottom, default, goreType, 1f);
                

                gore.velocity.X = gore.velocity.X + Main.rand.Next(-10, 11) * 0.05f;
                gore.velocity.Y = gore.velocity.Y + Main.rand.Next(-10, 11) * 0.05f;
            }
        }

        public override void UpdateHorizontalSpeeds(Player player) 
        {

        }
    }
}