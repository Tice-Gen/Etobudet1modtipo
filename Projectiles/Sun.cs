using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Tiles;
using ReLogic.Utilities;

namespace Etobudet1modtipo.Projectiles
{
    public class Sun : ModProjectile
    {
        private const int LIFE_TIME = 600;
        private const int FADE_TIME = 90;

        private const float SLOWDOWN = 0.99f;
        private const float UP_DRIFT = 0.02f;

        private const int FrameWidth = 54;
        private const int FrameHeight = 52;

        private const int RepeatDelay = 10;
        private const int RepeatDamage = 1000;
        private const float HeatAuraRadius = 500f;
        private const float HeatAuraPercentDamage = 0.15f;
        private const float MaxLifePercentDamage = 0.10f;
        private const int ScorchRadiusTiles = 2;
        private const int CloudExplosionDamage = 50;
        private const float CloudExplosionRadius = 96f;



        private SlotId solarSoundSlot;
        private bool soundStarted;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = FrameWidth;
            Projectile.height = FrameHeight;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;

            Projectile.timeLeft = LIFE_TIME + FADE_TIME;

            Projectile.alpha = 255;
            Projectile.damage = 1;
            Projectile.knockBack = 0;

            Projectile.noEnchantmentVisuals = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SurfaceOfTheSun>(), 999999999);
            TrySpawnSunDeathEffects(target);

            if (Main.netMode != NetmodeID.MultiplayerClient && !target.dontTakeDamage && !target.immortal)
            {
                int percentDamage = (int)(target.lifeMax * MaxLifePercentDamage);
                if (percentDamage > 0)
                {
                    target.StrikeNPC(new NPC.HitInfo
                    {
                        Damage = percentDamage,
                        Knockback = 0f,
                        HitDirection = target.direction,
                        Crit = false
                    });

                    TrySpawnSunDeathEffects(target);
                }
            }

            if (Projectile.localAI[0] != 0f)
                return;

            Projectile.localAI[0] = 1f;
            Projectile.localAI[1] = RepeatDelay;
            Projectile.localAI[2] = target.whoAmI;
        }

        public override void AI()
        {

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }


            Projectile.rotation += 0.02f;
            Projectile.velocity *= SLOWDOWN;
            Projectile.velocity.Y -= UP_DRIFT;

            Lighting.AddLight(Projectile.Center, 0.25f, 0.08f, 0.02f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ApplyHeatAuraDamage();
                ScorchNearbySoil();
            }




            if (Projectile.localAI[0] == 1f)
            {
                Projectile.localAI[1]--;

                if (Projectile.localAI[1] <= 0f)
                {
                    int npcIndex = (int)Projectile.localAI[2];

                    if (npcIndex >= 0 &&
                        npcIndex < Main.maxNPCs &&
                        Main.npc[npcIndex].active &&
                        !Main.npc[npcIndex].friendly)
                    {
                        NPC npc = Main.npc[npcIndex];

                        npc.StrikeNPC(new NPC.HitInfo
                        {
                            Damage = RepeatDamage,
                            Knockback = 0f,
                            HitDirection = npc.direction,
                            Crit = false
                        });

                        TrySpawnSunDeathEffects(npc);
                    }

                    Projectile.localAI[0] = 2f;
                }
            }




            Player player = Main.LocalPlayer;

            float distance =
                Vector2.Distance(player.Center, Projectile.Center);

            const float silenceStart = 100f;
            const float silenceEnd = 700f;

            float volume;

            if (distance <= silenceStart)
                volume = 1f;
            else
                volume = 1f - (distance - silenceStart) /
                                 (silenceEnd - silenceStart);

            volume = MathHelper.Clamp(volume, 0f, 1f);


            if (volume <= 0f)
            {
                if (soundStarted &&
                    SoundEngine.TryGetActiveSound(
                        solarSoundSlot, out ActiveSound stopSound))
                {
                    stopSound.Stop();
                    soundStarted = false;
                }
            }
            else
            {
                SoundStyle sound = new SoundStyle(
                    "Etobudet1modtipo/Sounds/SolarRadiation")
                {
                    IsLooped = true,
                    Volume = volume,
                    MaxInstances = 1
                };

                if (!soundStarted)
                {
                    solarSoundSlot =
                        SoundEngine.PlaySound(sound, Projectile.Center);

                    soundStarted = true;
                }
                else if (SoundEngine.TryGetActiveSound(
                    solarSoundSlot, out ActiveSound active))
                {
                    active.Position = Projectile.Center;
                    active.Volume = volume;
                }
            }


            if (Projectile.timeLeft <= FADE_TIME)
            {
                Projectile.scale -= 1f / FADE_TIME;
                if (Projectile.scale < 0f)
                    Projectile.scale = 0f;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (soundStarted &&
                SoundEngine.TryGetActiveSound(
                    solarSoundSlot, out ActiveSound sound))
            {
                sound.Stop();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = new Rectangle(0, FrameHeight * Projectile.frame, FrameWidth, FrameHeight);
            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float pulse = 0.8f + 0.2f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f);
            Color glowColor = new Color(255, 120, 40, 0);

            for (int i = 0; i < 15; i++)
            {
                float scale = Projectile.scale * (1.8f + i * 0.16f) * pulse;
                Color layer = glowColor * (0.11f - i * 0.006f);
                if (layer.A == 0 && layer.R == 0 && layer.G == 0 && layer.B == 0)
                    continue;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    layer,
                    Projectile.rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            const int auraSegments = 36;
            for (int i = 0; i < auraSegments; i++)
            {
                float angle = MathHelper.TwoPi * i / auraSegments + Main.GlobalTimeWrappedHourly * 0.2f;
                Vector2 radial = angle.ToRotationVector2();
                Vector2 tangent = radial.RotatedBy(MathHelper.PiOver2);
                Vector2 edgeCenter = drawPos + radial * HeatAuraRadius;

                for (int cluster = -1; cluster <= 1; cluster++)
                {
                    Vector2 clusterPos = edgeCenter + tangent * (cluster * 8f);
                    Color edgeColor = glowColor * 0.18f;

                    Main.EntitySpriteDraw(
                        texture,
                        clusterPos,
                        frame,
                        edgeColor,
                        Projectile.rotation,
                        origin,
                        Projectile.scale * 0.55f * pulse,
                        SpriteEffects.None,
                        0
                    );
                }
            }

            const int innerSegments = 24;
            for (int i = 0; i < innerSegments; i++)
            {
                float angle = MathHelper.TwoPi * i / innerSegments - Main.GlobalTimeWrappedHourly * 0.15f;
                Vector2 radial = angle.ToRotationVector2();
                Vector2 tangent = radial.RotatedBy(MathHelper.PiOver2);

                Vector2 innerCenterA = drawPos + radial * (HeatAuraRadius * 0.6f);
                Vector2 innerCenterB = drawPos + radial * (HeatAuraRadius * 0.3f);

                for (int cluster = -1; cluster <= 1; cluster++)
                {
                    Vector2 posA = innerCenterA + tangent * (cluster * 6f);
                    Vector2 posB = innerCenterB + tangent * (cluster * 4f);

                    Main.EntitySpriteDraw(
                        texture,
                        posA,
                        frame,
                        glowColor * 0.12f,
                        Projectile.rotation,
                        origin,
                        Projectile.scale * 0.42f * pulse,
                        SpriteEffects.None,
                        0
                    );

                    Main.EntitySpriteDraw(
                        texture,
                        posB,
                        frame,
                        glowColor * 0.09f,
                        Projectile.rotation,
                        origin,
                        Projectile.scale * 0.34f * pulse,
                        SpriteEffects.None,
                        0
                    );
                }
            }

            return false;
        }

        private void ApplyHeatAuraDamage()
        {
            float radiusSq = HeatAuraRadius * HeatAuraRadius;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(npc.Center, Projectile.Center) > radiusSq)
                {
                    continue;
                }

                npc.AddBuff(ModContent.BuffType<SurfaceOfTheSun>(), 120);

                int heatAuraDamage = (int)(npc.lifeMax * HeatAuraPercentDamage);
                if (heatAuraDamage < 1)
                {
                    heatAuraDamage = 1;
                }

                npc.life -= heatAuraDamage;
                if (npc.life <= 0)
                {
                    npc.life = 0;
                    TrySpawnSunDeathEffects(npc);
                    npc.checkDead();
                }
                else
                {
                    npc.HitEffect(0, heatAuraDamage);
                }

                npc.netUpdate = true;
            }
        }

        private void ScorchNearbySoil()
        {
            int scorchTileType = ModContent.TileType<ScorchedEarthT>();

            int centerX = (int)(Projectile.Center.X / 16f);
            int centerY = (int)(Projectile.Center.Y / 16f);

            for (int x = centerX - ScorchRadiusTiles; x <= centerX + ScorchRadiusTiles; x++)
            {
                for (int y = centerY - ScorchRadiusTiles; y <= centerY + ScorchRadiusTiles; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);
                    bool evaporatedWater = false;

                    if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water)
                    {
                        tile.LiquidAmount = 0;
                        evaporatedWater = true;

                        if (Main.netMode != NetmodeID.Server)
                        {
                            Vector2 smokeBasePos = new Vector2(x * 16f + 8f, y * 16f + 8f);
                            for (int k = 0; k < 6; k++)
                            {
                                Vector2 spawnPos = smokeBasePos + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f));
                                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), Main.rand.NextFloat(-2.2f, -0.8f));
                                Dust.NewDustPerfect(spawnPos, DustID.Smoke, vel, 120, default, Main.rand.NextFloat(0.9f, 1.4f));
                            }
                        }
                    }

                    if (!tile.HasTile)
                    {
                        if (evaporatedWater && Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 3);
                        }

                        continue;
                    }

                    ushort type = tile.TileType;

                    if (type == TileID.Obsidian || type == TileID.Hellstone || type == ModContent.TileType<ScorchedEarthT>())
                    {
                        continue;
                    }

                    bool convertToWater =
                        type == TileID.SnowBlock ||
                        type == TileID.IceBlock ||
                        type == TileID.CorruptIce ||
                        type == TileID.HallowedIce ||
                        type == TileID.FleshIce;
                    bool explodeCloud =
                        type == TileID.Cloud ||
                        type == TileID.RainCloud ||
                        type == TileID.SnowCloud;

                    bool convertToObsidian =
                        type == TileID.Stone ||
                        type == TileID.Ebonstone ||
                        type == TileID.Crimstone;
                    bool convertOreToHellstone = TileID.Sets.Ore[type] || Main.tileOreFinderPriority[type] > 0;

                    bool convertToAsh =
                        type == TileID.Sand ||
                        type == TileID.Ebonsand ||
                        type == TileID.Crimsand ||
                        type == TileID.Pearlsand ||
                        type == TileID.LivingWood ||
                        type == TileID.LeafBlock;

                    bool canScorchToScorchedEarth =
                        type == TileID.Dirt ||
                        type == TileID.Mud ||
                        type == TileID.ClayBlock ||
                        type == TileID.Grass ||
                        type == TileID.CorruptGrass ||
                        type == TileID.CrimsonGrass ||
                        type == TileID.HallowedGrass ||
                        type == TileID.JungleGrass ||
                        type == TileID.MushroomGrass ||
                        type == ModContent.TileType<AniseGrassTile>();

                    if (convertToWater)
                    {
                        tile.HasTile = false;
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = 255;
                    }
                    else if (explodeCloud)
                    {
                        ExplodeCloudAtTile(x, y);
                    }
                    else
                    {
                        tile.TileType = (ushort)(
                            convertOreToHellstone ? TileID.Hellstone :
                            convertToObsidian ? TileID.Obsidian :
                            convertToAsh ? TileID.Ash :
                            canScorchToScorchedEarth ? scorchTileType :
                            TileID.Ash);
                    }

                    WorldGen.SquareTileFrame(x, y, true);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 3);
                    }
                }
            }
        }

        private void ExplodeCloudAtTile(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            tile.HasTile = false;

            Vector2 center = new Vector2(x * 16f + 8f, y * 16f + 8f);
            float radiusSq = CloudExplosionRadius * CloudExplosionRadius;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.dontTakeDamage || npc.immortal)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(npc.Center, center) > radiusSq)
                {
                    continue;
                }

                npc.StrikeNPC(new NPC.HitInfo
                {
                    Damage = CloudExplosionDamage,
                    Knockback = 5f,
                    HitDirection = npc.direction,
                    Crit = false
                });
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                {
                    continue;
                }

                if (Vector2.DistanceSquared(player.Center, center) > radiusSq)
                {
                    continue;
                }

                player.Hurt(
                    PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(player.name + " was blown up by a cloud.")),
                    CloudExplosionDamage,
                    player.direction);
            }

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14, center);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, -0.4f));
                    Dust.NewDustPerfect(center, DustID.Smoke, vel, 120, default, Main.rand.NextFloat(1f, 1.6f));
                }
            }
        }

        public static void TrySpawnSunDeathEffects(NPC npc)
        {
            if (npc == null || npc.active && npc.life > 0)
            {
                return;
            }

            SunDeathMarkerGlobalNPC marker = npc.GetGlobalNPC<SunDeathMarkerGlobalNPC>();
            if (marker.SunDeathEffectDone)
            {
                return;
            }

            marker.SunDeathEffectDone = true;

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            Rectangle npcRect = npc.Hitbox;

            for (int i = 0; i < Main.maxGore; i++)
            {
                Gore gore = Main.gore[i];
                if (!gore.active)
                {
                    continue;
                }

                Rectangle goreRect = new Rectangle((int)gore.position.X, (int)gore.position.Y, (int)gore.Width, (int)gore.Height);
                if (goreRect.Intersects(npcRect))
                {
                    gore.active = false;
                }
            }

            int particleCount = System.Math.Clamp((npc.width * npc.height) / 30, 30, 150);
            for (int i = 0; i < particleCount; i++)
            {
                Vector2 spawnPos = new Vector2(
                    Main.rand.NextFloat(npc.position.X, npc.position.X + npc.width),
                    Main.rand.NextFloat(npc.position.Y, npc.position.Y + npc.height)
                );

                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-4f, -1f)
                );

                Dust.NewDustPerfect(spawnPos, DustID.Torch, velocity, 120, default, Main.rand.NextFloat(1f, 1.7f));
                Dust.NewDustPerfect(spawnPos, DustID.Smoke, velocity * 0.6f, 150, default, Main.rand.NextFloat(0.9f, 1.4f));
            }

            IEntitySource source = npc.GetSource_Death();
            for (int i = 0; i < 5; i++)
            {
                Vector2 goreSpawnPos = new Vector2(
                    Main.rand.NextFloat(npc.position.X, npc.position.X + npc.width),
                    Main.rand.NextFloat(npc.position.Y, npc.position.Y + npc.height)
                );

                Vector2 goreVelocity = new Vector2(
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-5f, -1f)
                );

                Gore.NewGore(source, goreSpawnPos, goreVelocity, 99, Main.rand.NextFloat(0.9f, 1.25f));
            }
        }
    }

    public class SunDeathMarkerGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool SunDeathEffectDone;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            SunDeathEffectDone = false;
        }
    }
}
