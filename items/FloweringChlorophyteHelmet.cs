using Terraria;

using Terraria.ID;

using Terraria.ModLoader;

using Etobudet1modtipo.Classes; 

using System.Collections.Generic;

using Etobudet1modtipo.Buffs;

using Etobudet1modtipo.Projectiles;

using Microsoft.Xna.Framework; 



namespace Etobudet1modtipo.items

{

    [AutoloadEquip(EquipType.Head)]

    public class FloweringChlorophyteHelmet : ModItem

    {

        public override void SetStaticDefaults() { }



        public override void SetDefaults()

        {

            Item.width = 18;

            Item.height = 18;

            Item.defense = 21;

            Item.rare = ItemRarityID.Yellow;

            Item.value = Item.buyPrice(gold: 10);

        }



        public override void UpdateEquip(Player player)

        {



            player.GetDamage<EndlessThrower>() += 0.20f;



            player.GetCritChance<EndlessThrower>() += 12;





            player.buffImmune[BuffID.Poisoned] = true; 

            player.buffImmune[BuffID.Ichor] = true;    

            player.buffImmune[BuffID.Venom] = true; 

            player.buffImmune[ModContent.BuffType<HighlyConcentratedTaste>()] = true; 



            Lighting.AddLight(player.Center, 1f, 2f, 1f);





            if (Main.rand.NextBool(40)) 

            {

                float verticalOffset = (player.gravDir == 1) 

                    ? -player.height * 0.3f - 10f    

                    : player.height * 0.3f + 10f;    



                Vector2 spawnPos = player.Center + new Vector2(

                    Main.rand.NextFloat(-player.width * 0.5f, player.width * 0.5f),

                    verticalOffset + Main.rand.NextFloat(-6f, 6f)

                );



                int d = Dust.NewDust(spawnPos, 2, 2, DustID.GoldCoin, 0f, 0f, 100, default(Color), Main.rand.NextFloat(0.6f, 1.0f));

                Dust dust = Main.dust[d];



                dust.velocity = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(0.12f, 0.45f));

                dust.noGravity = false;

                dust.fadeIn = 0.9f;

                dust.scale = Main.rand.NextFloat(0.6f, 0.95f);

                dust.color = new Color(255, 235, 120);

                dust.rotation = Main.rand.NextFloat(MathHelper.ToRadians(-30f), MathHelper.ToRadians(30f));

            }

        }



        public override bool IsArmorSet(Item head, Item body, Item legs)

        {

            return body.type == ModContent.ItemType<FloweringChlorophyteBreastplate>() &&

                   legs.type == ModContent.ItemType<FloweringChlorophyteLeggings>();

        }



        public override void UpdateArmorSet(Player player)

        {



            player.setBonus = "Increases Endless Thrower damage by 25%\n" +

                              "Restores life when you lose it\n" +

                              "Dodge chance increases as health decreases\n" +

                              "Increases Endless Thrower attack speed by 19%\n" +

                              "Creates a cloud of explosive spores around you every second";

            

            player.GetDamage<EndlessThrower>() += 0.25f;

            player.GetAttackSpeed<EndlessThrower>() += 0.19f;



            player.GetModPlayer<FloweringChlorophytePlayer>().chlorophyteSetBonusActive = true;

        }



        public override void ArmorSetShadows(Player player)

        {



            player.armorEffectDrawShadowSubtle = true;

        }





        public override void ModifyTooltips(List<TooltipLine> tooltips)

        {

            tooltips.Add(new TooltipLine(Mod, "AniseForestHelmetDesc", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.FloweringChlorophyteHelmet.AniseForestHelmetDesc")));

        }



        public override void AddRecipes()

        {

            CreateRecipe()

                .AddIngredient(ItemID.ChlorophyteBar, 15)

                .AddIngredient(ModContent.ItemType<FloweringChlorophyteBar>(), 19)

                .AddTile(TileID.MythrilAnvil)

                .Register();

        }

    }





    public class FloweringChlorophytePlayer : ModPlayer

    {

        public bool chlorophyteSetBonusActive;

        private int healTimer; 

        private int sporeSpawnTimer;



        public override void ResetEffects()

        {

            chlorophyteSetBonusActive = false;

        }



        public override void PostUpdateMiscEffects()

        {

            if (chlorophyteSetBonusActive)

            {



                healTimer++;

                if (healTimer >= 30) 

                {

                    if (Player.statLife < Player.statLifeMax2)

                    {

                        Player.statLife += 7;

                        Player.HealEffect(7); 

                    }

                    healTimer = 0;

                }





                sporeSpawnTimer++;

                if (sporeSpawnTimer >= 120)

                {

                    sporeSpawnTimer = 0;





                    if (Player.whoAmI == Main.myPlayer)

                    {



                        int count = Main.rand.Next(1, 1); 



                        for (int i = 0; i < count; i++)

                        {







                            float offsetX = Main.rand.NextFloat(-95, 95);

                            float offsetY = Main.rand.NextFloat(-55, 55);

                            

                            Vector2 spawnPos = Player.Center + new Vector2(offsetX, offsetY);

                            



                            Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));





                            int damage = 40; 



                            Projectile.NewProjectile(

                                Player.GetSource_FromThis(), 

                                spawnPos, 

                                velocity, 

                                ModContent.ProjectileType<BloomingSpores>(),

                                damage, 

                                0.5f, 

                                Player.whoAmI

                            );

                        }

                    }

                }

            }

            else

            {

                healTimer = 0;

                sporeSpawnTimer = 0;

            }

        }



        public override bool FreeDodge(Player.HurtInfo info)

        {

            if (chlorophyteSetBonusActive)

            {

                float missingHealthPct = 1f - (float)Player.statLife / Player.statLifeMax2;

                int stacks = (int)(missingHealthPct / 0.15f);



                if (stacks > 0)

                {

                    float dodgeChance = stacks * 0.06f; 

                    if (dodgeChance > 0.4f) dodgeChance = 0.4f;



                    if (Main.rand.NextFloat() < dodgeChance)

                    {

                        Player.SetImmuneTimeForAllTypes(Player.longInvince ? 80 : 40);

                        return true; 

                    }

                }

            }

            return false;

        }

    }

}


