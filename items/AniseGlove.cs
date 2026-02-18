using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Rarities;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class AniseGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            


            

            Item.rare = ModContent.RarityType<AniseRarity>();

            Item.value = Item.buyPrice(silver: 80);
        }




        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {


            string desc = Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AniseGlove.AniseGloveTooltip");


            tooltips.Add(new TooltipLine(Mod, "AniseGloveTooltip", desc));
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<AniseGlovePlayer>();
            
            modPlayer.aniseGloveEquipped = true;


            player.GetAttackSpeed<EndlessThrower>() += 0.10f;
            player.GetDamage<EndlessThrower>() += 0.05f;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarAnise, 15)
                .AddIngredient(ModContent.ItemType<AniseForestBar>(), 5) 
                .AddIngredient(ModContent.ItemType<AromaticGel>(), 15)
                .Register();
        }
    }


    public class AniseGlovePlayer : ModPlayer
    {
        public bool aniseGloveEquipped;

        public override void ResetEffects()
        {
            aniseGloveEquipped = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (aniseGloveEquipped && hit.DamageType.CountsAsClass(DamageClass.Melee))
            {
                ApplyDebuff(target);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (aniseGloveEquipped && proj.DamageType.CountsAsClass(DamageClass.Melee))
            {
                ApplyDebuff(target);
            }
        }

        private void ApplyDebuff(NPC target)
        {

            target.AddBuff(ModContent.BuffType<HighlyConcentratedStrike>(), 300);
        }
    }
}