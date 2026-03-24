using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Gores;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Projectiles;

namespace Etobudet1modtipo.NPCs
{
    [AutoloadBossHead]
    public class Astraclysm : ModNPC
    {
        private const int AnimationFrames = 4;
        private const int AnimationFrameWidth = 180;
        private const int AnimationFrameHeight = 143;
        private const float AnimationCycleTicks = 30f;
        private const int HoverState = 0;
        private const int DashState = 1;
        private const int CosmalFirePattern = 0;
        private const int AstralStonePattern = 1;
        private const float HoverDistanceX = 320f;
        private const float HoverDistanceYDrift = 56f;
        private const int DashCountPerCycle = 5;

        private ref float AIState => ref NPC.ai[0];
        private ref float StateTimer => ref NPC.ai[1];
        private ref float DashCounter => ref NPC.ai[2];
        private ref float OrbitSide => ref NPC.ai[3];
        private ref float AttackPattern => ref NPC.localAI[0];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = AnimationFrames;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Retinazer);
            NPC.width = AnimationFrameWidth;
            NPC.height = AnimationFrameHeight;
            NPC.damage = 200;
            NPC.defense = 120;
            NPC.lifeMax = 660000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(0, 10, 50, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            AnimationType = -1;
            NPC.boss = true;
            NPC.netAlways = true;
            Music = MusicID.Boss2;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            Player target = Main.player[NPC.target];

            if (!target.active || target.dead)
            {
                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0f, -14f), 0.08f);
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.15f);
                return;
            }

            if (OrbitSide == 0f)
                OrbitSide = NPC.Center.X < target.Center.X ? -1f : 1f;

            StateTimer++;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phaseTwo = lifeRatio <= 0.65f;
            bool phaseThree = lifeRatio <= 0.25f;

            Lighting.AddLight(NPC.Center, 0.95f, 0.2f, 0.2f);

            if (AIState == HoverState)
            {
                if (AttackPattern == AstralStonePattern)
                    DoAstralStoneHoverAttack(target, phaseTwo, phaseThree);
                else
                    DoCosmalFireHoverAttack(target, phaseTwo, phaseThree);
            }
            else
            {
                DoDashAttack(target, phaseTwo, phaseThree);
            }

            if (AIState == DashState)
            {
                if (Math.Abs(NPC.velocity.X) > 0.15f)
                    NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
            }
            else
            {
                NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
            }

            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.18f);

            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(AIState == DashState ? 2 : 4))
            {
                Vector2 dustVelocity = -NPC.velocity * 0.15f;
                Dust dust = Dust.NewDustPerfect(
                    NPC.Center + Main.rand.NextVector2Circular(28f, 28f),
                    DustID.Blood,
                    dustVelocity,
                    120,
                    default,
                    AIState == DashState ? 1.35f : 1.05f
                );
                dust.noGravity = true;
            }
        }

        private void DoCosmalFireHoverAttack(Player target, bool phaseTwo, bool phaseThree)
        {
            float hoverDistanceX = phaseThree ? 170f : phaseTwo ? 200f : 230f;
            float hoverDrift = phaseThree ? 34f : phaseTwo ? 42f : 48f;
            float yDrift = (float)Math.Sin(StateTimer / 18f) * hoverDrift;
            Vector2 hoverTarget = target.Center + new Vector2(OrbitSide * hoverDistanceX, yDrift);

            SmoothFlyTo(hoverTarget, phaseThree ? 15f : phaseTwo ? 14f : 13f, phaseThree ? 0.07f : phaseTwo ? 0.06f : 0.052f);

            int shootStart = 1;
            int shotInterval = phaseThree ? 2 : phaseTwo ? 3 : 4;
            int hoverDuration = 600;

            if (StateTimer >= shootStart && (StateTimer - shootStart) % shotInterval == 0f)
                FireCosmalFireBurst(target, phaseTwo, phaseThree);

            if (StateTimer >= hoverDuration)
                BeginDashState();
        }

        private void DoAstralStoneHoverAttack(Player target, bool phaseTwo, bool phaseThree)
        {
            float yDrift = (float)Math.Sin(StateTimer / 22f) * HoverDistanceYDrift;
            Vector2 hoverTarget = target.Center + new Vector2(OrbitSide * HoverDistanceX, yDrift);

            SmoothFlyTo(hoverTarget, phaseThree ? 34f : phaseTwo ? 30f : 26f, phaseThree ? 0.16f : 0.13f);

            int shootStart = phaseThree ? 18 : phaseTwo ? 24 : 30;
            int volleyInterval = phaseThree ? 18 : phaseTwo ? 24 : 30;
            int hoverDuration = phaseThree ? 96 : phaseTwo ? 120 : 150;

            if (StateTimer >= shootStart && (StateTimer - shootStart) % volleyInterval == 0f)
                FireAstralStoneVolley(target, phaseTwo, phaseThree);

            if (StateTimer >= hoverDuration)
                BeginDashState();
        }

        private void DoDashAttack(Player target, bool phaseTwo, bool phaseThree)
        {
            bool astralStonePattern = AttackPattern == AstralStonePattern;
            int prepTime = astralStonePattern ? (phaseThree ? 10 : phaseTwo ? 12 : 14) : (phaseThree ? 12 : phaseTwo ? 14 : 16);
            int maxPrepTime = prepTime + (phaseThree ? 8 : phaseTwo ? 10 : 12);
            int dashTime = astralStonePattern ? (phaseThree ? 42 : phaseTwo ? 46 : 50) : (phaseThree ? 38 : phaseTwo ? 42 : 46);
            int recoverTime = astralStonePattern ? (phaseThree ? 18 : 20) : (phaseThree ? 16 : 18);
            float dashSpeed = astralStonePattern ? (phaseThree ? 42f : phaseTwo ? 38f : 34f) : (phaseThree ? 38f : phaseTwo ? 34f : 30f);
            Vector2 launchTarget = GetDashLaunchTarget(target, phaseTwo, phaseThree, astralStonePattern);

            if (StateTimer <= maxPrepTime)
            {
                SmoothFlyTo(launchTarget, phaseThree ? 24f : phaseTwo ? 22f : 20f, phaseThree ? 0.22f : 0.18f);

                float launchDistance = Vector2.Distance(NPC.Center, launchTarget);
                if (StateTimer < prepTime || (launchDistance > 72f && StateTimer < maxPrepTime))
                    return;

                Vector2 predictedTarget = target.Center + target.velocity * (phaseThree ? 12f : 9f);
                Vector2 dashDirection = (predictedTarget - NPC.Center).SafeNormalize(Vector2.UnitX * OrbitSide);
                NPC.velocity = dashDirection * dashSpeed;
                if (astralStonePattern)
                    SpawnDashAstralStones(dashDirection, dashSpeed, phaseTwo, phaseThree);
                StateTimer = maxPrepTime + 1f;
                NPC.netUpdate = true;
                return;
            }

            if (StateTimer <= maxPrepTime + dashTime)
            {
                Vector2 dashDirection = NPC.velocity.SafeNormalize(Vector2.UnitX * OrbitSide);
                NPC.velocity = dashDirection * dashSpeed;
                return;
            }

            if (StateTimer <= maxPrepTime + dashTime + recoverTime)
            {
                Vector2 recoilTarget = GetDashRecoilTarget(target, phaseThree, astralStonePattern);
                SmoothFlyTo(recoilTarget, phaseThree ? 26f : phaseTwo ? 24f : 22f, 0.22f);
                return;
            }

            if (StateTimer >= maxPrepTime + dashTime + recoverTime)
            {
                DashCounter++;
                OrbitSide *= -1f;

                if (DashCounter >= DashCountPerCycle)
                {
                    AIState = HoverState;
                    DashCounter = 0f;
                    AttackPattern = AttackPattern == AstralStonePattern ? CosmalFirePattern : AstralStonePattern;
                }

                StateTimer = 0f;
                NPC.netUpdate = true;
            }
        }

        private Vector2 GetDashLaunchTarget(Player target, bool phaseTwo, bool phaseThree, bool astralStonePattern)
        {
            float distanceX = astralStonePattern ? (phaseThree ? 320f : phaseTwo ? 360f : 400f) : (phaseThree ? 280f : phaseTwo ? 320f : 360f);
            float distanceY = astralStonePattern ? (phaseThree ? 90f : 120f) : (phaseThree ? 70f : 100f);
            float verticalDirection = DashCounter % 2f == 0f ? -1f : 1f;
            return target.Center + new Vector2(OrbitSide * distanceX, verticalDirection * distanceY);
        }

        private Vector2 GetDashRecoilTarget(Player target, bool phaseThree, bool astralStonePattern)
        {
            Vector2 fallbackDirection = new Vector2(-OrbitSide, 0f);
            Vector2 recoilDirection = (NPC.Center - target.Center).SafeNormalize(fallbackDirection);
            float recoilDistance = astralStonePattern ? (phaseThree ? 300f : 360f) : (phaseThree ? 240f : 300f);
            float verticalOffset = DashCounter % 2f == 0f ? -56f : 56f;
            return NPC.Center + recoilDirection * recoilDistance + new Vector2(0f, verticalOffset);
        }

        private void BeginDashState()
        {
            AIState = DashState;
            StateTimer = 0f;
            DashCounter = 0f;
            NPC.netUpdate = true;
        }

        private void SmoothFlyTo(Vector2 targetPosition, float maxSpeed, float lerpAmount)
        {
            Vector2 toTarget = targetPosition - NPC.Center;
            float distance = toTarget.Length();
            float speed = MathHelper.Lerp(4f, maxSpeed, MathHelper.Clamp(distance / 320f, 0f, 1f));
            Vector2 desiredVelocity = toTarget.SafeNormalize(Vector2.UnitY) * speed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, lerpAmount);
        }

        private void FireCosmalFireBurst(Player target, bool phaseTwo, bool phaseThree)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float shotSpeed = phaseThree ? 13f : phaseTwo ? 11f : 9f;
            float prediction = phaseThree ? 5f : phaseTwo ? 4f : 3f;
            float spread = phaseThree ? 0.18f : phaseTwo ? 0.14f : 0.1f;
            int damage = phaseThree ? 105 : phaseTwo ? 85 : 70;
            Vector2 aimPosition = target.Center + target.velocity * prediction;
            Vector2 baseDirection = (aimPosition - NPC.Center).SafeNormalize(Vector2.UnitX * OrbitSide);
            Vector2 shotVelocity = baseDirection.RotatedBy(Main.rand.NextFloat(-spread, spread)) * shotSpeed * Main.rand.NextFloat(0.92f, 1.08f);
            Vector2 spawnPosition = NPC.Center + shotVelocity.SafeNormalize(Vector2.UnitX * OrbitSide) * 42f;

            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                spawnPosition,
                shotVelocity,
                ModContent.ProjectileType<CosmolFire>(),
                damage,
                0f,
                Main.myPlayer
            );
        }

        private void FireAstralStoneVolley(Player target, bool phaseTwo, bool phaseThree)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float shotSpeed = phaseThree ? 28f : phaseTwo ? 24f : 20f;
            float prediction = phaseThree ? 12f : phaseTwo ? 10f : 8f;
            int damage = phaseThree ? 170 : phaseTwo ? 145 : 120;
            float spread = 0.16f;

            Vector2 aimPosition = target.Center + target.velocity * prediction;
            Vector2 baseDirection = (aimPosition - NPC.Center).SafeNormalize(Vector2.UnitX * OrbitSide);

            for (int i = -1; i <= 1; i++)
            {
                Vector2 shotVelocity = baseDirection.RotatedBy(spread * i) * shotSpeed;
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    shotVelocity,
                    ModContent.ProjectileType<AstralStone>(),
                    damage,
                    0f,
                    Main.myPlayer
                );
            }
        }

        private void SpawnDashAstralStones(Vector2 dashDirection, float dashSpeed, bool phaseTwo, bool phaseThree)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int damage = phaseThree ? 180 : phaseTwo ? 155 : 130;
            Vector2 backward = -dashDirection;
            Vector2 perpendicular = dashDirection.RotatedBy(MathHelper.PiOver2);
            Vector2 backOffset = backward * 90f;
            Vector2 sideOffset = perpendicular * 110f;

            SpawnDashAstralStone(backOffset, dashDirection * dashSpeed, damage);
            SpawnDashAstralStone(backOffset + sideOffset, dashDirection * dashSpeed, damage);
            SpawnDashAstralStone(backOffset - sideOffset, dashDirection * dashSpeed, damage);
        }

        private void SpawnDashAstralStone(Vector2 spawnOffset, Vector2 velocity, int damage)
        {
            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center + spawnOffset,
                velocity,
                ModContent.ProjectileType<AstralStone>(),
                damage,
                0f,
                Main.myPlayer
            );
        }

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write(AttackPattern);
        }

        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            AttackPattern = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= AnimationCycleTicks)
                NPC.frameCounter = 0;

            int frame = (int)(NPC.frameCounter / (AnimationCycleTicks / AnimationFrames));
            if (frame >= AnimationFrames)
                frame = AnimationFrames - 1;

            NPC.frame.X = 0;
            NPC.frame.Width = AnimationFrameWidth;
            NPC.frame.Height = AnimationFrameHeight;
            NPC.frame.Y = frame * AnimationFrameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle frame = NPC.frame;
            if (frame.Width == 0 || frame.Height == 0)
                frame = new Rectangle(0, 0, AnimationFrameWidth, texture.Height / Main.npcFrameCount[NPC.type]);

            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
            {
                if (NPC.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = (NPC.oldPos.Length - i) / (float)NPC.oldPos.Length;
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos + new Vector2(0f, NPC.gfxOffY);
                Color trailColor = new Color(170, 70, 255, 0) * (0.42f * progress);
                float scale = NPC.scale * (0.88f + progress * 0.28f);
                float rotation = i < NPC.oldRot.Length ? NPC.oldRot[i] : NPC.rotation;

                Main.EntitySpriteDraw(texture, drawPos, frame, trailColor, rotation, origin, scale, effects, 0);
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0 || Main.dedServ)
                return;

            int goreType1 = ModContent.GoreType<AstraclysmGore1>();
            int goreType2 = ModContent.GoreType<AstraclysmGore2>();

            SpawnDeathGore(goreType1, new Vector2(-3.6f, -6.1f));
            SpawnDeathGore(goreType2, new Vector2(3.6f, -6.1f));

            for (int i = 0; i < 28; i++)
            {
                Vector2 dustPosition = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.4f, NPC.height * 0.35f);
                Vector2 dustVelocity = Main.rand.NextVector2Circular(5.6f, 5.6f) + new Vector2(0f, -1.4f);
                Dust dust = Dust.NewDustPerfect(
                    dustPosition,
                    181,
                    dustVelocity,
                    110,
                    default,
                    Main.rand.NextFloat(1.05f, 1.65f)
                );
                dust.noGravity = true;
            }
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextFloat() < 0.30f)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<HeartOfCalamity>());

                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.GoldCoin, 5);

                int particleAmount = Main.rand.Next(15, 41);
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<ParticleOfCalamity>(), particleAmount);
            }
        }

        private void SpawnDeathGore(int goreType, Vector2 baseVelocity)
        {
            Vector2 velocity = baseVelocity + Main.rand.NextVector2Circular(1.4f, 1.4f);
            int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, velocity, goreType, Main.rand.NextFloat(0.95f, 1.15f));
            if (goreIndex >= 0 && goreIndex < Main.gore.Length)
            {
                Main.gore[goreIndex].velocity = velocity;
                Main.gore[goreIndex].rotation = Main.rand.NextFloat(-0.45f, 0.45f);
            }
        }
    }
}
