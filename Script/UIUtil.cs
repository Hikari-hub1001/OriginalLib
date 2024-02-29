//============================================================================================================================
//
// このクラスはUIに関係するメソッドを用意しています。
// このファイルではUI全般に共通して使用できる関数を用意しています。
// またDotweenを使用した処理もありますので、Dotweenを各自インストールをお願いします。
//
// 関係のあるファイル
// ・UIUti_Image.cs
// ・UIUti_Text.cs
// ・UIUtil.cs
//
// このクラスで定義されるメソッドは全てstaticとなっているので、利用の際にインスタンスの生成を行う必要はありません。
//
//============================================================================================================================

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace OriginalLib
{
	public partial class UIUtil
	{
		/// <summary>
		/// キャンバスの新規作成
		/// </summary>
		/// <param name="canvasName">キャンバスオブジェクトの名称</param>
		/// <returns>作成したキャンバス</returns>
		public static GameObject CreateCanvas(string canvasName = "Canvas")
		{
			return CreateCanvas(RenderMode.ScreenSpaceOverlay, canvasName);
		}


		/// <summary>
		/// キャンバスの新規作成
		/// </summary>
		/// <param name="mode">レンダーモード</param>
		/// <param name="canvasName">キャンバスオブジェクトの名称</param>
		/// <returns>作成したキャンバス</returns>
		public static GameObject CreateCanvas(RenderMode mode, string canvasName = "Canvas")
		{
			GameObject canvas = new GameObject();
			canvas.AddComponent<Canvas>().renderMode = mode;
			canvas.AddComponent<CanvasScaler>();
			canvas.AddComponent<GraphicRaycaster>();
			canvas.name = canvasName;

			return canvas;
		}

		/// <summary>
		/// 子要素ごとUIをフェードアウトさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		public static void FadeOut(GameObject go)
		{
			var group = go.AddComponent<CanvasGroup>();
			group.DOFade(0, 1.0f)
				.OnComplete(() =>
				{
					Object.Destroy(group);
					go.SetActive(false);
				});
		}

		/// <summary>
		/// 子要素ごとUIをフェードアウトさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="time">フェードさせる時間</param>
		public static void FadeOut(GameObject go, float time)
		{
			var group = go.AddComponent<CanvasGroup>();
			group.DOFade(0, time)
				.OnComplete(() =>
				{
					Object.Destroy(group);
					go.SetActive(false);
				});
		}


		/// <summary>
		/// 子要素ごとUIをフェードインさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		public static void FadeIn(GameObject go)
		{
			go.SetActive(true);
			var group = go.AddComponent<CanvasGroup>();
			group.DOFade(1, 1.0f)
				.OnComplete(() =>
				{
					Object.Destroy(group);
				});
		}

		/// <summary>
		/// 子要素ごとUIをフェードインさせる
		/// </summary>
		/// <param name="go">フェードアウトさせるUI</param>
		/// <param name="time">フェードさせる時間</param>
		public static void FadeIn(GameObject go, float time)
		{
			go.SetActive(true);
			var group = go.AddComponent<CanvasGroup>();
			group.DOFade(1, time)
				.OnComplete(() =>
				{
					Object.Destroy(group);
				});
		}
	}
}