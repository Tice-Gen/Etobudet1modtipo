using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Systems.InfernalAwakening;
using System.Collections.Generic;

namespace Etobudet1modtipo.NPCs
{
    public class InfernalWallOfFlesh : GlobalNPC
    {
        public override bool InstancePerEntity => true;




        private static Dictionary<int, DamageCapData> sharedCaps = new();

        private class DamageCapData
        {
            public int timer;
            public int damage;
        }




        private int toxicAttackTimer;

        private int healTimer;
        private int healBurstsLeft;
        private int healAmount;

        private bool healed35;
        private bool healed10;
        private bool healed5;


        private const int DAMAGE_CAP = 1000;
        private const int DAMAGE_WINDOW = 300;


        public override bool AppliesToEntity(NPC npc, bool lateInstantiation)
        {
            return npc.type == NPCID.WallofFlesh
                || npc.type == NPCID.WallofFleshEye;
        }


        public override void AI(NPC npc)
        {
            if (!InfernalAwakeningSystem.IsActive())
                return;

            int root = npc.realLife >= 0 ? npc.realLife : npc.whoAmI;

            if (!sharedCaps.TryGetValue(root, out var data))
            {
                data = new DamageCapData();
                sharedCaps[root] = data;
            }

            data.timer++;

            if (data.timer >= DAMAGE_WINDOW)
            {
                data.timer = 0;
                data.damage = 0;
            }

            if (npc.type == NPCID.WallofFlesh)
            {
                HandleToxicAttack(npc);
                HandleHealing(npc);
            }
        }




        public override void ModifyHitByItem(
            NPC npc,
            Player player,
            Item item,
            ref NPC.HitModifiers modifiers)
        {
            ApplyCap(npc, ref modifiers);
        }

        public override void ModifyHitByProjectile(
            NPC npc,
            Projectile projectile,
            ref NPC.HitModifiers modifiers)
        {
            ApplyCap(npc, ref modifiers);
        }

        private void ApplyCap(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!InfernalAwakeningSystem.IsActive())
                return;

            int root = npc.realLife >= 0 ? npc.realLife : npc.whoAmI;

            if (!sharedCaps.TryGetValue(root, out var data))
                return;

            modifiers.ModifyHitInfo += (ref NPC.HitInfo hit) =>
            {
                int dmg = hit.Damage;


                if (data.damage == 0 && dmg >= DAMAGE_CAP)
                {
                    data.damage = DAMAGE_CAP;
                    return;
                }

                if (data.damage >= DAMAGE_CAP)
                {
                    hit.Damage = 1;
                    return;
                }

                if (data.damage + dmg > DAMAGE_CAP)
                {
                    hit.Damage = 1;
                    data.damage = DAMAGE_CAP;
                    return;
                }

                data.damage += dmg;
            };
        }




        private void HandleToxicAttack(NPC npc)
        {
            toxicAttackTimer++;

            if (toxicAttackTimer >= 360)
            {
                toxicAttackTimer = 0;
                FireToxicHalfCircle(npc);
            }
        }

        private void FireToxicHalfCircle(NPC wall)
        {
            Player player = Main.player[wall.target];
            if (!player.active || player.dead)
                return;

            Vector2 dir = player.Center - wall.Center;
            dir.Normalize();

            Vector2 mouthPos = wall.Center;
            mouthPos.X += wall.direction * 140f;
            mouthPos.Y -= 30f;

            const int count = 24;
            const float speed = 7f;

            float baseRot = dir.ToRotation();
            float spread = MathHelper.Pi;

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                float rot = baseRot - spread / 2f + spread * t;

                Projectile.NewProjectile(
                    wall.GetSource_FromAI(),
                    mouthPos,
                    rot.ToRotationVector2() * speed,
                    ModContent.ProjectileType<Projectiles.ToxicSmokeForWallOfFlesh>(),
                    45,
                    2f,
                    Main.myPlayer
                );
            }
        }




        private void HandleHealing(NPC npc)
        {
            float hp = npc.life / (float)npc.lifeMax;

            if (!healed35 && hp <= 0.35f)
            {
                StartHealBurst(4, 500);
                healed35 = true;
            }

            if (!healed10 && hp <= 0.10f)
            {
                StartHealBurst(4, 500);
                healed10 = true;
            }

            if (!healed5 && hp <= 0.05f)
            {
                StartHealBurst(1, 1500);
                healed5 = true;
            }

            if (healBurstsLeft > 0)
            {
                healTimer++;

                if (healTimer >= 240)
                {
                    healTimer = 0;
                    healBurstsLeft--;

                    npc.life += healAmount;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;

                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, healAmount);
                    npc.HealEffect(healAmount, true);
                }
            }
        }

        private void StartHealBurst(int count, int amount)
        {
            healBurstsLeft = count;
            healAmount = amount;
            healTimer = 0;
        }
        public override void OnKill(NPC npc)
{

    if (npc.type != NPCID.WallofFlesh)
        return;


    if (!Main.hardMode)
        return;


    if (Main.netMode == NetmodeID.MultiplayerClient)
        return;


    Vector2 spawnPos = npc.Center;

    NPC.NewNPC(
        npc.GetSource_Death(),
        (int)spawnPos.X,
        (int)spawnPos.Y,
        ModContent.NPCType<TheHungriest>()
    );
}


    }
}
