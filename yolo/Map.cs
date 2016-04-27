using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace yolo
{
	public class Map
	{
		Tile[,] tiles;
		List<Actor> actors;
		Texture2D pixel;

		int width;
		int height;
		int tile_size;

		Color tile_tint = Color.White;
		Rectangle rect_dst;

		public Map (int width, int height, int tile_size)
		{
			this.width = width;
			this.height = height;
			this.tile_size = tile_size;

			this.tiles = new Tile[width, height];
			this.actors = new List<Actor> ();

			rect_dst = new Rectangle (0, 0, tile_size, tile_size);

		}

		public Tile WorldGet(Vector2 pos) 
		{
			var x = (int) pos.X / this.tile_size;
			var y = (int) pos.Y / this.tile_size;

			return this.Get (x, y);
		}

		public Tile Get (int x, int y)
		{
			if (this.Inside (x, y)) {
				var result = this.tiles [x, y];

				if (result == null) {
					result = new Tile (x, y);
					this.tiles [x, y] = result;
				}

				return result;

			}
			return null;
		}

		public bool Inside (int x, int y)
		{
			return (0 <= x && x < this.width && 0 <= y && y < this.height);
		}


		int[,] neighborIndices = new int[,] { 
			{ 1, 0 },
			{ -1, 0 }, 
			{ 0, 1 }, 
			{ 0, -1 },
			{ 1, 1 },
			{ -1, 1 },
			{ -1, 1 },
			{ -1, -1 }
		};


		public Tile[] WalkableNeighbors (Tile tile)
		{
			var result = new Tile[neighborIndices.GetLength (0)];

			for (int i = 0; i < neighborIndices.GetLength (0); i++) {
				var dx = neighborIndices [i, 0];
				var dy = neighborIndices [i, 1];
				var neighbor = this.Get (tile.X + dx, tile.Y + dy);

				if (neighbor != null && neighbor.Type != TileType.WALL) {
					result [i] = neighbor;
					continue;
				} 
				result [i] = null;
			}

			return result;
		}

		public void Add (Actor actor)
		{
			this.actors.Add (actor);
		}

		public int Width {
			get { return this.width; }
		}

		public int Height {
			get { return this.height; }
		}

		public void Render (SpriteBatch batch)
		{
			if (pixel == null) {
				pixel = new Texture2D (batch.GraphicsDevice, 1, 1);
				pixel.SetData<Color> (new Color[]{ Color.White });
			}

			rect_dst.Width = tile_size;
			rect_dst.Height = tile_size;


			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					
					var tile = this.Get (x, y);

					if (tile.Type == TileType.FLOOR)
						tile_tint = Color.Wheat;

					if (tile.Type == TileType.WALL)
						tile_tint = Color.Black;

					rect_dst.X = x * this.tile_size;
					rect_dst.Y = y * this.tile_size;

					batch.Draw (pixel, rect_dst, tile_tint);
				}

			}

			for (int i = 0; i < actors.Count; i++) {
				var actor = actors [i];

				rect_dst.X = actor.X * this.tile_size + actor.Width / 2;
				rect_dst.Y = actor.Y * this.tile_size + actor.Height / 2;
				rect_dst.Width = actor.Width;
				rect_dst.Height = actor.Height;

				tile_tint = actor.Color;

				batch.Draw (pixel, rect_dst, tile_tint);
			}
		}

		public void Render(SpriteBatch batch, Vector2 min, Vector2 max)
		{
			Render (batch, (int) min.X / tile_size, (int) min.Y / tile_size, (int) max.X / tile_size, (int) max.Y / tile_size);
		}

		public void Render(SpriteBatch batch, int minX, int minY, int maxX, int maxY)
		{
			if (pixel == null) {
				pixel = new Texture2D (batch.GraphicsDevice, 1, 1);
				pixel.SetData<Color> (new Color[]{ Color.White });
			}

			// bounds check
			if (!Inside (minX, minY) || !Inside (maxX, maxY))
				return;

			rect_dst.Width = tile_size;
			rect_dst.Height = tile_size;


			for (int x = minX; x < maxX; x++) {
				for (int y = minY; y < maxY; y++) {

					var tile = this.Get (x, y);

					if (tile.Type == TileType.FLOOR)
						tile_tint = Color.Wheat;

					if (tile.Type == TileType.WALL)
						tile_tint = Color.Black;

					rect_dst.X = x * this.tile_size;
					rect_dst.Y = y * this.tile_size;

					batch.Draw (pixel, rect_dst, tile_tint);
				}

			}

			for (int i = 0; i < actors.Count; i++) {
				var actor = actors [i];

				rect_dst.X = actor.X * this.tile_size + actor.Width / 2;
				rect_dst.Y = actor.Y * this.tile_size + actor.Height / 2;
				rect_dst.Width = actor.Width;
				rect_dst.Height = actor.Height;

				tile_tint = actor.Color;

				batch.Draw (pixel, rect_dst, tile_tint);
			}
		}

		public void Overlay (SpriteBatch batch, int x, int y, int size, Color color)
		{
			if (pixel == null) {
				pixel = new Texture2D (batch.GraphicsDevice, 1, 1);
				pixel.SetData<Color> (new Color[]{ Color.White });
			}

			rect_dst.X = x * this.tile_size + size / 2;
			rect_dst.Y = y * this.tile_size + size / 2;
			rect_dst.Width = size;
			rect_dst.Height = size;

			tile_tint = color;

			batch.Draw (pixel, rect_dst, tile_tint);

		}
	}
}

