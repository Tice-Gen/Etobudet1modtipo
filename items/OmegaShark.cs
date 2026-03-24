using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.items
{
    public class OmegaShark : ModItem
    {
        private const int MaxUseSpeed = 36;
        private const int MinUseSpeed = 2;
        private const int UseSpeedStep = 2;
        private const int MaxSpeedDurationTicks = 10 * 60;
        private const int OverheatLockTicks = 45;
        private const float MaxSpreadDegrees = 5f;

        private int currentUseSpeed = MaxUseSpeed;
        private int maxSpeedTimer;
        private int overheatLockTimer;

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 24;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = MaxUseSpeed;
            Item.useAnimation = MaxUseSpeed;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(gold: 18);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.ArmorPenetration = 9999;
        }

        public override bool CanUseItem(Player player)
        {
            if (overheatLockTimer > 0)
            {
                return false;
            }

            Item.useTime = currentUseSpeed;
            Item.useAnimation = currentUseSpeed;
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (overheatLockTimer > 0)
            {
                overheatLockTimer--;
            }

            bool isFiring = player.itemAnimation > 0 && player.HeldItem.type == Item.type;

            if (isFiring)
            {
                if (currentUseSpeed <= MinUseSpeed)
                {
                    maxSpeedTimer++;
                    if (maxSpeedTimer >= MaxSpeedDurationTicks)
                    {
                        TriggerOverheat(player);
                    }
                }

                return;
            }

            maxSpeedTimer = 0;
            currentUseSpeed = MaxUseSpeed;
            Item.useTime = MaxUseSpeed;
            Item.useAnimation = MaxUseSpeed;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.type != Item.type)
            {
                currentUseSpeed = MaxUseSpeed;
                maxSpeedTimer = 0;
                overheatLockTimer = 0;
                Item.useTime = MaxUseSpeed;
                Item.useAnimation = MaxUseSpeed;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            if (currentUseSpeed > MinUseSpeed)
            {
                currentUseSpeed -= UseSpeedStep;
                if (currentUseSpeed < MinUseSpeed)
                {
                    currentUseSpeed = MinUseSpeed;
                }
            }

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float speedProgress = (MaxUseSpeed - currentUseSpeed) / (float)(MaxUseSpeed - MinUseSpeed);
            speedProgress = MathHelper.Clamp(speedProgress, 0f, 1f);
            float spreadRadians = MathHelper.ToRadians(MaxSpreadDegrees * speedProgress);
            float randomAngle = Main.rand.NextFloat(-spreadRadians, spreadRadians);
            velocity = velocity.RotatedBy(randomAngle);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, 0f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "OmegaSharkTooltip", Terraria.Localization.Language.GetTextValue("Mods.Etobudet1modtipo.ItemTooltips.OmegaShark.Tooltip")));
        }

        private void TriggerOverheat(Player player)
        {
            currentUseSpeed = MaxUseSpeed;
            Item.useTime = MaxUseSpeed;
            Item.useAnimation = MaxUseSpeed;
            maxSpeedTimer = 0;
            overheatLockTimer = OverheatLockTicks;
            player.itemAnimation = 0;
            player.itemTime = 0;

            SoundEngine.PlaySound(SoundID.Item14, player.Center);

            if (!Main.dedServ)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2.6f, 2.6f);
                    Gore.NewGore(player.GetSource_ItemUse(Item), player.Center, velocity, 99);
                }
            }
        }
    }
}
