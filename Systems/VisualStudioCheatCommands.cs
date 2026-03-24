using System;
using Etobudet1modtipo.Buffs;
using Etobudet1modtipo.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Systems
{
    public static class VisualStudioCheatUtils
    {
        public static bool CanUseCheats(CommandCaller caller, out string reason)
        {
            reason = string.Empty;

            if (caller.Player == null)
            {
                reason = "This command can only be used by a player.";
                return false;
            }

            if (!caller.Player.HasBuff(ModContent.BuffType<VisualStudioBuff>()))
            {
                reason = "You need Visual Studio Buff to use this command.";
                return false;
            }

            if (Main.netMode != NetmodeID.SinglePlayer && caller.Player.whoAmI != 0)
            {
                reason = "In multiplayer, only the host owner can use this command.";
                return false;
            }

            return true;
        }

        public static bool TryParseIdOrName(string raw, bool npc, out int type)
        {
            type = 0;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            if (raw.StartsWith("ID:", StringComparison.OrdinalIgnoreCase))
            {
                return int.TryParse(raw.Substring(3), out type);
            }

            if (int.TryParse(raw, out int numericId))
            {
                type = numericId;
                return true;
            }

            string query = raw.Trim();
            if (npc)
            {
                for (int i = 1; i < NPCLoader.NPCCount; i++)
                {
                    if (!ContentSamples.NpcsByNetId.TryGetValue(i, out NPC sample))
                    {
                        continue;
                    }

                    string displayName = sample.TypeName ?? string.Empty;
                    string langName = Lang.GetNPCNameValue(i);
                    if (query.Equals(displayName, StringComparison.OrdinalIgnoreCase) ||
                        query.Equals(langName, StringComparison.OrdinalIgnoreCase))
                    {
                        type = i;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 1; i < ItemLoader.ItemCount; i++)
                {
                    if (!ContentSamples.ItemsByType.TryGetValue(i, out Item sample))
                    {
                        continue;
                    }

                    string displayName = sample.Name ?? string.Empty;
                    string langName = Lang.GetItemNameValue(i);
                    if (query.Equals(displayName, StringComparison.OrdinalIgnoreCase) ||
                        query.Equals(langName, StringComparison.OrdinalIgnoreCase))
                    {
                        type = i;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryParseAmount(string raw, out int amount)
        {
            if (int.TryParse(raw, out amount))
            {
                amount = Math.Clamp(amount, 1, 9999);
                return true;
            }

            return false;
        }
    }

    public class GiveCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "give";
        public override string Usage => "/give <item name|ID:123|123> <amount>";
        public override string Description => "Gives an item when Visual Studio Buff is active.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!VisualStudioCheatUtils.CanUseCheats(caller, out string reason))
            {
                caller.Reply(reason, Color.OrangeRed);
                return;
            }

            if (args.Length < 2)
            {
                caller.Reply(Usage, Color.Yellow);
                return;
            }

            string amountRaw = args[args.Length - 1];
            if (!VisualStudioCheatUtils.TryParseAmount(amountRaw, out int amount))
            {
                caller.Reply("Invalid amount.", Color.OrangeRed);
                return;
            }

            string itemRaw = string.Join(" ", args, 0, args.Length - 1);
            if (!VisualStudioCheatUtils.TryParseIdOrName(itemRaw, npc: false, out int itemType) ||
                itemType <= ItemID.None || itemType >= ItemLoader.ItemCount)
            {
                caller.Reply("Item not found.", Color.OrangeRed);
                return;
            }

            int spawned = Item.NewItem(caller.Player.GetSource_GiftOrReward(), caller.Player.Hitbox, itemType, amount);
            if (spawned >= 0 && spawned < Main.maxItems)
            {
                string itemName = Lang.GetItemNameValue(itemType);
                caller.Reply($"Given: {itemName} x{amount}", Color.LightBlue);
            }
        }
    }

    public class SpawnCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "spawn";
        public override string Usage => "/spawn <npc name|ID:50|50> <amount>";
        public override string Description => "Spawns NPCs when Visual Studio Buff is active.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!VisualStudioCheatUtils.CanUseCheats(caller, out string reason))
            {
                caller.Reply(reason, Color.OrangeRed);
                return;
            }

            if (args.Length < 2)
            {
                caller.Reply(Usage, Color.Yellow);
                return;
            }

            string amountRaw = args[args.Length - 1];
            if (!VisualStudioCheatUtils.TryParseAmount(amountRaw, out int amount))
            {
                caller.Reply("Invalid amount.", Color.OrangeRed);
                return;
            }

            string npcRaw = string.Join(" ", args, 0, args.Length - 1);
            if (!VisualStudioCheatUtils.TryParseIdOrName(npcRaw, npc: true, out int npcType) ||
                npcType <= NPCID.None || npcType >= NPCLoader.NPCCount)
            {
                caller.Reply("NPC not found.", Color.OrangeRed);
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                int x = (int)caller.Player.Center.X + Main.rand.Next(-160, 161);
                int y = (int)caller.Player.Center.Y - 80;
                NPC.NewNPC(caller.Player.GetSource_FromThis(), x, y, npcType);
            }

            string npcName = Lang.GetNPCNameValue(npcType);
            caller.Reply($"Spawned: {npcName} x{amount}", Color.LightBlue);
        }
    }

    public class GodCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "god";
        public override string Usage => "/god";
        public override string Description => "Enables immortality when Visual Studio Buff is active.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!VisualStudioCheatUtils.CanUseCheats(caller, out string reason))
            {
                caller.Reply(reason, Color.OrangeRed);
                return;
            }

            var cheatPlayer = caller.Player.GetModPlayer<VisualStudioCheatPlayer>();
            cheatPlayer.GodModeEnabled = true;
            caller.Reply("God mode enabled.", Color.LightGreen);
        }
    }

    public class HumanCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "human";
        public override string Usage => "/human";
        public override string Description => "Disables immortality.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!VisualStudioCheatUtils.CanUseCheats(caller, out string reason))
            {
                caller.Reply(reason, Color.OrangeRed);
                return;
            }

            var cheatPlayer = caller.Player.GetModPlayer<VisualStudioCheatPlayer>();
            cheatPlayer.GodModeEnabled = false;
            caller.Reply("God mode disabled.", Color.LightGreen);
        }
    }

    public class ShowCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "show";
        public override string Usage => "/show GoodNature";
        public override string Description => "Shows hidden debug values when Visual Studio Buff is active.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player == null)
            {
                caller.Reply("This command can only be used by a player.", Color.OrangeRed);
                return;
            }

            if (!caller.Player.HasBuff(ModContent.BuffType<VisualStudioBuff>()))
            {
                caller.Reply("You need Visual Studio Buff to use this command.", Color.OrangeRed);
                return;
            }

            if (args.Length < 1)
            {
                caller.Reply(Usage, Color.Yellow);
                return;
            }

            if (args[0].Equals("GoodNature", StringComparison.OrdinalIgnoreCase))
            {
                AnimalsSaverAchievementPlayer data = caller.Player.GetModPlayer<AnimalsSaverAchievementPlayer>();
                int points = data.KindnessPoints;
                bool unlockedFlag = data.AnimalsSaverUnlocked;
                bool unlockedUi = data.IsAnimalsSaverCompletedForUi();
                caller.Reply($"GoodNature points: {points}, AnimalsSaver flag: {unlockedFlag}, UI completed: {unlockedUi}", Color.LightGreen);
                return;
            }

            caller.Reply("Unknown value. Available: GoodNature", Color.OrangeRed);
        }
    }
}
