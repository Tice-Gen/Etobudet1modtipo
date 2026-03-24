using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Common.GlobalNPCs;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{
    public class Hook : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.width = 74;
            Item.height = 66;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(silver: 80);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool immuneToPull = target.boss || target.knockBackResist <= 0f;
            if (immuneToPull)
            {
                // Bosses and knockback-immune NPCs keep plain 40 damage with no pull behavior.
                modifiers.Knockback *= 0f;
                return;
            }

            int pullDirection = target.Center.X > player.Center.X ? -1 : 1;
            modifiers.HitDirectionOverride = pullDirection;

            HookPullGlobalNPC hookState = target.GetGlobalNPC<HookPullGlobalNPC>();
            float charge = MathHelper.Clamp(hookState.GetProximityTicks(player.whoAmI) / 240f, 0f, 1f);
            float wantedDamage = MathHelper.Lerp(36f, 60f, charge);
            modifiers.FinalDamage *= wantedDamage / 40f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool immuneToPull = target.boss || target.knockBackResist <= 0f;
            if (immuneToPull)
            {
                return;
            }

            Vector2 pull = (player.Center - target.Center).SafeNormalize(Vector2.Zero);
            float pullStrength = 3.2f * MathHelper.Clamp(target.knockBackResist, 0.2f, 1f);
            target.velocity += pull * pullStrength;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(t => t.Name == "Knockback");
            tooltips.Add(new TooltipLine(Mod, "HookPullInfo", "Medium pull"));
        }
    }
}
