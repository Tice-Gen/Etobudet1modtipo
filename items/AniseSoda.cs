using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Rarities; 
using Etobudet1modtipo.items; 


using Etobudet1modtipo;

namespace Etobudet1modtipo.items
{
    public class AniseSoda : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.FoodParticleColors[Type] = new Color[]
            {
                new Color(180, 255, 120),
                new Color(100, 160, 80)
            };
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.value = Item.buyPrice(silver: 5);
            
            Item.rare = ModContent.RarityType<AniseRarity>();

            Item.buffType = ModContent.BuffType<HealthyAniseBuff>();
            Item.buffTime = 60 * 60 * 4;

            Item.noUseGraphic = true; 
            Item.holdStyle = ItemHoldStyleID.HoldFront; 
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name == "ItemName")
                {
                    t.Text = Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AniseSoda.ItemName");

                }
            }
            tooltips.Add(new TooltipLine(Mod, "AniseSodaTooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AniseSoda.AniseSodaTooltip")));
        }
        
        public override void AddRecipes()
        {

            
            Recipe hardRecipe = CreateRecipe(5);
            hardRecipe.AddIngredient(ItemID.StarAnise, 15);
            hardRecipe.AddIngredient(ModContent.ItemType<TinCan>(), 1);
            hardRecipe.AddTile(TileID.WorkBenches);
            

            hardRecipe.AddCondition(new Condition(
                "Requires 'Hard Anise Recipes' mode enabled", 
                () => ModContent.GetInstance<EtobudetConfig>().UseHarderRecipes
            ));
            hardRecipe.Register();


            
            Recipe easyRecipe = CreateRecipe(5);
            easyRecipe.AddIngredient(ItemID.StarAnise, 15);
            easyRecipe.AddIngredient(ItemID.BottledWater, 1);
            

            easyRecipe.AddCondition(new Condition(
                "Requires 'Hard Anise Recipes' mode disabled", 
                () => !ModContent.GetInstance<EtobudetConfig>().UseHarderRecipes
            ));
            easyRecipe.Register();
        }
    }



    public class AniseSoda_HeldLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            
            if (player == null || player.HeldItem == null)
                return;
            if (player.HeldItem.type != ModContent.ItemType<AniseSoda>())
                return;

            Asset<Texture2D> asset = ModContent.Request<Texture2D>("Etobudet1modtipo/items/AniseSoda_Held", AssetRequestMode.ImmediateLoad);
            Texture2D tex = asset.Value;
            
            if (tex == null) return;


            Vector2 drawPos = drawInfo.ItemLocation - Main.screenPosition; 

            const int offset = 7; 
            drawPos.X += offset * player.direction;

            
            Vector2 origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);

            float rotation = player.itemRotation; 
            float scale = player.GetAdjustedItemScale(player.HeldItem);
            SpriteEffects effects = drawInfo.itemEffect;
            Color drawColor = drawInfo.itemColor; 

            DrawData dd = new DrawData(
                tex, 
                drawPos, 
                null, 
                drawColor, 
                rotation, 
                origin, 
                scale, 
                effects, 
                0
            );

            drawInfo.DrawDataCache.Add(dd);
        }
    }
}

