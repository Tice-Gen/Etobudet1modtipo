using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace Etobudet1modtipo.Projectiles
{
    public class PoisonKnifeProj : ModProjectile
    {
        private const int TotalFrames = 28;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = TotalFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
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
                    
                    if (Projectile.velocity != Projectile.oldVelocity)
                        Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }


            float baseRotation = Projectile.velocity.ToRotation();
            Projectile.rotation = baseRotation;
            

            float distanceFromPlayer = 5f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.Center + direction * distanceFromPlayer;
            

            Projectile.frame++;
            if (Projectile.frame >= TotalFrames)
                Projectile.frame = 0;
            

            Vector2 mouseDirection = Main.MouseWorld - player.Center;
            
            if (mouseDirection.X >= 0)
            {
                Projectile.spriteDirection = 1;
                player.ChangeDir(1);
            }
            else
            {
                Projectile.spriteDirection = -1;
                player.ChangeDir(-1);
            }
            

            Projectile.ai[1] = mouseDirection.Y < 0 ? 1 : -1;
            

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();
            

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position, 
                    Projectile.width, 
                    Projectile.height, 
                    DustID.TerraBlade,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    default,
                    0.9f
                );
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
            
            if (Projectile.ai[0] == 0 || Projectile.ai[0] % 16 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { 
                    Volume = 0.6f,
                    Pitch = Main.rand.NextFloat(-0.1f, 0.1f)
                }, Projectile.Center);
            }
            
            Projectile.ai[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 lineStart = player.MountedCenter;
            Vector2 lineEnd = lineStart + Projectile.velocity.SafeNormalize(Vector2.Zero) * 75f;
            
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), 
                targetHitbox.Size(), 
                lineStart, 
                lineEnd, 
                32f, 
                ref collisionPoint
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(BuffID.Poisoned, 300); 



            if (Main.rand.NextBool(10)) 
            {

                int numProjectiles = Main.rand.Next(2, 5); 
                int projectileType = ModContent.ProjectileType<AnisePoisonGas>();
                float speed = 1f;
                int damage = Projectile.damage / 1;

                for (int i = 0; i < numProjectiles; i++)
                {

                    float rotation = MathHelper.ToRadians(Main.rand.Next(360));
                    

                    Vector2 velocity = rotation.ToRotationVector2() * speed;


                    Projectile.NewProjectile(
                        Projectile.GetSource_OnHit(target),
                        target.Center,
                        velocity,
                        projectileType,
                        damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }


            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(
                    target.position, 
                    target.width, 
                    target.height, 
                    DustID.Electric,
                    hit.HitDirection * 2f,
                    -1f,
                    0,
                    default,
                    1f
                );
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0.25f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            
            int frameHeight = texture.Height / TotalFrames;
            Rectangle sourceRect = new Rectangle(
                0, 
                Projectile.frame * frameHeight, 
                texture.Width, 
                frameHeight
            );
            
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            
            SpriteEffects effects = SpriteEffects.None;
            
            if (Projectile.spriteDirection == -1)
            {
                effects |= SpriteEffects.FlipHorizontally;
            }
            
            if (Projectile.ai[1] < 0)
            {
                effects |= SpriteEffects.FlipVertically;
            }
            

            float drawRotation = Projectile.rotation;



            if (Projectile.spriteDirection == -1)
            {
                drawRotation += MathHelper.Pi; 
            }
            
            Color glowColor = lightColor * 1.2f;
            
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                glowColor,
                drawRotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false;
        }
    }
}