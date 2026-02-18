using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class BerserkBalde : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 58;
            Item.height = 64;
            Item.useTime = 17;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 8);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = false;
        }

        public override void UpdateInventory(Player player)
        {
            float hpPercentage = (float)player.statLife / player.statLifeMax2;
            float lostHp = 1f - hpPercentage;


            int steps = (int)(lostHp / 0.05f);


            Item.damage = (int)(50 * (1f + 0.25f * steps));


            float speedMultiplier = 5f - 0.01f * steps;
            
            if (speedMultiplier < 0.1f) speedMultiplier = 0.1f;

            Item.useTime = (int)(5 * speedMultiplier);
            Item.useAnimation = (int)(5 * speedMultiplier);
        }


        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {


            modifiers.ScalingArmorPenetration += 0.5f;
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && target.type != NPCID.TargetDummy && !target.friendly)
            {
                int healAmount = (int)(player.statLifeMax2 * 0.05f);
                player.Heal(healAmount);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.TitaniumBar, 12); 
            recipe1.AddIngredient(ItemID.DarkShard, 2);
            recipe1.AddIngredient(ItemID.SoulofNight, 5);  
            recipe1.AddTile(TileID.MythrilAnvil);          
            recipe1.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.AdamantiteBar, 12); 
            recipe2.AddIngredient(ItemID.DarkShard, 2);
            recipe2.AddIngredient(ItemID.SoulofNight, 5);    
            recipe2.AddTile(TileID.MythrilAnvil);            
            recipe2.Register();
        }
    }
}