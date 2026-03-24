using System.IO;
using Etobudet1modtipo.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Projectiles
{
    public class AstraniseShard : ModProjectile
    {
        private enum ShardPersonality : byte
        {
            Normal = 0,
            HighHpHunter = 1,
            WeakestHunter = 2,
            HeavyHitter = 3,
            ArmorBreaker = 4
        }

        private const float HomingSpeed = 12f;
        private const float HomingInertia = 22f;
        private const float CatchUpSpeedMultiplier = 2f;
        private const int CatchUpDelay = 120;
        private const float DashSpeed = 21f;
        private const int DashDuration = 10;
        private const int MaxDashes = 9;
        private const int NoTargetLifetime = 300;
        private const float ExplosionRadius = 76f;

        private ShardPersonality personality = ShardPersonality.Normal;
        private int noTargetTimer;
        private int trackedTarget = -1;
        private int catchUpTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.DamageType = ModContent.GetInstance<EndlessThrower>();

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void OnSpawn(IEntitySource source)
        {
            personality = (ShardPersonality)Main.rand.Next(5);
            noTargetTimer = 0;
            trackedTarget = -1;
            catchUpTimer = 0;

            if (personality == ShardPersonality.WeakestHunter)
            {
                Projectile.localNPCHitCooldown = 4;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)personality);
            writer.Write((short)trackedTarget);
            writer.Write((short)System.Math.Clamp(catchUpTimer, 0, short.MaxValue));
            writer.Write((short)System.Math.Clamp(noTargetTimer, 0, short.MaxValue));
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            personality = (ShardPersonality)reader.ReadByte();
            trackedTarget = reader.ReadInt16();
            catchUpTimer = reader.ReadInt16();
            noTargetTimer = reader.ReadInt16();
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] >= MaxDashes)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[0] == 1f)
            {
                UpdateDashState();
            }
            else
            {
                UpdateHomingState();
            }

            if (Projectile.velocity.LengthSquared() > 0.001f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation += 0.32f;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 trailVelocity = -Projectile.velocity * 0.08f + Main.rand.NextVector2Circular(0.7f, 0.7f);
                Dust trailDust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, trailVelocity, 90, default, Main.rand.NextFloat(0.85f, 1.2f));
                trailDust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.12f);
        }

        private void UpdateDashState()
        {
            int dashDuration = personality == ShardPersonality.WeakestHunter ? 6 : DashDuration;

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= dashDuration)
            {
                Projectile.ai[0] = 0f;
                Projectile.localAI[1] = 0f;
                Projectile.velocity *= 0.5f;
                Projectile.netUpdate = true;
            }
        }

        private void UpdateHomingState()
        {
            int targetIndex = FindTargetByPersonality();
            if (targetIndex == -1)
            {
                trackedTarget = -1;
                catchUpTimer = 0;

                noTargetTimer++;
                Projectile.velocity *= 0.96f;
                if (noTargetTimer >= NoTargetLifetime)
                {
                    Projectile.Kill();
                }
                return;
            }

            noTargetTimer = 0;

            if (trackedTarget != targetIndex)
            {
                trackedTarget = targetIndex;
                catchUpTimer = 0;
            }
            else
            {
                catchUpTimer++;
            }

            float homingSpeed = HomingSpeed;
            if (personality == ShardPersonality.WeakestHunter)
            {
                homingSpeed *= 1.2f;
            }

            if (catchUpTimer >= CatchUpDelay)
            {
                homingSpeed *= CatchUpSpeedMultiplier;
            }

            float homingInertia = personality == ShardPersonality.WeakestHunter ? 14f : HomingInertia;

            NPC target = Main.npc[targetIndex];
            Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Vector2 desiredVelocity = direction * homingSpeed;
            Projectile.velocity = (Projectile.velocity * (homingInertia - 1f) + desiredVelocity) / homingInertia;
        }

        private int FindTargetByPersonality()
        {
            int found = -1;

            if (personality == ShardPersonality.Normal)
            {
                float bestDistanceSq = float.MaxValue;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.CanBeChasedBy(this))
                    {
                        continue;
                    }

                    float distanceSq = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                    if (distanceSq < bestDistanceSq)
                    {
                        bestDistanceSq = distanceSq;
                        found = i;
                    }
                }

                return found;
            }

            float bestScore = float.MinValue;
            float bestDistanceForTie = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(this))
                {
                    continue;
                }

                float score = GetTargetScore(npc);
                float distanceSq = Vector2.DistanceSquared(Projectile.Center, npc.Center);

                if (score > bestScore || (score == bestScore && distanceSq < bestDistanceForTie))
                {
                    bestScore = score;
                    bestDistanceForTie = distanceSq;
                    found = i;
                }
            }

            return found;
        }

        private float GetTargetScore(NPC npc)
        {
            switch (personality)
            {
                case ShardPersonality.HighHpHunter:
                    return npc.lifeMax;
                case ShardPersonality.WeakestHunter:
                    return -npc.life;
                case ShardPersonality.HeavyHitter:
                    return npc.damage;
                case ShardPersonality.ArmorBreaker:
                    return npc.defense;
                default:
                    return 0f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (personality == ShardPersonality.HeavyHitter)
            {
                modifiers.FinalDamage *= 1.35f;
            }
            else if (personality == ShardPersonality.ArmorBreaker)
            {
                modifiers.ScalingArmorPenetration += 1f;

                if (target.takenDamageMultiplier > 0f && target.takenDamageMultiplier < 1f)
                {
                    float currentMultiplier = target.takenDamageMultiplier;
                    float targetMultiplier = 1f;
                    modifiers.FinalDamage *= targetMultiplier / currentMultiplier;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);

            trackedTarget = -1;
            catchUpTimer = 0;

            if (personality == ShardPersonality.HighHpHunter && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int percentDamage = (int)(target.lifeMax * 0.01f);
                if (percentDamage > 0 && !target.dontTakeDamage && !target.immortal)
                {
                    target.StrikeNPC(new NPC.HitInfo
                    {
                        Damage = percentDamage,
                        Knockback = 0f,
                        HitDirection = target.direction,
                        Crit = false
                    });
                }
            }

            if (personality == ShardPersonality.HeavyHitter)
            {
                target.GetGlobalNPC<AstraniseShardGlobalNPC>().damageWeakenTimer = 180;
            }

            if (Projectile.localAI[0] < MaxDashes)
            {
                float dashSpeed = personality == ShardPersonality.WeakestHunter ? DashSpeed * 1.25f : DashSpeed;
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity.SafeNormalize(Vector2.UnitX));
                Projectile.ai[0] = 1f;
                Projectile.localAI[1] = 0f;
                Projectile.localAI[0]++;
                Projectile.velocity = direction * dashSpeed;
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            if (Projectile.localAI[0] >= MaxDashes)
            {
                Projectile.Kill();
                return;
            }

            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4.2f, 4.2f);
                Dust flame = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, velocity, 40, default, Main.rand.NextFloat(1.2f, 1.85f));
                flame.noGravity = true;

                Vector2 smokeVel = Main.rand.NextVector2Circular(2.8f, 2.8f);
                Dust smoke = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, smokeVel, 130, default, Main.rand.NextFloat(0.9f, 1.3f));
                smoke.noGravity = Main.rand.NextBool(4);
            }

            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                {
                    continue;
                }

                if (npc.Distance(Projectile.Center) <= ExplosionRadius)
                {
                    npc.SimpleStrikeNPC((int)(Projectile.damage * 0.8f), Projectile.direction, false, Projectile.knockBack);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldDrawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float progress = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(255, 190, 90, 0) * (0.45f * progress);
                float trailScale = Projectile.scale * (0.82f + 0.25f * progress);

                Main.EntitySpriteDraw(texture, oldDrawPos, frame, trailColor, Projectile.oldRot[i], origin, trailScale, SpriteEffects.None, 0);
            }

            float pulse = 0.8f + 0.2f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 10f + Projectile.whoAmI);
            Color glowColor = new Color(255, 210, 100, 0);
            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f + Main.GlobalTimeWrappedHourly * 2.4f;
                Vector2 offset = angle.ToRotationVector2() * (1.8f * pulse);
                Main.EntitySpriteDraw(texture, drawPos + offset, frame, glowColor * 0.3f, Projectile.rotation, origin, Projectile.scale * (1.03f + 0.05f * pulse), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class AstraniseShardGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int damageWeakenTimer;

        public override void ResetEffects(NPC npc)
        {
            if (damageWeakenTimer > 0)
            {
                damageWeakenTimer--;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (damageWeakenTimer > 0)
            {
                modifiers.FinalDamage *= 0.75f;
            }
        }
    }
}
