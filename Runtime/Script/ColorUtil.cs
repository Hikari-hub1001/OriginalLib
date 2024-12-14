using Codice.Client.BaseCommands.WkStatus.Printers;
using UnityEngine;

namespace OriginalLib
{
	public class ColorUtil
	{
		/// <summary>
		/// 0~255�ŃJ���[���\�z
		/// </summary>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Blue</param>
		/// <param name="a">Alpha</param>
		/// <returns>Color�\����</returns>
		public static Color IntToColor(int r, int g, int b, int a)
		{
			r = Mathf.Clamp(r, 0, 255);
			g = Mathf.Clamp(g, 0, 255);
			b = Mathf.Clamp(b, 0, 255);
			a = Mathf.Clamp(a, 0, 255);

			return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
		}

		/// <summary>
		/// �O���[�X�P�[����
		/// </summary>
		/// <param name="original">�ʏ�J���[</param>
		/// <returns>�O���[�X�P�[���������J���[</returns>
		public static Color GlayScale(Color original)
		{
			float gray = original.r * 0.3f + original.g * 0.59f + original.b * 0.11f;
			return new Color(gray, gray, gray, original.a);
		}

		/// <summary>
		/// �O���[�X�P�[����
		/// </summary>
		/// <param name="original">�ʏ�J���[</param>
		/// <returns>�O���[�X�P�[���������J���[</returns>
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
