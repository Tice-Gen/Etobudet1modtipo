using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Etobudet1modtipo.items
{
    public class BerserkAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;

            Item.accessory = true;

            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Red;
        }


        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            int index = player.FindBuffIndex(BuffID.PotionSickness);

            if (index != -1 && player.buffTime[index] > 600)
                return false;

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BerserkPlayer>().berserkEquipped = true;


            player.AddBuff(BuffID.PotionSickness, 900);


            float hpPercent =
                (float)player.statLife / player.statLifeMax2;


            if (hpPercent >= 0.5f)
            {
                player.lifeRegen = -250;
            }
            else if (hpPercent >= 0.3f)
            {
                player.lifeRegen = -25;
            }
            else if (hpPercent >= 0.2f)
            {
                player.lifeRegen = -20;
            }
            else
            {
                player.lifeRegen = -10;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SealedCoreOfLife>())
                .AddIngredient(ModContent.ItemType<SteelOfLife>(), 5)
                .AddIngredient(ItemID.BandofRegeneration)
                .AddIngredient(ItemID.LifeCrystal, 5)
                .AddIngredient(ItemID.HealingPotion, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }




    public class BerserkPlayer : ModPlayer
    {
        public bool berserkEquipped;


        private int healCooldown;

        public override void ResetEffects()
        {
            berserkEquipped = false;
        }

        public override void PreUpdate()
        {
            if (healCooldown > 0)
                healCooldown--;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!berserkEquipped)
                return;

            if (healCooldown > 0)
                return;


            int damage = info.Damage;

            if (damage <= 0)
                return;


            int heal = damage / 2 + 10;

            if (heal <= 0)
                return;


            Player.statLife += heal;
            if (Player.statLife > Player.statLifeMax2)
                Player.statLife = Player.statLifeMax2;

            Player.HealEffect(heal);


            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Player.position,
                    Player.width,
                    Player.height,
                    183,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    0,
                    default,
                    1.2f
                );
                d.noGravity = true;
            }


            int points = 70;
            float baseScale = 3f;
            float scale = baseScale * Main.rand.NextFloat(0.5f, 1.5f);

            float rotation = MathHelper.ToRadians(
                Main.rand.NextFloat(-15f, 15f)
            );

            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);

            for (int i = 0; i < points; i++)
            {
                float t = MathHelper.TwoPi * i / points;

                float x = 16f * MathF.Pow(MathF.Sin(t), 3);
                float y =
                    13f * MathF.Cos(t)
                    - 5f * MathF.Cos(2f * t)
                    - 2f * MathF.Cos(3f * t)
                    - MathF.Cos(4f * t);

                Vector2 heartVec = new Vector2(x, -y) * scale;

                heartVec = new Vector2(
                    heartVec.X * cos - heartVec.Y * sin,
                    heartVec.X * sin + heartVec.Y * cos
                );

                Dust d = Dust.NewDustDirect(
                    Player.Center,
                    0,
                    0,
                    235,
                    0f,
                    0f,
                    0,
                    default,
                    Main.rand.NextFloat(1.4f, 2.1f)
                );

                d.noGravity = true;
                d.velocity = heartVec * 0.25f;
            }


            healCooldown = 20;
        }
    }
}
