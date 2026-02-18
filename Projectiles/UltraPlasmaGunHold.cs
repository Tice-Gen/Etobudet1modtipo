using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;

namespace Etobudet1modtipo.Projectiles
{
    public class UltraPlasmaGunHold : ModProjectile
    {
        private const int TotalFrames = 1;
        

        public float ShootTimer {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

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
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            

            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }


            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel)
                {
                    Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
                    if (toMouse == Vector2.Zero) toMouse = Vector2.UnitX * player.direction;
                    toMouse.Normalize();
                    
                    Projectile.velocity = toMouse;
                    Projectile.netUpdate = true;


                    ShootTimer++;
                    if (ShootTimer >= 3f)
                    {
                        ShootTimer = 0f;


                        Vector2 muzzleOffset = Vector2.Normalize(Projectile.velocity) * 20f;
                        Vector2 shootPos = Projectile.Center + muzzleOffset;


                        Vector2 shootVel = Projectile.velocity * 12f;


                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            shootPos,
                            shootVel,
                            ProjectileID.CrystalLeafShot,
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner
                        );


                        SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.5f, Pitch = 0.2f }, Projectile.Center);
                    }
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }


            Projectile.Center = player.MountedCenter + Projectile.velocity * 10f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            

            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            player.ChangeDir(Projectile.spriteDirection);
            
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();


            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + Projectile.velocity * 18f, 0, 0, DustID.TerraBlade);
                d.noGravity = true;
                d.scale = 0.8f;
                d.velocity = Projectile.velocity * 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null, 
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false;
        }
    }
}