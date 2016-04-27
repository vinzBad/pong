using System;
using Microsoft.Xna.Framework;

namespace yolo
{
	public class Actor
	{
		public int X {
			get;
			set;
		}

		public int Y {
			get;
			set;
		}

		public Color Color {
			get;
			set;
		}

		public int Width {
			get;
			set;
		}

		public int Height {
			get; 
			set;
		}

		public Actor ()
		{	
			this.Width = 24;
			this.Height = 24;
			this.Color = Color.Red;
		}
	}
}

