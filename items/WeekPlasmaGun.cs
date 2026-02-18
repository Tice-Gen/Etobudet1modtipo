using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.items
{
    public class WeekPlasmaGun : ModItem
    {
        private const int frameWidth = 36;
        private const int frameHeight = 24;

        public override void SetDefaults()
        {
            Item.width = frameWidth;
            Item.height = frameHeight;
            Item.useTime = 4;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 10;
            Item.crit = 96;
            Item.mana = 2;
            Item.shoot = ProjectileID.LaserMachinegunLaser;
            Item.shootSpeed = 6f;
        }


       public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
{
    modifiers.FinalDamage.Base = 10f;
    modifiers.ArmorPenetration += target.defense * 1f;
}


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {

            float spreadDegrees = 5f;
            float spreadRadians = MathHelper.ToRadians(spreadDegrees);
            float randomAngle = Main.rand.NextFloat(-spreadRadians, spreadRadians);
            Vector2 newVelocity = velocity.RotatedBy(randomAngle);

            Vector2 muzzleOffset = Vector2.Normalize(newVelocity) * 1f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            Projectile.NewProjectile(source, position, newVelocity, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4f, 0f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle sourceRect = new Rectangle(0, 0, frameWidth, frameHeight);
            spriteBatch.Draw(texture, position, sourceRect, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle sourceRect = new Rectangle(0, 0, frameWidth, frameHeight);
            Vector2 position = Item.Center - Main.screenPosition;
            Vector2 origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
            spriteBatch.Draw(texture, position, sourceRect, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MeteoriteBar, 20);
            recipe.AddIngredient(ModContent.ItemType<ParticleOfCalamity>(), 9);
            recipe.AddTile(TileID.ShimmerMonolith);
            recipe.Register();
        }
    }
}
