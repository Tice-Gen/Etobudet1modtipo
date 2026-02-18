using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Etobudet1modtipo.Buffs;
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
            Texture2D texture =
                TextureAssets.Projectile[Projectile.type].Value;

            Rectangle frame = new Rectangle(
                0,
                FrameHeight * Projectile.frame,
                FrameWidth,
                FrameHeight
            );

            Vector2 origin = frame.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero)
                    continue;

                float progress =
                    (Projectile.oldPos.Length - k) /
                    (float)Projectile.oldPos.Length;

                Vector2 drawPos =
                    Projectile.oldPos[k]
                    - Main.screenPosition
                    + Projectile.Size / 2f
                    + new Vector2(0f, Projectile.gfxOffY);

                Color glow =
                    new Color(255, 180, 90, 0) *
                    progress * 0.35f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    frame,
                    glow,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * (1f + k * 0.05f),
                    SpriteEffects.None,
                    0
                );
            }

            return false;
        }
    }
}
