using Terraria.ModLoader;

namespace Etobudet1modtipo.Systems
{
    public class OtherWatersCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "otherwaters";
        public override string Usage => "/otherwaters";
        public override string Description => "Enter or exit the Other Waters subworld.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool available = global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersAvailable();
            bool active = global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive();
            string id = global::Etobudet1modtipo.Etobudet1modtipo.OtherWatersIdentifier;
            caller.Reply($"OW status: available={available}, active={active}, id={id}");

            if (!global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersAvailable())
            {
                caller.Reply("SubworldLibrary was not found or Other Waters is not registered.");
                return;
            }

            if (global::Etobudet1modtipo.Etobudet1modtipo.IsOtherWatersActive())
            {
                if (!global::Etobudet1modtipo.Etobudet1modtipo.ExitSubworld())
                {
                    caller.Reply("Could not exit the subworld.");
                }

                return;
            }

            if (!global::Etobudet1modtipo.Etobudet1modtipo.EnterOtherWaters())
            {
                caller.Reply("Could not enter Other Waters.");
            }
        }
    }
}
