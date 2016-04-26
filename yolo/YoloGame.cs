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

		bool[,] map;
		int map_width = 40;
		int map_height = 44;
		int tile_size = 24;
		int player_size = 12;
		int player_x = 1;
		int player_y = 1;

		int mouse_x = 0;
		int mouse_y = 0;

		int goal_x = 5;
		int goal_y = 3;
		int cost = 5;

		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();
		List<Node> path = new List<Node> ();

		Matrix cam = Matrix.Identity;
		float cam_speed = 50;

		Camera camera = new Camera ();

		bool goal_found = false;

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
			map = new bool[map_width,map_height];
			for (int x = 0; x < map_width; x++) {
				for (int y = 0; y < map_height; y++) {
					if ((0 < x  && x < map_width - 1) && (0 < y && y< map_height - 1)) {
						map [x,y] = false;
					} else {
						map [x, y] = true;
					}
				}
					
			}
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


			//TODO: use this.Content to load your game content here 
		}

		void IterativeSearch() {
			var current = GetCheapestNode (openList);

			if (current.X == goal_x && current.Y == goal_y) {
				goal_found = true;
				while (current.Parent != null) {
					path.Add (current);
					current = current.Parent;
				}

				return;
			}



			var neighbors = GetWalkableNeighbors (current);

			foreach (var node in neighbors) {

				if (closedList.Exists( n=> n.X == node.X && n.Y == node.Y )) {
					continue;
				}

				if (openList.Exists( n=> n.X == node.X && n.Y == node.Y )) {
					if (current.G + cost < node.G) {
						node.G = current.G + cost;
						node.F = node.H + node.G;
						node.Parent = current;
					}

				} else {
					node.Parent = current;
					node.G = current.G + cost;
					node.H = (Math.Abs (node.X - goal_x) + Math.Abs (node.Y - goal_y) )* (cost+2);
					node.F = node.H + cost;
					openList.Add (node);
				}
			}
			openList.RemoveAll (n => n.X == current.X && n.Y == current.Y); // (current);
			closedList.Add (current);
		}



		Node GetCheapestNode(List<Node> nodes) {
			var min = new Node () { F = int.MaxValue };

			foreach (var node in nodes) {
				if (node.F < min.F) {
					min = node;
				}
			}

			return min;
		}

		/// <summary>
		/// Gets the walkable neighbors of a node.
		/// </summary>
		/// <returns>The walkable neighbors.</returns>
		/// <param name="node">Node.</param>
		List<Node> GetWalkableNeighbors(Node node) {
			var nodes = new List<Node> (8);
			var neighbor = GetWalkableNode(node.X -1, node.Y);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X + 1, node.Y);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X, node.Y -1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X, node.Y +1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}
			/*
			neighbor = GetWalkableNode (node.X - 1, node.Y - 1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X + 1, node.Y - 1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X + 1, node.Y + 1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}

			neighbor = GetWalkableNode (node.X - 1, node.Y + 1);

			if (neighbor != null) {
				nodes.Add (neighbor);
			}
			*/
			return nodes;
		}

		Node GetWalkableNode (int x, int y) {
			if (x >= 0 && x < map_width && y >= 0 && y < map_height) {
				if (map [x, y] != true) {
					return new Node () { X = x, Y = y };
				}
			}
			return null;
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
			mouse_x = (int)mouse.X / tile_size;
			mouse_y = (int)mouse.Y / tile_size;

			if (!Inside(mouse_x, mouse_y) ){
				mouse_x = 0;
				mouse_y = 0;
			}

			if (ms.LeftButton == ButtonState.Pressed) {
				int x = ms.Position.X / tile_size;
				int y = ms.Position.Y / tile_size;
				if (Inside (x, y)) {
					map [x, y] = true;
					doSearch = true;
				}
			}

			if (ms.RightButton == ButtonState.Pressed) {
				int x = ms.Position.X / tile_size;
				int y = ms.Position.Y / tile_size;

				if (Inside (x, y)) {
					map [x, y] = false;
					doSearch = true;
				}
			}
			int prev_x = player_x;
			int prev_y = player_y;

			if (ks.IsKeyDown(Keys.D)) {
				player_x += 1;
			}
			if (ks.IsKeyDown(Keys.A)) {
				player_x -= 1;
			}
			if (ks.IsKeyDown(Keys.W)) {
				player_y -= 1;
			}
			if (ks.IsKeyDown(Keys.S)) {
				player_y += 1;
			}

		

			if (map [player_x, player_y] == true) {
				player_x = prev_x;
				player_y = prev_y;
			}

			camera.CenterOn (new Vector2 (player_x * tile_size, player_y * tile_size));

			if (doSearch || prev_x != player_x || prev_y != player_y) {
				openList.Clear ();
				closedList.Clear ();
				path.Clear ();

				goal_found = false;
				openList.Add (new Node () {
					X = player_x,
					Y = player_y,
					G = 0,
					H = Math.Abs(player_x - goal_x) + Math.Abs(player_y - goal_y),
					F = Math.Abs(player_x - goal_x) + Math.Abs(player_y - goal_y),
					Parent = null
				});

			}

			if (goal_found == false) {
				IterativeSearch ();
			}

            


			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
			int offset = tile_size / 2 - player_size / 2;
			//TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null, null,null,Matrix.Identity);
			for (int x = 0; x < map_width; x++) {
				for (int y = 0; y < map_height; y++) {
					if (map[x,y] == true) {
						spriteBatch.Draw (pixel, new Rectangle (x*tile_size, y*tile_size, tile_size, tile_size), null, Color.Black);
					} else {
						spriteBatch.Draw (pixel, new Rectangle (x*tile_size, y*tile_size, tile_size, tile_size), null, Color.White);
					}
				}


				foreach (var node in closedList) {
					spriteBatch.Draw (pixel, new Rectangle (node.X * tile_size + offset +2, node.Y * tile_size + offset+2, player_size+6, player_size+6), null, Color.CadetBlue);
				}
				foreach (var node in openList) {
					spriteBatch.Draw (pixel, new Rectangle (node.X * tile_size + offset +1, node.Y * tile_size + offset+1, player_size+2, player_size+2), null, Color.Blue);
				}
				foreach (var node in path) {
					spriteBatch.Draw (pixel, new Rectangle (node.X * tile_size + offset +1, node.Y * tile_size + offset+1, player_size+2, player_size+2), null, Color.Aqua);
				}

				spriteBatch.Draw (pixel, new Rectangle (player_x * tile_size + offset, player_y * tile_size + offset, player_size, player_size), null, Color.IndianRed);
				spriteBatch.Draw (pixel, new Rectangle (goal_x * tile_size + offset, goal_y * tile_size + offset, player_size, player_size), null, Color.DarkSeaGreen);

				spriteBatch.Draw (pixel, new Rectangle (mouse_x*tile_size, mouse_y*tile_size, tile_size, tile_size), null, new Color(Color.GreenYellow, 50));

			}
				
			spriteBatch.End ();
            
			base.Draw (gameTime);
		}
	}
}

