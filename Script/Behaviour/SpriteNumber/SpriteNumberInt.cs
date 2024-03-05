using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	public class SpriteNumberInt : SpriteNumber<int>
	{
		protected override string CreateStrNum()
		{
			string format = "0";
			if (IntZeroFill)
			{
				for (int i = 1; i < IntFillDigit; i++)
				{
					format = "0" + format;
				}
			}
			return Value.ToString(format);
		}


		//少数点を設定するメソッドは隠す
		private new void SetSprite(Sprite point){ }

		private new void SetSprite(Sprite zero, Sprite one, Sprite two,
			Sprite three, Sprite four, Sprite five, Sprite six,
			Sprite seven, Sprite eight, Sprite nine, Sprite plus, Sprite minus, Sprite point)
		{ }

	}

}
