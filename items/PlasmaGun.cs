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
    public class PlasmaGun : ModItem
    {
        private const int frameWidth = 58;
        private const int frameHeight = 24;

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 24;
            Item.useTime = 4;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(gold: 9);
            Item.reuseDelay = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 71;
            Item.mana = 12;
            Item.shoot = ProjectileID.MagicMissile;
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
            recipe.AddIngredient(ItemID.HallowedBar, 20);
            recipe.AddIngredient(ItemID.SoulofLight, 50);
            recipe.AddIngredient(ItemID.StarCannon);
            recipe.AddIngredient(ModContent.ItemType<WeekPlasmaGun>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
