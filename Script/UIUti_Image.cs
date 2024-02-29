//============================================================================================================================
//
// このクラスはUIに関係するメソッドを用意しています。
// このファイルではImage、RawImage、Spriteで使用できる関数を用意しています。
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

namespace OriginalLib
{
	public partial class UIUtil
	{
		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite)
		{
			return CreateImage(sprite, new(0.0f, 0.0f, 0.0f), new(100.0f, 100.0f), Color.white, true);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos)
		{
			return CreateImage(sprite, pos, new(100.0f, 100.0f), Color.white, true);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos, Vector2 size)
		{
			return CreateImage(sprite, pos, size, Color.white, true);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="parent">親オブジェクト</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos, Vector2 size, GameObject parent)
		{
			return CreateImage(sprite, pos, size, default, true, parent);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos, Vector2 size, Color col)
		{
			return CreateImage(sprite, pos, size, col, true);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <param name="active">活性状態</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos, Vector2 size, Color col, bool active)
		{
			//キャンバスの検索
			var canvas = HierarchyUtil.FindComponent<Canvas>();
			if (canvas == null)
			{
				//見つからない場合は生成
				canvas = CreateCanvas();
			}
			return CreateImage(sprite, pos, size, col, active, canvas);
		}

		/// <summary>
		/// Image作成
		/// </summary>
		/// <param name="sprite">表示する画像</param>
		/// <param name="pos">座標</param>
		/// <param name="size">サイズ</param>
		/// <param name="col">色</param>
		/// <param name="active">活性状態</param>
		/// <param name="parent">親オブジェクト</param>
		/// <returns>作成したオブジェクト</returns>
		public static GameObject CreateImage(Sprite sprite, Vector3 pos = default, Vector2 size = default, Color col = default, bool active = true, GameObject parent = null)
		{
			GameObject go = new();

			go.transform.parent = parent?.transform;

			var img = go.AddComponent<Image>();
			img.sprite = sprite;
			img.color = col;

			var rt = go.GetComponent<RectTransform>();
			rt.anchoredPosition = pos;
			rt.sizeDelta = size;

			go.SetActive(active);

			return go;
		}


		/// <summary>
		/// スプライト変更
		/// </summary>
		/// <param name="go">変更するオブジェクト</param>
		/// <param name="sprite">変更後のsprite</param>
		public static void SetSprite(GameObject go, Sprite sprite)
		{
			if (go == null) { return; }
			var img = go.GetComponent<Image>();
			if (img == null) { return; }
			img.sprite = sprite;
		}

		/// <summary>
		/// スプライト変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="objName">変更対象の名前</param>
		/// <param name="sprite">変更後のsprite</param>
		public static void SetSprite(GameObject go, string objName, Sprite sprite)
		{
			if (go == null) { return; }
			var obj = HierarchyUtil.FindObject(go, objName);
			if (obj == null) { return; }
			SetSprite(obj, sprite);
		}

		/// <summary>
		/// テクスチャ変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="tex">変更後のtexture</param>
		public static void SetRawImage(GameObject go, Texture tex)
		{
			if (go == null) { return; }
			var img = go.GetComponent<RawImage>();
			if (img == null) { return; }
			img.texture = tex;
		}

		/// <summary>
		/// テクスチャ変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="objName">変更対象の名前</param>
		/// <param name="tex">変更後のtexture</param>
		public static void SetRawImage(GameObject go, string objName, Texture tex)
		{
			if (go == null) { return; }
			var obj = HierarchyUtil.FindObject(go, objName);
			if (obj == null) { return; }
			SetRawImage(obj, tex);
		}

		/// <summary>
		/// イメージの色変更
		/// </summary>
		/// <param name="go">変更対象</param>
		/// <param name="col">変更後の色</param>
		public static void SetImageCol(GameObject go, Color col)
		{
			if (go == null) { return; }
			if (col == null) { return; }
			var img = go.GetComponent<Image>();
			if (img != null)
			{
				img.color = col;
				return;
			}

			var rawImg = go.GetComponent<RawImage>();
			if (rawImg != null)
			{
				rawImg.color = col;
				return;
			}
		}

		/// <summary>
		/// イメージの色変更
		/// </summary>
		/// <param name="go">変更するオブジェクトの親</param>
		/// <param name="objName">変更対象の名前</param>
		/// <param name="col">変更後の色</param>
		public static void SetImageCol(GameObject go, string objName, Color col)
		{
			if (go == null) { return; }
			var obj = HierarchyUtil.FindObject(go, objName);
			if (obj == null) { return; }
			SetImageCol(obj, col);
		}
	}
}