using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Etobudet1modtipo.items
{



    public class OneHpChallenge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Master;
            Item.value = 0;
        }



        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
    }