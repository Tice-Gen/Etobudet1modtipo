using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.items;
using Etobudet1modtipo.Projectiles;
using Etobudet1modtipo.Systems.InfernalAwakening;
using System;
namespace Etobudet1modtipo.NPCs
{
    public class AshWoodenMonster : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Splinterling];
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.aiStyle = 3;
            AnimationType = NPCID.Splinterling;

            NPC.damage = 25;
            NPC.defense = 5;
            NPC.lifeMax = 250;

            NPC.knockBackResist = 0.9f;
            NPC.value = 60f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.lavaImmune = true;


            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.buffImmune[BuffID.Burning] = true;
        }




        public override float SpawnChance(NPCSpawnInfo spawnInfo)
{

    if (!InfernalAwakeningSystem.IsActive())
        return 0f;


    if (!spawnInfo.Player.ZoneUnderworldHeight)
        return 0f;

    if (spawnInfo.SpawnTileY < Main.maxTilesY - 200)
        return 0f;

    Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];

    if (tile.HasTile &&
       (tile.TileType == TileID.Ash ||
        tile.TileType == TileID.Hellstone ||
        tile.TileType == TileID.AshGrass))
    {
        return 0.8f;
    }

    return 0f;
}



        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

            for (int i = 0; i < 10; i++)
            {
                float vx = Main.rand.NextFloat(-2f, 2f);
                float vy = Main.rand.NextFloat(-2f, 2f);
                Dust.NewDust(target.position, target.width, target.height,
                    DustID.Smoke, vx, vy, 100, default, 1.2f);
            }


            target.AddBuff(ModContent.BuffType<AshDisease>(), 600);
        }




        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.AshWood, 1, 5, 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.AshBlock, 1, 10, 20));
            npcLoot.Add(ItemDropRule.Common(ItemID.Obsidian, 1, 2, 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hellstone, 4, 1, 3));
        }




        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {

                for (int i = 0; i < 8; i++)
                {
                    float vx = Main.rand.NextFloat(-2.5f, 2.5f);
                    float vy = Main.rand.NextFloat(-2.5f, 0f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height,
                        DustID.Smoke, vx, vy, 100, default, Main.rand.NextFloat(0.9f, 1.3f));
                }


                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    DustID.Lava, 0f, 0f, 100, default, 0.9f);
                return;
            }


            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    DustID.Ash, 0f, 0f, 150, default, 1.2f);
            }

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    DustID.Lava,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    100, default, 1.5f);
            }

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    DustID.Torch, 0f, 0f, 100, default, 2f);
            }


            int count = Main.rand.Next(3, 6);

            for (int i = 0; i < count; i++)
            {
                float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                Vector2 velocity =
                    new Vector2((float)System.Math.Cos(angle),
                                (float)System.Math.Sin(angle)) * 2f;

                Projectile.NewProjectile(
    NPC.GetSource_Death(),
    NPC.Center,
    velocity,
    ModContent.ProjectileType<ToxicSmoke3>(),
    20,
    0f,
    Main.myPlayer);

            }
        }




        public override void AI()
{

    NPC.TargetClosest();
    Player target = Main.player[NPC.target];

    if (NPC.lavaWet)
    {

        float horizontalSpeed = 2.4f;
        float horizontalAccel = 0.06f;
        float verticalSpeedMax = 4.2f;
        float verticalAccel = 0.10f;
        float followFactor = 0.028f;
        float sameLevelThreshold = 8f;


        NPC.defense = 50;
        NPC.damage = 150;
        NPC.noGravity = true;


        float targetY = target.Center.Y;
        float yDiff = targetY - NPC.Center.Y;


        float desiredVY;
        if (Math.Abs(yDiff) > sameLevelThreshold)
        {

            desiredVY = MathHelper.Clamp(yDiff * 0.06f, -verticalSpeedMax, verticalSpeedMax);
        }
        else
        {

            desiredVY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.45f;
        }

        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, desiredVY, verticalAccel);


        float xDiff = target.Center.X - NPC.Center.X;
        float desiredVX = MathHelper.Clamp(xDiff * followFactor, -horizontalSpeed, horizontalSpeed);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredVX, horizontalAccel);


        if (NPC.velocity.X > 0.1f) NPC.direction = NPC.spriteDirection = 1;
        else if (NPC.velocity.X < -0.1f) NPC.direction = NPC.spriteDirection = -1;


        if (Main.rand.NextBool(7))
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height,
                DustID.Smoke,
                NPC.velocity.X * 0.2f,
                NPC.velocity.Y * 0.2f,
                100, default, Main.rand.NextFloat(0.9f, 1.4f));
        }


        if (Main.rand.NextBool(60))
        {
            Dust.NewDust(NPC.Bottom - new Vector2(4, 0), 8, 6, DustID.Lava,
                0f, 1f, 100, default, 1.1f);
        }


    }
    else
    {

        NPC.noGravity = false;
        NPC.defense = 0;
        NPC.damage = 25;


        base.AI();
    }
}





        public override void SetBestiary(
            BestiaryDatabase database,
            BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags
                    .SpawnConditions.Biomes.TheUnderworld,

                new FlavorTextBestiaryInfoElement(
                    "A living ashwood creature of the underworld. The deeper the heat, the fiercer its strikes.")
            });
        }
    }
}

