using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.Projectiles
{
    public class TitaniumYoyoProj : ModProjectile
    {
        private const int SharpCount = 4;
        internal int ShardOverdriveTimer;
        private bool spawnedShards;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 500;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (ShardOverdriveTimer > 0)
                ShardOverdriveTimer--;

            if (spawnedShards)
                return;


            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            spawnedShards = true;

            int sharpDamage = Projectile.damage / 2;
            if (sharpDamage < 1)
                sharpDamage = 1;

            for (int i = 0; i < SharpCount; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<TitaniumShard>(),
                    sharpDamage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI,
                    i
                );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShardOverdriveTimer = 30;

            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/MetalHit_Omega")
            {
                Volume = 0.9f,
                Pitch = Main.rand.NextFloat(-0.01f, 0.01f),
                PitchVariance = 0f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);

            Vector2 center = Projectile.Center;
            for (int i = 0; i < 18; i++)
            {
                Dust.NewDust(center - new Vector2(8, 8), 16, 16, DustID.Titanium);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.lifeMax > 0)
            {
                float missingPercent = 1f - (float)target.life / target.lifeMax;
                if (missingPercent < 0f)
                    missingPercent = 0f;

                int lostPercent = (int)(missingPercent * 100f);
                int bonusDamage = lostPercent * 5;
                modifiers.FlatBonusDamage += bonusDamage;
            }


            modifiers.ScalingArmorPenetration += 0.90f;


            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                modifiers.FinalDamage *= 1f / target.takenDamageMultiplier;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                    continue;

                if (p.owner == Projectile.owner &&
                    p.type == ModContent.ProjectileType<TitaniumShard>() &&
                    (int)p.ai[0] == Projectile.whoAmI)
                {
                    p.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color color = lightColor * (0.2f * progress);
                float scale = Projectile.scale * (0.95f + 0.05f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}

