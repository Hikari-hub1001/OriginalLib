using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	public class SpriteNumberFloat : SpriteNumber<float>
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

			format += ".";
			var s = Value.ToString().Split(".");

			if (FloatZeroFill)
			{
				for (int i = 0; i < FloatFillDigits; i++)
				{
					format += "0";
				}
			}
			else if(s.Length > 1)
			{
				for (int i = 0; i < s[1].Length; i++)
				{
					format += "0";
				}
			}
			return Value.ToString(format);
		}
	}

}