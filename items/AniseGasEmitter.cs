using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.items
{

    public class AniseGasEmitter : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.damage = 31;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 2);
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AniseEmitterPlayer>().hasAniseEmitter = true;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Tooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.AniseGasEmitter.Tooltip")));
        }
    }


    public class AniseEmitterPlayer : ModPlayer
    {
        public bool hasAniseEmitter;
        private int emitterTimer;

        public override void ResetEffects()
        {
            hasAniseEmitter = false;
        }

        public override void PostUpdateEquips()
        {
            if (hasAniseEmitter)
            {
                emitterTimer++;


                if (emitterTimer >= 120)
                {
                    SpawnGasRing();
                    emitterTimer = 0;
                }
            }
            else
            {
                emitterTimer = 0;
            }
        }

        private void SpawnGasRing()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                int projectileType = ModContent.ProjectileType<AnisePoisonGas>();
                int damage = 31;
                float knockback = 1f;
                

                float baseSpeed = 3f; 


                Vector2[] dirs = new Vector2[8];
                dirs[0] = new Vector2(0f, -1f);
                dirs[1] = new Vector2(0f, 1f);
                dirs[2] = new Vector2(1f, 0f);
                dirs[3] = new Vector2(-1f, 0f);
                dirs[4] = new Vector2(1f, -1f);
                dirs[5] = new Vector2(1f, 1f);
                dirs[6] = new Vector2(-1f, -1f);
                dirs[7] = new Vector2(-1f, 1f);

                for (int i = 0; i < dirs.Length; i++)
                {
                    Vector2 dir = dirs[i];
                    if (dir != Vector2.Zero)
                        dir.Normalize();


                    float angleOffset = MathHelper.ToRadians(Main.rand.NextFloat(-10f, 10f));
                    dir = dir.RotatedBy(angleOffset);


                    float speed = baseSpeed + Main.rand.NextFloat(-2f, 2f);
                    if (speed < 0.2f) speed = 0.2f;

                    Vector2 velocity = dir * speed;


                    int projIndex = Projectile.NewProjectile(
                        Player.GetSource_Accessory(new Item(ModContent.ItemType<AniseGasEmitter>())),
                        Player.Center,
                        velocity,
                        projectileType,
                        damage,
                        knockback,
                        Main.myPlayer
                    );


                    Main.projectile[projIndex].CritChance = -9999;
                }
            }
        }
    }
}