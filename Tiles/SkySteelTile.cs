using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Etobudet1modtipo.items;

namespace Etobudet1modtipo.Tiles
{
	public class SkySteelTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileShine[Type] = 1100;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			

			DustType = 319;
			


			
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			RegisterItemDrop(ModContent.ItemType<SkySteel>());

			AddMapEntry(new Color(120, 107, 130), Language.GetText("SkySteel"));
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			if (!WorldGen.SolidTileAllowBottomSlope(i, j + 1)) {
				WorldGen.KillTile(i, j);
			}
			return true;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {

			if (!fail && !effectOnly && Main.netMode != NetmodeID.Server) {
				for (int k = 0; k < 5; k++) {
					Dust dust = Dust.NewDustDirect(
						new Vector2(i * 16, j * 16),
						16,
						16,
						222,
						0f,
						0f,
						100,
						default(Color),
						1.5f
					);
					
					dust.noGravity = true;
					dust.velocity *= 2f;
					dust.velocity = new Vector2(
						Main.rand.NextFloat(-2f, 2f),
						Main.rand.NextFloat(-3f, -1f)
					);
				}
			}
		}
	}
}