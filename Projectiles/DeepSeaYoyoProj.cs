using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Etobudet1modtipo.Players;

namespace Etobudet1modtipo.Projectiles
{
    public class DeepSeaYoyoProj : ModProjectile
    {
        private const int ComponentCount = 3;
        private bool spawnedComponent;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 500f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 30f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (spawnedComponent || Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            spawnedComponent = true;

            int componentDamage = Projectile.damage / 2;
            if (componentDamage < 1)
            {
                componentDamage = 1;
            }

            for (int i = 0; i < ComponentCount; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<DeepSeaYoyoComponent>(),
                    componentDamage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI,
                    i);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Ignore 100% of target defense.
            modifiers.ScalingArmorPenetration += 1f;

            // Ignore all damage resistance multipliers below 1x.
            if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
            {
                modifiers.FinalDamage /= target.takenDamageMultiplier;
            }

            // +25% damage to bosses below 50% HP.
            if (target.boss && target.life > 0 && target.life < target.lifeMax * 0.5f)
            {
                modifiers.FinalDamage *= 1.25f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle hitSound = new SoundStyle("Etobudet1modtipo/Sounds/DeepHit2")
            {
                Volume = 1f,
                PitchVariance = 0.05f,
                MaxInstances = 20
            };
            SoundEngine.PlaySound(hitSound, Projectile.Center);

            Player owner = Main.player[Projectile.owner];
            DeepSeaYoyoPlayer deepPlayer = owner.GetModPlayer<DeepSeaYoyoPlayer>();

            if (!deepPlayer.IsDeepSeaReady)
            {
                deepPlayer.DeepSeaChargeHits++;
                return;
            }

            if (Projectile.owner != Main.myPlayer)
            {
                deepPlayer.DeepSeaChargeHits = 0;
                return;
            }

            Vector2 startCenter = target.Center;
            Vector2[] offsets =
            {
                new Vector2(-100f, -100f),
                new Vector2(100f, -100f),
                new Vector2(100f, 100f),
                new Vector2(-100f, 100f)
            };

            int[] shardTypes =
            {
                ModContent.ProjectileType<DeepShard1>(),
                ModContent.ProjectileType<DeepShard2>(),
                ModContent.ProjectileType<DeepShard3>(),
                ModContent.ProjectileType<DeepShard4>()
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2 spawnPos = startCenter + offsets[i];

                for (int d = 0; d < 18; d++)
                {
                    Vector2 burst = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.4f, 4.2f);
                    Dust dust = Dust.NewDustPerfect(spawnPos, DustID.DungeonWater, burst, 80, default, 1.25f);
                    dust.noGravity = true;
                }

                Projectile.NewProjectile(
                    Projectile.GetSource_OnHit(target),
                    spawnPos,
                    Vector2.Zero,
                    shardTypes[i],
                    10000,
                    0f,
                    owner.whoAmI,
                    target.whoAmI,
                    0f);
            }

            deepPlayer.DeepSeaChargeHits = 0;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                {
                    continue;
                }

                if (p.owner == Projectile.owner &&
                    p.type == ModContent.ProjectileType<DeepSeaYoyoComponent>() &&
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
                Color trailColor = new Color(70, 170, 255, 0) * (0.45f * progress);
                Main.EntitySpriteDraw(texture, drawPos, frame, trailColor, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Color glow = new Color(120, 210, 255, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, glow, Projectile.rotation, origin, Projectile.scale * 1.06f, SpriteEffects.None, 0);
            return true;
        }
    }
}
