using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Etobudet1modtipo
{
    [Label("Anise Mod Settings")]
    public class EtobudetConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;


        [Header("CraftSettings")] 
        

        [Label("Logical, more complex recipes")]
        [Tooltip("If enabled, crafting Anise Soda requires an additional ingredient (TinCan).")]
        [DefaultValue(false)] 
        public bool UseHarderRecipes { get; set; }
    }
}
