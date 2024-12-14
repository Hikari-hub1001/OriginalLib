#if !DOTWEEN && UNITY_EDITOR
using System;
using UnityEngine;

namespace OriginalLib
{
	internal class DOTweenSupport
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

		internal DOTweenSupport OnComplete(Action complete)
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

	internal static class DOTweenSupport_Ex
	{
		internal static DOTweenSupport DOFade(this CanvasGroup canvasGroup, float target, float time)
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