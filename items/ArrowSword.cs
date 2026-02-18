using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Etobudet1modtipo.items
{
    public class ArrowSword : ModItem
    {



        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;


            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {

            return false;
        }




        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArrowSwordPlayer modPlayer = player.GetModPlayer<ArrowSwordPlayer>();


            modPlayer.pending = true;
            modPlayer.queuedOwner = player.whoAmI;
            modPlayer.queuedItemType = Item.type;
            modPlayer.queuedType = type;
            modPlayer.queuedDamage = damage;
            modPlayer.queuedKnockback = knockback;
            modPlayer.queuedBaseSpeed = velocity.Length();


            modPlayer.queuedMouseWorld = Main.MouseWorld;


            return false;
        }
    }


    public class ArrowSwordPlayer : ModPlayer
    {

        public bool pending = false;


        public int queuedOwner;
        public int queuedItemType;
        public int queuedType;
        public int queuedDamage;
        public float queuedKnockback;
        public float queuedBaseSpeed;


        public Vector2 queuedMouseWorld = Vector2.Zero;


        public override void ResetEffects()
        {


        }



        public override void PostUpdate()
        {
            if (!pending) return;


            if (queuedOwner < 0 || queuedOwner >= Main.maxPlayers)
            {
                pending = false;
                return;
            }

            Player player = Main.player[queuedOwner];


            if (player == null || !player.active || player.dead)
            {
                pending = false;
                return;
            }



            if (player.HeldItem == null || player.HeldItem.type != queuedItemType)
            {
                pending = false;
                return;
            }


            const int spawnThreshold = 5;

            if (player.itemAnimation > spawnThreshold)
            {

                return;
            }



            if (Main.myPlayer != queuedOwner)
            {


                pending = false;
                return;
            }


            IEntitySource spawnSource = player.GetSource_ItemUse(player.HeldItem);



            Vector2 spawnOffset = new Vector2(20f * player.direction, -8f);
            Vector2 spawnPos = player.MountedCenter + spawnOffset;


            Vector2 aim = queuedMouseWorld - spawnPos;
            if (aim.LengthSquared() < 0.0001f)
            {
                aim = new Vector2(player.direction, 0f);
            }
            aim.Normalize();

            int count;
            float spread;
            float speedMultiplier;

            if (queuedItemType == ModContent.ItemType<MetalArrowSword>())
            {

                count = Main.rand.Next(4, 8);
                spread = MathHelper.ToRadians(12f);
                speedMultiplier = 1.15f;
            }
            else
            {

                count = Main.rand.Next(2, 5);
                spread = MathHelper.ToRadians(8f);
                speedMultiplier = 1.25f;
            }

            for (int i = 0; i < count; i++)
            {

                float rot = Main.rand.NextFloat(-spread, spread);


                float speedVar = 1f + (Main.rand.NextFloat() * 0.08f - 0.04f);
                float finalSpeed = queuedBaseSpeed * speedMultiplier * speedVar;

                Vector2 velocity = aim.RotatedBy(rot) * finalSpeed;


                Projectile.NewProjectile(
                    spawnSource,
                    spawnPos,
                    velocity,
                    queuedType,
                    queuedDamage,
                    queuedKnockback,
                    queuedOwner
                );
            }


            pending = false;
        }
    }
}
