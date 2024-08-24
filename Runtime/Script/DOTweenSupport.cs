#if !DOTWEEN && UNITY_EDITOR
using System;
using UnityEngine;

namespace OriginalLib
{
	public class DOTweenSupport
	{
		internal static DOTweenSupport I
		{
			get
			{
				if (_i == null) { _i = new(); }
				return null;
			}
		}
		private static DOTweenSupport _i;

		public DOTweenSupport OnComplete(Action complete)
		{
			return Main();
		}

		internal static DOTweenSupport Main()
		{
#if UNITY_EDITOR
			Debug.LogError("You must install DOTween to use this feature.");
			return I;
#else
			throw new DOTweenSaportException("Install DOTween.");
#endif
		}
	}

	public static class DOTweenSupport_Ex
	{
		public static DOTweenSupport DOFade(this CanvasGroup canvasGroup, float target, float time)
		{
			return DOTweenSupport.Main();
		}
	}

	internal class DOTweenSaportException : Exception
	{
		internal DOTweenSaportException(string message) : base(message) { }
	}
}
#endif