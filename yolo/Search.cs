using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace yolo
{
	public class Search
	{
		Map map;
		Tile start;
		Tile goal;

		int cost = 10;
		int iterations = 0;
		public int Iterations { get { return iterations; } }

		bool found = false;
		public bool Found { get { return found; } }

		List<Tile> closedList;
		public List<Tile> ClosedList { get { return closedList; } }

		List<Tile> openList;
		public List<Tile> OpenList { get { return openList; } }

		List<Tile> path;
		public List<Tile> Path { get { return path; } }

		Dictionary<Tile, int> gScore;
		Dictionary<Tile, int> fScore;
		Dictionary<Tile, int> hScore;

		Dictionary<Tile, Tile> parents;

		public Search (Map map)
		{
			this.map = map;

			this.closedList = new List<Tile> ();
			this.openList = new List<Tile> ();
			this.path = new List<Tile> ();

			this.gScore = new Dictionary<Tile, int> ();
			this.fScore = new Dictionary<Tile, int> ();
			this.hScore = new Dictionary<Tile, int> ();

			this.parents = new Dictionary<Tile, Tile> ();
		}

		public void Start(Tile start, Tile goal) 
		{
			this.found = false;
			this.iterations = 0;

			this.closedList.Clear ();
			this.openList.Clear ();
			this.path.Clear ();

			this.start = start;
			this.goal = goal;

			this.gScore [start] = 0;
			this.hScore [start] = heuristicCost (start);
			this.fScore [start] = this.hScore [start];
			this.parents [start] = null;

			this.openList.Add (start);
		}

		private int heuristicCost(Tile tile) {

			return ((int) (tile.X - goal.X ) * (tile.X - goal.X ) + (tile.Y - goal.Y ) * (tile.Y - goal.Y )) * cost;
			// return (Math.Abs(tile.X - goal.X) + Math.Abs(tile.Y - goal.Y)) * (cost+4);
		}

		private Tile cheapestTile() {
			var min = this.openList [0];

			for (int i = 0; i < this.openList.Count; i++) {
				var tile = openList [i];
				if (fScore [tile] < fScore [min]) {
					min = tile;
				}
			}
			return min;
		}
			
		public void Full(int maxIterations) {
			while (!this.found && this.openList.Count > 0 && this.iterations < maxIterations) {
				this.Iterative ();
			}
		}

		public void Iterative()
		{
			if (found || openList.Count == 0)
				return;

			var current = cheapestTile ();

			if (current == this.goal) {
				this.found = true;
				while (parents [current] != null) {
					this.path.Add (current);
					current = parents [current];
				}
				return;
			}

			this.iterations += 1;

			var neighbors = map.WalkableNeighbors (current);
			var possibleGScore = gScore [current] + cost;

			for (int i = 0; i < neighbors.Length; i++) {
				var neighbor = neighbors [i];

				if (neighbor == null)
					continue;

				if (this.closedList.Contains (neighbor))
					continue;


				if (this.openList.Contains (neighbor)) {
					if (possibleGScore < gScore [neighbor]) {
						parents [neighbor] = current;
						gScore [neighbor] = possibleGScore;
						fScore [neighbor] = hScore [neighbor] + gScore [neighbor] ;
					}
				} else {
					this.openList.Add (neighbor);
					parents [neighbor] = current;
					gScore [neighbor] = possibleGScore;
					hScore [neighbor] = heuristicCost (neighbor);
					fScore [neighbor] = hScore [neighbor] + gScore [neighbor];
				}
			}

			this.openList.Remove (current);
			this.closedList.Add (current);
		}



		public void Iterative(int steps) 
		{
			for (int i = 0; i < steps; i++) 
			{
				Iterative ();
			}
		}

		public void Visualize(SpriteBatch batch)
		{
			for (int i = 0; i < this.openList.Count; i++) {
				var tile = this.openList [i];
				map.Overlay (batch, tile.X, tile.Y, 8, Color.Yellow);
			}

			for (int i = 0; i < this.closedList.Count; i++) {
				var tile = this.closedList [i];
				map.Overlay (batch, tile.X, tile.Y, 6, Color.Blue);
			}

			for (int i = 0; i < this.path.Count; i++) {
				var tile = this.path [i];
				map.Overlay (batch, tile.X, tile.Y, 4, Color.Aqua);
			}
		}



	}
}

