using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Etobudet1modtipo.Tiles
{
    public class AniseTree : ModTree
    {
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[] { ModContent.TileType<AniseGrassTile>() };
        }

        public override TreeTypes CountsAsTreeType => TreeTypes.Forest;

        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings();

        public override int DropWood() => ItemID.Wood;

        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("Terraria/Images/Tiles_5");

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("Terraria/Images/Tree_Tops_0");

        public override Asset<Texture2D> GetBranchTextures() => ModContent.Request<Texture2D>("Terraria/Images/Tree_Branches_0");

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            topTextureFrameWidth = 80;
            topTextureFrameHeight = 80;
        }
    }
}
