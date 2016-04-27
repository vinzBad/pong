using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace yolo
{
	class Node {
		public int X;
		public int Y;
		public Node Parent;
		public int F; // G + H
		public int G; // 
		public int H;
	}
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class YoloGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D pixel;

		Map map;
		Search search;

		int map_width = 40;
		int map_height = 44;

		Actor player;
		Actor goal;

		/*
		int mouse_x = 0;
		int mouse_y = 0;
		*/
	

		Camera camera = new Camera ();

	
		public YoloGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			IsMouseVisible = true;
			// TODO: Add your initialization logic here
			//map = new bool[map_width,map_height];

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			pixel = new Texture2D(GraphicsDevice,1,1);
			pixel.SetData<Color> (new Color[] { Color.White });

			camera.ViewportWidth  = graphics.GraphicsDevice.Viewport.Width;
			camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

			map = new Map (map_width, map_height, 18);

			for (int x = 0; x < map_width; x++) {
				for (int y = 0; y < map_height; y++) {
					if ((0 < x && x < map_width - 1) && (0 < y && y < map_height - 1)) {
						map.Get (x, y).Type = TileType.FLOOR;
					} else {
						map.Get (x, y).Type = TileType.WALL;
					}
				}
			
			}

			player = new Actor () {
				X = 3,
				Y = 4,
				Width = 6,
				Height = 12,
				Color = Color.Green
			};

			goal = new Actor () {
				X = 10,
				Y = 12, 
				Width = 12,
				Height = 12,
				Color = Color.Red
			};

			map.Add (player);
			map.Add (goal);

			search = new Search (map);


			//TODO: use this.Content to load your game content here 
		}

		bool Inside(int x, int y) {
			return 0 <= x && x < map_width && 0 <= y && y <= map_height;
		}
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			// TODO: Add your update logic here
			var ks = Keyboard.GetState();
			 var ms = Mouse.GetState ();

			bool doSearch = false;

			Vector2 mouse = new Vector2 (ms.Position.X, ms.Position.Y);

			if (ms.LeftButton == ButtonState.Pressed) {
				var tile = map.WorldGet (mouse);
				if (tile != null)
					tile.Type = TileType.WALL;
			}

			if (ms.RightButton == ButtonState.Pressed) {
				var tile = map.WorldGet (mouse);
				if (tile != null)
					tile.Type = TileType.FLOOR;
			}

			int prev_x = player.X;
			int prev_y = player.Y;

			if (ks.IsKeyDown(Keys.D)) {
				player.X += 1;
			}
			if (ks.IsKeyDown(Keys.A)) {
				player.X -= 1;
			}
			if (ks.IsKeyDown(Keys.W)) {
				player.Y -= 1;
			}
			if (ks.IsKeyDown(Keys.S)) {
				player.Y += 1;

			
			}

		

			if (map.Get(player.X, player.Y).Type == TileType.WALL) {
				player.X = prev_x;
				player.Y = prev_y;
			}

			//camera.CenterOn (new Vector2 (player_x * tile_size, player_y * tile_size));

			if (doSearch || prev_x != player.X || prev_y != player.Y) {
				search.Start (
					map.Get (player.X, player.Y),
					map.Get (goal.X, goal.Y));
			}



			search.Iterative (10);
            


			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);

			//TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null, null,null,Matrix.Identity);

			map.Render (spriteBatch);

			search.Visualize (spriteBatch);

			spriteBatch.End ();
            
			base.Draw (gameTime);
		}
	}
}

