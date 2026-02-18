using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Etobudet1modtipo.Buffs;
using Microsoft.Xna.Framework;

namespace Etobudet1modtipo.items
{
    public class LivingBlade : ModItem
    {
        private int idleTimer = 0;
        private const float WoundedDamageMultiplier = 1.2f;
        private const float BossBonusMultiplier = 1.15f;
        private const float ExecutionMultiplier = 5f;

        private static readonly string[] AttackPhrases =
        {
            "Пей их кровь!",
            "Больше ярости, хозяин!",
            "Ощути вкус битвы!",
            "Слабак! Дай я покажу, как надо!",
            "Ещё! Ещё крови!",
            "Я живу ради этого!"
        };

        private static readonly string[] IdlePhrases =
        {
            "Ты снова забыл обо мне?",
            "Я голоден...",
            "Когда мы снова будем убивать?",
            "Не оставляй меня в одиночестве...",
            "Я слышу зов крови... а ты?"
        };

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 30;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 0f;
        }


        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            float multiplier = 1f;
            if (target.life < target.lifeMax)
                multiplier *= WoundedDamageMultiplier;
            if (target.lifeMax > 0 && target.life < target.lifeMax * 0.1f)
                multiplier *= ExecutionMultiplier;
            if (target.boss)
                multiplier *= BossBonusMultiplier;

            if (multiplier > 1f)
                modifiers.SourceDamage *= multiplier;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
{
    idleTimer = 0;


    for (int i = 0; i < 20; i++)
    {
        Dust.NewDustDirect(
            target.position,
            target.width,
            target.height,
            DustID.Blood,
            Main.rand.NextFloat(-4f, 4f),
            Main.rand.NextFloat(-4f, 4f),
            100,
            default,
            Main.rand.NextFloat(1.2f, 1.8f)
        ).noGravity = true;
    }


    if (Main.rand.NextBool(4))
    {
        Main.NewText(
            "[Клинок]: " + AttackPhrases[Main.rand.Next(AttackPhrases.Length)],
            Color.Red
        );
    }


    if (target.type == NPCID.TargetDummy)
        return;

    player.AddBuff(ModContent.BuffType<FleshSplash>(), 180);


    if (Main.rand.NextBool(2))
    {
        player.Heal(5);
    }
}


        public override void MeleeEffects(Player player, Rectangle hitbox)
{

    if (Main.rand.NextBool(2))
    {
        Dust dust = Dust.NewDustDirect(
            hitbox.TopLeft(),
            hitbox.Width,
            hitbox.Height,
            DustID.Blood,
            Main.rand.NextFloat(-2f, 2f),
            Main.rand.NextFloat(-2f, 2f),
            80,
            default,
            Main.rand.NextFloat(1.1f, 1.5f)
        );

        dust.noGravity = true;
    }
}



        public override void UpdateInventory(Player player)
        {
            idleTimer++;

            if (idleTimer >= 900)
            {
                if (player.HeldItem == Item && !player.controlUseItem)
                {
                    string phrase = IdlePhrases[Main.rand.Next(IdlePhrases.Length)];
                    Main.NewText("[Клинок]: " + phrase, Color.DarkRed);
                }

                idleTimer = 0;
            }
        }
    }
}
