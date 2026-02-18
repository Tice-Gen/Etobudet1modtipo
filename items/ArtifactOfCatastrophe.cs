using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class ArtifactOfCatastrophe : ModItem
    {
        private int debuffTimer = 0;

        public override string Texture => "Etobudet1modtipo/items/ArtifactOfCatastrophe";

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        
            player.GetModPlayer<ArtifactOfCatastrophePlayer>().NoHealingFromArtifact = true;

            player.GetDamage(DamageClass.Magic) += 0.99f;




            

            player.AddBuff(BuffID.PotionSickness, 3600);






            debuffTimer++;
            if (debuffTimer >= 6)
            {

                player.AddBuff(BuffID.Suffocation, 10);
                debuffTimer = 0;
            }


            const int totalParticles = 15;
            const int starPoints = 5;
            const float radius = 125f;
            const int particlesPerSegment = totalParticles / starPoints;
            int[] starOrder = new int[] { 0, 2, 4, 1, 3, 0 };

            float rotation = Main.GameUpdateCount * 0.035f;
            Vector2 center = player.Center;


            Vector2[] verts = new Vector2[starPoints];
            for (int i = 0; i < starPoints; i++)
            {
                float angle = i * MathHelper.TwoPi / starPoints - MathHelper.PiOver2;
                verts[i] = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius;
            }


            if (Main.netMode != NetmodeID.Server)
            {

                if (Main.GameUpdateCount % 2 == 0)
                {
                    for (int seg = 0; seg < starPoints; seg++)
                    {
                        Vector2 a = verts[starOrder[seg]];
                        Vector2 b = verts[starOrder[seg + 1]];

                        for (int k = 0; k < particlesPerSegment; k++)
                        {
                            float t = (k + 1f) / (particlesPerSegment + 1f);
                            Vector2 localPos = Vector2.Lerp(a, b, t);
                            localPos = localPos.RotatedBy(rotation);

                            Vector2 spawnPos = center + localPos;


                            Dust d = Dust.NewDustPerfect(
                                spawnPos,
                                DustID.PurpleTorch,
                                Vector2.Zero,
                                0,
                                new Color(200, 110, 255),
                                1.4f
                            );

                            if (d != null)
                            {
                                d.noGravity = true;
                                d.velocity = Vector2.Zero;
                                d.fadeIn = 0.9f;
                                d.scale = 1.25f + Main.rand.NextFloat(0f, 0.35f);

                                Lighting.AddLight(spawnPos, 0.55f, 0.12f, 0.7f);
                            }
                        }
                    }
                }
            }

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ManaCrystal, 5)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddIngredient(ItemID.SoulofFright, 20)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddTile(TileID.BewitchingTable)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            TooltipLine normalLine = new TooltipLine(Mod, "MagicBoost", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ArtifactOfCatastrophe.MagicBoost"));


            int redIndex = tooltips.FindIndex(t => t.Name == "Description");
            if (redIndex != -1)
            {

                tooltips.Insert(redIndex, normalLine);
            }
            else
            {
                tooltips.Add(normalLine);
            }


            TooltipLine redLine = new TooltipLine(Mod, "Description", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ArtifactOfCatastrophe.Description"))
            {
                OverrideColor = new Color(255, 69, 0)
            };


            if (redIndex == -1)
                tooltips.Add(redLine);
        }
    }
}

