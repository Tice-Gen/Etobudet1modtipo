using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class ProphecyBlade : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ProphecyBladeTooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.ProphecyBlade.ProphecyBladeTooltip")));
        }


        public override void MeleeEffects(Player player, Rectangle hitbox)
        {

            for (int i = 0; i < 3; i++) 
            {
                int dustIndex = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GoldFlame);
                
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.5f;

                dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
            }
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Muramasa, 1);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddIngredient(ItemID.CrystalShard, 15);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.50f)
            {
                int delayedDamage = damageDone * 2;
                int npcIndex = target.whoAmI;
                int timer = 60;

                ModContent.GetInstance<ProphecyBladeSystem>().AddDelayedHit(npcIndex, delayedDamage, timer, player);

                Vector2 direction = Vector2.Normalize(target.Center - player.Center) * 10f;
                Projectile.NewProjectile(
                    player.GetSource_OnHit(target),
                    player.Center,
                    direction,
                    ProjectileID.DD2PhoenixBowShot,
                    damageDone / 2,
                    3f,
                    player.whoAmI
                );
            }
        }
    }

    public class ProphecyBladeSystem : ModSystem
    {
        private struct DelayedHit
        {
            public int NpcIndex;
            public int Damage;
            public int Timer;
            public Player Owner;
        }

        private readonly List<DelayedHit> delayedHits = new();

        public void AddDelayedHit(int npcIndex, int damage, int timer, Player owner)
        {
            delayedHits.Add(new DelayedHit
            {
                NpcIndex = npcIndex,
                Damage = damage,
                Timer = timer,
                Owner = owner
            });
        }

        public override void PostUpdateNPCs()
        {
            for (int i = delayedHits.Count - 1; i >= 0; i--)
            {
                DelayedHit hit = delayedHits[i];
                hit.Timer--;

                if (hit.Timer <= 0)
                {
                    if (hit.NpcIndex >= 0 && hit.NpcIndex < Main.npc.Length)
                    {
                        NPC npc = Main.npc[hit.NpcIndex];
                        if (npc.active && !npc.friendly && npc.life > 0)
                        {
                            int finalDamage = hit.Damage;
                            npc.life -= finalDamage;
                            CombatText.NewText(npc.Hitbox, Color.MediumPurple, finalDamage);

                            if (npc.life <= 0)
                            {
                                npc.checkDead();
                                npc.HitEffect();
                                npc.active = false;
                            }
                        }
                    }
                    delayedHits.RemoveAt(i);
                }
                else
                {
                    delayedHits[i] = hit;
                }
            }
        }
    }
}