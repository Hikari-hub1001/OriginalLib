using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	public class SpriteNumberInt : SpriteNumber<int>
	{


		protected override string CreateStrNum()
		{
			string format = "D";
			if (IntZeroFill)
			{
				format += IntFillDigit;
			}
			return Value.ToString(format);
		}

		//­”“_‚ÉŠÖ˜A‚·‚é‚à‚Ì‚Í‰B‚·
		private new Sprite PointSprite => null;
		private new void SetSprite(Sprite point){ }

		private new void SetSprite(Sprite zero, Sprite one, Sprite two,
			Sprite three, Sprite four, Sprite five, Sprite six,
			Sprite seven, Sprite eight, Sprite nine, Sprite plus, Sprite minus, Sprite point)
		{ }

	}

}
