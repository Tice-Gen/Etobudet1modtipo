using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Etobudet1modtipo.Players;

namespace Etobudet1modtipo.NPCs
{
    public class Statue : ModNPC
    {
        private bool _wasFrozen;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 42;

            NPC.aiStyle = 3;
            AIType = NPCID.Zombie;

            NPC.damage = 9999;
            NPC.defense = 0;
            NPC.lifeMax = 200;

            NPC.knockBackResist = 0.5f;
            NPC.value = 0f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            bool frozen = IsPlayerLookingAtMe(player);
            if (frozen)
            {
                NPC.aiStyle = 0;
                NPC.velocity.X = 0f;
            }
            else
            {
                NPC.aiStyle = 3;
            }

            if (frozen != _wasFrozen)
            {
                _wasFrozen = frozen;
                NPC.netUpdate = true;
            }
        }

        private bool IsPlayerLookingAtMe(Player player)
        {
            if (!player.active || player.dead)
                return false;

            Vector2 toNpc = NPC.Center - player.Center;
            bool facing = (player.direction == 1 && toNpc.X > 0f) ||
                          (player.direction == -1 && toNpc.X < 0f);

            if (!facing)
                return false;

            return Collision.CanHitLine(
                player.position, player.width, player.height,
                NPC.position, NPC.width, NPC.height
            );
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.GetModPlayer<BlackoutPlayer>().BlackoutActive = true;
            target.KillMe(PlayerDeathReason.ByNPC(NPC.whoAmI), 999999, 0);
        }
    }
}
