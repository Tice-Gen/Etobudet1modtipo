using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Global;
using Etobudet1modtipo.Players;

namespace Etobudet1modtipo.Projectiles
{
    public class ShroomyoProj : ModProjectile
    {
    

        private static readonly SoundStyle YoyoHitSound = new("Etobudet1modtipo/Sounds/MushroomHit")
        {
            Volume = 1f,
            PitchVariance = 0.1f
        };

        private static readonly SoundStyle AuraHitSound = new("Etobudet1modtipo/Sounds/AuraHit_Mushroom")
        {
            Volume = 0.9f,
            PitchVariance = 0.12f
        };

        private static readonly SlotId[] ActiveAuraSoundByOwner = new SlotId[Main.maxPlayers];

        private const float AuraBaseRadius = 100f;
        private const float AuraKillRadiusBonus = 10f;
        private const int AuraKillRadiusBonusDuration = 120;
        private const float AuraBaseHitInterval = 12f;
        private const int AuraVisualDustType = 45;
        private const int AuraHitBurstDustType = 56;

        private int auraTimer;
        private int auraRadiusBonusTimer;
        private bool pendingEmpowerResolved;
        private readonly float[] auraProgressByNpc = new float[Main.maxNPCs];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 430f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 18f;
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
            ResolveQueuedEmpower();
            auraTimer++;
            if (auraRadiusBonusTimer > 0)
                auraRadiusBonusTimer--;

            SpawnAuraVisualDust();
            TryAuraDamage();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(YoyoHitSound, target.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0f;
            if (target.boss)
                modifiers.SourceDamage *= 1.13f;

            ShroomyoPlayer shroomyoPlayer = Main.player[Projectile.owner].GetModPlayer<ShroomyoPlayer>();
            if (shroomyoPlayer.IsEmpoweredProjectile(Projectile))
                modifiers.SourceDamage *= ShroomyoPlayer.EmpowerDamageMultiplier;
        }

        private void SpawnAuraVisualDust()
        {
            const int particlesPerTick = 7;
            float auraRadius = GetCurrentAuraRadius();

            for (int i = 0; i < particlesPerTick; i++)
            {
                float angle = auraTimer * 0.1f + MathHelper.TwoPi * i / particlesPerTick;
                float jitter = Main.rand.NextFloat(-6f, 6f);
                Vector2 pos = Projectile.Center + angle.ToRotationVector2() * (auraRadius + jitter);

                Dust d = Dust.NewDustPerfect(pos, AuraVisualDustType, Vector2.Zero, 120, default, 1f);
                d.noGravity = true;
                d.velocity = (Projectile.Center - pos) * 0.02f;
            }
        }

        private void TryAuraDamage()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            bool auraHitOccurred = false;
            float auraRadius = GetCurrentAuraRadius();

            Player owner = Main.player[Projectile.owner];
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.life <= 0 || npc.lifeMax <= 0)
                {
                    auraProgressByNpc[i] = 0f;
                    continue;
                }

                if (npc.Distance(Projectile.Center) > auraRadius)
                    continue;

                auraProgressByNpc[i] += 1f + GetAuraSpeedBonusFromCurrentLife(npc);
                if (auraProgressByNpc[i] < AuraBaseHitInterval)
                    continue;

                auraProgressByNpc[i] -= AuraBaseHitInterval;

                int auraDamage = CalculateAuraDamageAgainstTarget(npc);
                int hitDir = npc.Center.X < owner.Center.X ? -1 : 1;
                bool wasAliveBeforeAuraHit = npc.life > 0;
                ApplyAuraStrikeIgnoringDefenseAndDR(npc, auraDamage, hitDir);
                npc.GetGlobalNPC<ShroomyoInfectionGlobalNPC>().AddInfection(npc);
                SpawnAuraHitBurst(npc);
                auraHitOccurred = true;

                if (wasAliveBeforeAuraHit && (!npc.active || npc.life <= 0))
                    auraRadiusBonusTimer = AuraKillRadiusBonusDuration;
            }

            if (auraHitOccurred && !SoundEngine.TryGetActiveSound(ActiveAuraSoundByOwner[Projectile.owner], out _))
                ActiveAuraSoundByOwner[Projectile.owner] = SoundEngine.PlaySound(AuraHitSound, Projectile.Center);
        }

        private float GetCurrentAuraRadius()
        {
            return AuraBaseRadius + (auraRadiusBonusTimer > 0 ? AuraKillRadiusBonus : 0f);
        }

        private static float GetAuraSpeedBonusFromCurrentLife(NPC npc)
        {
            float lifeRatio = npc.life / (float)npc.lifeMax;
            if (lifeRatio <= 0.25f)
                return 0.15f;

            if (lifeRatio <= 0.50f)
                return 0.10f;

            if (lifeRatio <= 0.75f)
                return 0.05f;

            return 0f;
        }

        private static void SpawnAuraHitBurst(NPC npc)
        {
            for (int k = 0; k < 8; k++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2.8f, 2.8f);
                Dust d = Dust.NewDustPerfect(npc.Center, AuraHitBurstDustType, velocity, 80, default, 1.15f);
                d.noGravity = true;
            }
        }

        private int CalculateAuraDamageAgainstTarget(NPC npc)
        {
            int baseDamage = System.Math.Max(1, Projectile.damage / 4);
            float multiplier = npc.boss ? 0.93f : 1.30f;
            return System.Math.Max(1, (int)System.MathF.Round(baseDamage * multiplier));
        }

        private static void ApplyAuraStrikeIgnoringDefenseAndDR(NPC npc, int damage, int hitDir)
        {
            int originalDefense = npc.defense;
            float originalTakenDamageMultiplier = npc.takenDamageMultiplier;

            try
            {
                npc.defense = 0;
                npc.takenDamageMultiplier = 1f;
                npc.SimpleStrikeNPC(damage, hitDir, crit: false, knockBack: 0f);
            }
            finally
            {
                npc.defense = originalDefense;
                npc.takenDamageMultiplier = originalTakenDamageMultiplier;
            }
        }

        private void ResolveQueuedEmpower()
        {
            if (pendingEmpowerResolved || Main.myPlayer != Projectile.owner)
                return;

            pendingEmpowerResolved = true;
            ShroomyoPlayer shroomyoPlayer = Main.player[Projectile.owner].GetModPlayer<ShroomyoPlayer>();
            if (!shroomyoPlayer.ConsumeQueuedEmpower())
                return;

            int spentPoints = ShroomyoInfectionGlobalNPC.ConsumeOneInfectionFromAllNPCs();
            if (spentPoints <= 0)
            {
                Main.NewText("Нет зараженных целей для усиления Shroomyo.", Color.LightGray);
                return;
            }

            shroomyoPlayer.ActivateEmpower(Projectile);
            CombatText.NewText(Main.player[Projectile.owner].Hitbox, new Color(170, 255, 170), "Shroomyo усилен +15%");
            Main.NewText($"Shroomyo усилен на 15% (потрачено очков: {spentPoints})", new Color(170, 255, 170));
        }
    }
}
