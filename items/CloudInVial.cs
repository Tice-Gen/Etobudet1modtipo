using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Waist)]
    public class CloudInVial : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {


            player.GetJumpState<CloudInVialJump>().Enable();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 5)
                .AddIngredient(ItemID.Glass, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }



    public class CloudInVialJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(ExtraJump.BlizzardInABottle);

        public override float GetDurationMultiplier(Player player)
        {
            return 0.4f; 
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            playSound = true;


            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Cloud, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].noGravity = true;
            }



            for (int j = 0; j < 1; j++) 
            {


                int goreType = Main.rand.Next(11, 13);
                
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