//============================================================================================================================
//
// このクラスはUIに関係するメソッドを用意しています。
// このファイルではUI全般に共通して使用できる関数を用意しています。
//
// 関係のあるファイル
// ・UIUti_Image.cs
// ・UIUti_Text.cs
// ・UIUtil.cs
//
// このクラスで定義されるメソッドは全てstaticとなっているので、利用の際にインスタンスの生成を行う必要はありません。
//
//============================================================================================================================

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN
//using DG.Tweening;
#endif
namespace OriginalLib
{
	public partial class UIUtil
	{
		/// <summary>
		/// キャンバスの新規作成
		/// ScreenSpace-Overlayで作成を行う
		/// </summary>
		/// <param name="canvasName">キャンバスオブジェクトの名称</param>
		/// <returns>作成したキャンバス</returns>
		public static Canvas CreateCanvas(string canvasName = "Canvas")
		{
			return CreateCanvas(RenderMode.ScreenSpaceOverlay, canvasName);
		}

		/// <summary>
		/// キャンバスの新規作成
		/// </summary>
		/// <param name="mode">レンダーモード</param>
		/// <param name="canvasName">キャンバスオブジェクトの名称</param>
		/// <returns>作成したキャンバス</returns>
		public static Canvas CreateCanvas(RenderMode mode, string canvasName = "Canvas")
		{
			return CreateCanvas(mode, null, canvasName);
		}


		/// <summary>
		/// キャンバスの新規作成
		/// </summary>
		/// <param name="mode">レンダーモード</param>
		/// <param name="canvasName">キャンバスオブジェクトの名称</param>
		/// <param name="parent">親</param>
		/// <returns>作成したキャンバス</returns>
		public static Canvas CreateCanvas(RenderMode mode, Transform parent, string canvasName = "Canvas")
		{
			GameObject canvasObj = new GameObject();
			Canvas canvas = canvasObj.AddComponent<Canvas>();
			canvas.renderMode = mode;
			canvasObj.AddComponent<CanvasScaler>();
			canvasObj.AddComponent<GraphicRaycaster>();
			canvasObj.name = canvasName;
			canvasObj.transform.parent = parent;

			return canvas;
		}

		/// <summary>
		/// 子要素ごとUIをフェードアウトさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		public static async Task FadeOut(GameObject go)
		{
			await FadeOut(go, 1.0f);
		}

		/// <summary>
		/// 子要素ごとUIをフェードアウトさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="time">フェードさせる時間</param>
		public static async Task FadeOut(GameObject go, float time)
		{
			await Fade(go, 0.0f, time);
		}

		/// <summary>
		/// 子要素ごとUIをフェードインさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		public static async Task FadeIn(GameObject go)
		{
			await FadeIn(go, 1.0f);
		}

		/// <summary>
		/// 子要素ごとUIをフェードインさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="time">フェードさせる時間</param>
		public static async Task FadeIn(GameObject go, float time)
		{
			await Fade(go, 1.0f, time);
		}

		/// <summary>
		/// 子要素ごとUIをフェードさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="alpha">目標のアルファ</param>
		/// <param name="time">フェードさせる時間</param>
		public static async Task Fade(GameObject go, float alpha, float time)
		{
			var group = go.GetComponent<CanvasGroup>();
			if (group == null) group = go.AddComponent<CanvasGroup>();

			float timePer = (alpha - group.alpha) / time;

			float elapsedTime = 0.0f;

			while (elapsedTime < time)
			{
				group.alpha += timePer * Time.deltaTime;
				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
		}

		/// <summary>
		/// 子要素ごとUIをフェードさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="alpha">目標のアルファ</param>
		public static async Task Fade(GameObject go, float alpha)
		{
			await Fade(go, alpha, 1.0f);
		}
	}

	/// <summary>
	/// 上記UIUtilクラスの静的メソッドを拡張メソッドに変換
	/// </summary>
	public static partial class UIUtil_Ex
	{
		public static async Task FadeIn(this CanvasGroup canvasGroup)
		{
			await UIUtil.FadeIn(canvasGroup.gameObject);
		}
		public static async void FadeIn(this CanvasGroup canvasGroup, float time)
		{
			await UIUtil.FadeIn(canvasGroup.gameObject, time);
		}
		public static async Task FadeOut(this CanvasGroup canvasGroup)
		{
			await UIUtil.FadeOut(canvasGroup.gameObject);
		}
		public static async Task FadeOut(this CanvasGroup canvasGroup, float time)
		{
			await UIUtil.FadeOut(canvasGroup.gameObject, time);
		}
		public static async Task Fade(this CanvasGroup canvasGroup, float alpha)
		{
			await UIUtil.Fade(canvasGroup.gameObject, alpha);
		}
		public static async Task Fade(this CanvasGroup canvasGroup, float alpha, float time)
		{
			await UIUtil.Fade(canvasGroup.gameObject, alpha, time);
		}
	}

}