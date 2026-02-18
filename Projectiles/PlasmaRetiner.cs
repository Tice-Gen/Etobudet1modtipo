using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Etobudet1modtipo.Projectiles; 

namespace Etobudet1modtipo.Projectiles
{
    public class PlasmaRetiner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {

            Projectile.width = 29;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float velocitySpeed = 15f; 

            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    Vector2 toMouse = Main.MouseWorld - player.Center;
                    toMouse.Normalize();

                    if (toMouse.HasNaNs())
                        toMouse = Vector2.UnitX * player.direction;

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toMouse * velocitySpeed, 0.15f);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }

            Vector2 toCursor = Main.MouseWorld - player.Center;
            Vector2 dir = toCursor.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.Center + dir * 10f + new Vector2(0f, 4f);
            Projectile.rotation = toCursor.ToRotation();
            Projectile.spriteDirection = toCursor.X >= 0 ? 1 : -1;
            player.ChangeDir(Projectile.spriteDirection);
            
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();

            if (Projectile.ai[0] % 16 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.6f }, Projectile.Center);
            }
            Projectile.ai[0]++;


            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.ai[1]++;

                int cycleTimer = (int)Projectile.ai[1] % 10;
                int projTypeToShoot = 0;

                if (cycleTimer == 0)
                {
                    projTypeToShoot = ModContent.ProjectileType<SolarBlade>();
                }
                else if (cycleTimer == 5) 
                {
                    projTypeToShoot = ModContent.ProjectileType<SolarBlade2>();
                }

                if (projTypeToShoot != 0)
                {
                    float baseSpeed = 8f;
                    float speedMultiplier = 2.0f; 

                    Vector2 shootVel = Projectile.velocity;
                    if (shootVel == Vector2.Zero)
                        shootVel = toCursor.SafeNormalize(Vector2.UnitY);
                    else
                        shootVel = shootVel.SafeNormalize(Vector2.Zero);


                    shootVel = shootVel.RotatedByRandom(MathHelper.ToRadians(9));
                    shootVel *= baseSpeed * speedMultiplier;


                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        shootVel,
                        projTypeToShoot,
                        Projectile.damage, 
                        Projectile.knockBack, 
                        Projectile.owner
                    );
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 start = player.MountedCenter;
            Vector2 end = start + Projectile.velocity.SafeNormalize(Vector2.Zero) * 75f;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 26f, ref point);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.Knockback *= 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle source = texture.Frame();
            Vector2 origin = source.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, source, lightColor, drawRotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}