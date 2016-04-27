using System;

namespace yolo
{
	public enum TileType {
		FLOOR,
		WALL
	}
	public class Tile
	{
		public int X {
			get;
			private set;
		}

		public int Y {
			get;
			private set;
		}
		public Tile (int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public TileType Type {
			get;
			set;
		}
	}
}

