using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Rarities;
using Etobudet1modtipo.Classes;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Players;

namespace Etobudet1modtipo.items
{
    [AutoloadEquip(EquipType.Head)]
    public class DeepSeaHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 38;
            Item.rare = ModContent.RarityType<DeepnestRare>();
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<EndlessThrower>() += 0.10f;
            player.hasMagiluminescence = true;
            player.GetModPlayer<DeepSeaArmorPlayer>().deepSeaHelmetEquipped = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DeepSeaBreastplate>()
                && legs.type == ModContent.ItemType<DeepSeaLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Infinite breathing in water, honey and lava\n"
                + "+20% damage reduction\n"
                + "Melee/Ranged/Summon/Endless Thrower damage +20%\n"
                + "Melee/Ranged/Endless Thrower crit chance +40%\n"
                + "Immunity: fire, poison, bleed, silence, broken armor, suffocation, slows, web, frozen, stoned, AshDisease\n"
                + "Enables water dash with 100% dodge chance during dash";

            player.endurance += 0.20f;
            player.gills = true;
            player.ignoreWater = true;
            player.accDivingHelm = true;
            player.breath = player.breathMax;
            player.lavaImmune = true;
            player.lavaTime = player.lavaMax;

            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.Daybreak] = true;

            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Dazed] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.buffImmune[ModContent.BuffType<AshDisease>()] = true;

            player.GetDamage(DamageClass.Melee) += 0.20f;
            player.GetDamage(DamageClass.Ranged) += 0.20f;
            player.GetDamage(DamageClass.Summon) += 0.20f;
            player.GetDamage<EndlessThrower>() += 0.20f;

            player.GetCritChance(DamageClass.Melee) += 40f;
            player.GetCritChance(DamageClass.Ranged) += 40f;
            player.GetCritChance<EndlessThrower>() += 40f;

            player.GetModPlayer<DeepSeaArmorPlayer>().deepSeaSetActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeepSeaBar>(), 25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
