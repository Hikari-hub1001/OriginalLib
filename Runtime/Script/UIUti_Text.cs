//============================================================================================================================
//
// このクラスはUIに関係するメソッドを用意しています。
// このファイルではTextまたはTextMeshProで使用できる関数を用意しています。
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
#if ENABLE_TMP
using TextComponent = TMPro.TextMeshProUGUI;
#else
using TextComponent = UnityEngine.UI.Text;
#endif

namespace OriginalLib
{
	public partial class UIUtil
	{
		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message)
		{
			return CreateText(message, new(0.0f, 0.0f, 0.0f), new(100.0f, 100.0f), Color.black, true);
		}

		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <param name="pos">座標</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message, Vector3 pos)
		{
			return CreateText(message, pos, new(100.0f, 100.0f), Color.black, true);
		}

		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent reateText(string message, Vector3 pos, Vector2 size)
		{
			return CreateText(message, pos, size, Color.black, true);
		}

		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="parent">親オブジェクト</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message, Vector3 pos, Vector2 size, Transform parent)
		{
			return CreateText(message, pos, size, default, true, parent);
		}

		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message, Vector3 pos, Vector2 size, Color col)
		{
			return CreateText(message, pos, size, col, true);
		}

		/// <summary>
		/// Text作成
		/// </summary>
		/// <param name="message">表示するテキスト</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <param name="active">活性状態</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message, Vector3 pos, Vector2 size, Color col, bool active)
		{
			//キャンバスの検索
			var canvas = HierarchyUtil.FindComponent<Canvas>();
			if (canvas == null)
			{
				//見つからない場合は生成
				canvas = CreateCanvas();
			}

			return CreateText(message, pos, size, col, active, canvas.transform);
		}


		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="message">表示する文字列</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <param name="active">活性状態</param>
		/// <param name="parent">親オブジェクト</param>
		/// <returns>作成したオブジェクト</returns>
		public static TextComponent CreateText(string message, Vector3 pos = default, Vector2 size = default, Color col = default, bool active = true, Transform parent = null)
		{
			GameObject go = new();

			go.transform.parent = parent;

			var txt = go.AddComponent<TextComponent>();

			txt.text = message;
			txt.color = col;

			var rt = go.AddComponent<RectTransform>();
			rt.anchoredPosition = pos;
			rt.sizeDelta = size;

			go.SetActive(active);

			return txt;
		}


		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, string message)
		{
			if (go == null) { return; }
			var text = go.GetComponent<TextComponent>();
			if (text == null) { return; }
			text.text = message;
		}

		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="objName">変更対象の名前</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, string objName, string message)
		{
			if (go == null) { return; }
			var obj = HierarchyUtil.FindObject(go, objName);
			if (obj == null) { return; }
			SetText(obj, message);
		}

		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, int message)
		{
			SetText(go, message.ToString());
		}
		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, float message)
		{
			SetText(go, message.ToString());
		}
		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, double message)
		{
			SetText(go, message.ToString());
		}
		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, bool message)
		{
			SetText(go, message.ToString());
		}
		/// <summary>
		/// テキスト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="message">変更後のテキスト</param>
		public static void SetText(GameObject go, char message)
		{
			SetText(go, message.ToString());
		}

		/// <summary>
		/// テキストカラー変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="col">変更後の色</param>
		public static void SetTextCol(GameObject go, Color col)
		{
			if (go == null) { return; }
			if (col == null) { return; }
			var text = go.GetComponent<TextComponent>();
			if (text != null)
			{
				text.color = col;
				return;
			}
		}

		/// <summary>
		/// テキストカラー変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="objName">変更対象の名前</param>
		/// <param name="col">変更後の色</param>
		public static void SetTextCol(GameObject go, string objName, Color col)
		{
			if (go == null) { return; }
			var obj = HierarchyUtil.FindObject(go, objName);
			if (obj == null) { return; }
			SetTextCol(obj, col);
		}
	}

	/// <summary>
	/// 上記UIUtilクラスの静的メソッドを拡張メソッドに変換
	/// </summary>
	public static partial class UIUtil_Ex
	{
		public static void SetText(this GameObject go, string message)
		{
			UIUtil.SetText(go, message);
		}
	}
}