using UnityEngine;
using System.Collections;

namespace com.javierquevedo
{
	//public enum BubbleColor { Red, Blue, Yellow, Green, Black, White };
	public enum BubbleColor { Red, Blue, Yellow}
	public class Bubble
	{

		private BubbleColor _color;

		public Bubble(BubbleColor color)
		{
			this._color = color;

		}

		public BubbleColor color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

	}
}
