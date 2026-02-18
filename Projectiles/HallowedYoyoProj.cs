using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Players;

namespace Etobudet1modtipo.Projectiles
{
    public class HallowedYoyoProj : ModProjectile
    {
        private static readonly SoundStyle YoyoHitSound = new("Etobudet1modtipo/Sounds/HallowedHit")
        {
            Volume = 1f,
            PitchVariance = 0.08f
        };

        private static readonly SoundStyle AuraHitSound = new("Etobudet1modtipo/Sounds/AuraHit")
        {
            Volume = 0.9f,
            PitchVariance = 0.1f
        };
        private static readonly SlotId[] ActiveAuraSoundByOwner = new SlotId[Main.maxPlayers];

        private int auraTimer;
        private int enragedAuraTimer;
        private int overdriveAuraTimer;

        private const float AuraRadius = 130f;
        private const float LowLifeAuraScale = 1.5f;
        private const int AuraTickRate = 12;
        private const int AuraTickRateEnraged = 6;
        private const int AuraRegenDuration = 60;
        private const int EnragedDuration = 60;
        private const int OverdriveDuration = 300;
        private const int OverdriveMultiplier = 5;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 20f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 600f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 25f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            auraTimer++;
            if (enragedAuraTimer > 0)
                enragedAuraTimer--;

            if (overdriveAuraTimer > 0)
                overdriveAuraTimer--;

            int ownerIndex = Projectile.owner;
            if (ownerIndex < 0 || ownerIndex >= Main.maxPlayers)
                return;

            Player owner = Main.player[ownerIndex];
            bool lowLifeState = owner.statLife <= owner.statLifeMax2 * 0.15f;
            float auraScale = lowLifeState ? LowLifeAuraScale : 1f;

            ApplyOwnerAuraContactEffects(owner, auraScale);
            HealFriendlyNpcsAndTownPets(auraScale);
            SpawnAuraVisualDust(auraScale);
            TryAuraDamageAndRegen(owner, lowLifeState, auraScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            enragedAuraTimer = EnragedDuration;
            SoundEngine.PlaySound(YoyoHitSound, target.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.boss)
                modifiers.FlatBonusDamage += 50f;
        }

        private void SpawnAuraVisualDust(float auraScale)
        {
            const int particlesPerTick = 8;
            float auraRadius = AuraRadius * auraScale;

            for (int i = 0; i < particlesPerTick; i++)
            {
                float angle = (auraTimer * 0.1f) + MathHelper.TwoPi * i / particlesPerTick;
                float radiusJitter = Main.rand.NextFloat(-8f, 8f);
                Vector2 pos = Projectile.Center + angle.ToRotationVector2() * (auraRadius + radiusJitter);
                Color auraColor = enragedAuraTimer > 0 ? new Color(255, 70, 70) : default;
                Dust d = Dust.NewDustPerfect(pos, 57, Vector2.Zero, 120, auraColor, 1.1f);
                d.noGravity = true;
                d.velocity = (Projectile.Center - pos) * 0.03f;
            }

            if (enragedAuraTimer > 0)
                Lighting.AddLight(Projectile.Center, 0.8f, 0.1f, 0.1f);
        }

        private void TryAuraDamageAndRegen(Player owner, bool lowLifeState, float auraScale)
        {
            int baseTickRate = enragedAuraTimer > 0 ? AuraTickRateEnraged : AuraTickRate;
            int currentTickRate = overdriveAuraTimer > 0 ? System.Math.Max(1, baseTickRate / OverdriveMultiplier) : baseTickRate;
            if (Main.myPlayer != Projectile.owner || auraTimer % currentTickRate != 0)
                return;

            float auraRadius = AuraRadius * auraScale;
            int auraDamageBase = System.Math.Max(1, Projectile.damage / 3);
            if (owner.statLife <= owner.statLifeMax2 * 0.45f)
                auraDamageBase += 15;

            if (IsAuraTouchingHitbox(owner.Hitbox, auraRadius))
                auraDamageBase = System.Math.Max(1, auraDamageBase - 5);

            bool auraHitOccurred = false;
            int enemiesHit = 0;
            int healFromLowLifeHits = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.life <= 0)
                    continue;

                if (npc.Distance(Projectile.Center) > auraRadius)
                    continue;

                int targetDamage = CalculateAuraDamageAgainstTarget(auraDamageBase, npc);
                int hitDir = npc.Center.X < owner.Center.X ? -1 : 1;
                ApplyAuraStrikeIgnoringDefenseAndDR(npc, targetDamage, hitDir);

                auraHitOccurred = true;
                enemiesHit++;

                owner.GetModPlayer<HallowedYoyoPlayer>().ActivateAuraRegen(AuraRegenDuration);
                SpawnHealFlowDustToPlayer(npc, owner);

                if (lowLifeState)
                    healFromLowLifeHits++;
            }

            if (lowLifeState && healFromLowLifeHits > 0)
                HealPlayer(owner, healFromLowLifeHits);

            if (enemiesHit > 10)
                overdriveAuraTimer = OverdriveDuration;

            if (auraHitOccurred && !SoundEngine.TryGetActiveSound(ActiveAuraSoundByOwner[Projectile.owner], out _))
                ActiveAuraSoundByOwner[Projectile.owner] = SoundEngine.PlaySound(AuraHitSound, Projectile.Center);
        }

        private int CalculateAuraDamageAgainstTarget(int auraDamageBase, NPC npc)
        {
            if (npc.lifeMax <= 0)
                return auraDamageBase;

            float missingLifePercent = (1f - npc.life / (float)npc.lifeMax) * 100f;
            int lostFivePercentSteps = (int)(missingLifePercent / 5f);
            float multiplier = 1f + lostFivePercentSteps * 0.01f;
            return System.Math.Max(1, (int)System.MathF.Round(auraDamageBase * multiplier));
        }

        private void ApplyOwnerAuraContactEffects(Player owner, float auraScale)
        {
            float auraRadius = AuraRadius * auraScale;
            if (IsAuraTouchingHitbox(owner.Hitbox, auraRadius))
                owner.statDefense += 5;
        }

        private void HealFriendlyNpcsAndTownPets(float auraScale)
        {
            if (auraTimer % 60 != 0)
                return;

            float auraRadius = AuraRadius * auraScale;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.friendly || npc.life <= 0 || npc.life >= npc.lifeMax)
                    continue;

                if (npc.Distance(Projectile.Center) > auraRadius)
                    continue;

                npc.life += 1;
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            }
        }

        private bool IsAuraTouchingHitbox(Rectangle hitbox, float auraRadius)
        {
            Rectangle auraRect = new Rectangle(
                (int)(Projectile.Center.X - auraRadius),
                (int)(Projectile.Center.Y - auraRadius),
                (int)(auraRadius * 2f),
                (int)(auraRadius * 2f));

            return auraRect.Intersects(hitbox);
        }

        private static void HealPlayer(Player owner, int healAmount)
        {
            if (healAmount <= 0)
                return;

            int lifeMissing = owner.statLifeMax2 - owner.statLife;
            if (lifeMissing <= 0)
                return;

            int actualHeal = System.Math.Min(healAmount, lifeMissing);
            owner.statLife += actualHeal;
            owner.HealEffect(actualHeal, true);
        }

        private static void ApplyAuraStrikeIgnoringDefenseAndDR(NPC npc, int damage, int hitDir)
        {
            int originalDefense = npc.defense;
            float originalTakenDamageMultiplier = npc.takenDamageMultiplier;

            try
            {
                npc.defense = 0;
                npc.takenDamageMultiplier = 1f;
                npc.SimpleStrikeNPC(damage, hitDir, false, 0f);
            }
            finally
            {
                npc.defense = originalDefense;
                npc.takenDamageMultiplier = originalTakenDamageMultiplier;
            }
        }

        private static void SpawnHealFlowDustToPlayer(NPC npc, Player owner)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 start = npc.Center + Main.rand.NextVector2Circular(18f, 18f);
                Vector2 toPlayer = owner.Center - start;

                if (toPlayer.LengthSquared() < 0.001f)
                    toPlayer = new Vector2(0f, -1f);

                toPlayer.Normalize();
                Vector2 vel = toPlayer * Main.rand.NextFloat(2.6f, 4.2f);

                Dust d = Dust.NewDustPerfect(start, 154, vel, 80, default, 1f);
                d.noGravity = true;
            }
        }
    }
}
