using Codice.Client.BaseCommands.WkStatus.Printers;
using UnityEngine;

namespace OriginalLib
{
	public class ColorUtil
	{
		/// <summary>
		/// 0~255でカラーを構築
		/// </summary>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Blue</param>
		/// <param name="a">Alpha</param>
		/// <returns>Color構造体</returns>
		public static Color IntToColor(int r, int g, int b, int a)
		{
			r = Mathf.Clamp(r, 0, 255);
			g = Mathf.Clamp(g, 0, 255);
			b = Mathf.Clamp(b, 0, 255);
			a = Mathf.Clamp(a, 0, 255);

			return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
		}

		/// <summary>
		/// グレースケール化
		/// </summary>
		/// <param name="original">通常カラー</param>
		/// <returns>グレースケール化したカラー</returns>
		public static Color GlayScale(Color original)
		{
			float gray = original.r * 0.3f + original.g * 0.59f + original.b * 0.11f;
			return new Color(gray, gray, gray, original.a);
		}

		/// <summary>
		/// グレースケール化
		/// </summary>
		/// <param name="original">通常カラー</param>
		/// <returns>グレースケール化したカラー</returns>
		public static Color GleyScale(Color original)
		{
			return GlayScale(original);
		}
	}

	public static class ColorUtil_Ex
	{
		public static Color GlayScale(this Color original)
		{
			return ColorUtil.GlayScale(original);
		}

		public static Color GreyScale(this Color original)
		{
			return GlayScale(original);
		}
	}

}
