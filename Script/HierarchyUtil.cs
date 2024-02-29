//============================================================================================================================
//
// このクラスはHierarchyに関係するメソッドを用意しています。
//
// このクラスで定義されるメソッドは全てstaticとなっているので、利用の際にインスタンスの生成を行う必要はありません。
//
//============================================================================================================================
using UnityEngine;

namespace OriginalLib
{
	public class HierarchyUtil
	{
		/// <summary>
		/// オブジェクトの検索
		/// </summary>
		/// <param name="name">検索名</param>
		/// <returns>検索結果（複数ある場合は初めに見つかったオブジェクト）</returns>
		public static GameObject FindObject(string name)
		{
			return GameObject.Find(name);
		}

		/// <summary>
		/// オブジェクト検索
		/// </summary>
		/// <param name="go">検索をかけるオブジェクト</param>
		/// <param name="name">検索名</param>
		/// <returns>検索結果（複数ある場合は初めに見つかったオブジェクト）</returns>
		public static GameObject FindObject(GameObject go, string name)
		{
			if (go == null)
			{
				return null;
			}

			GameObject result = null;
			foreach(Transform child in go.transform)
			{
				result = child.Find(name)?.gameObject;

				if(result == null)
				{
					result = FindObject(child.gameObject,name);
				}

				if(result != null) return result;
			}

			return null;
		}

		/// <summary>
		/// コンポーネントからオブジェクトを検索する
		/// </summary>
		/// <param name="type">検索するコンポーネント</param>
		/// <returns>コンポーネントを持ったオブジェクト</returns>
		public static GameObject FindComponent(System.Type type)
		{
			var o = Object.FindObjectOfType(type);
			if (o == null) return null;
			return FindObject(o.name);
		}

		/// <summary>
		/// コンポーネントからオブジェクトを検索する
		/// </summary>
		/// <typeparam name="T">検索するコンポーネント</typeparam>
		/// <returns>コンポーネントを持ったオブジェクト</returns>
		public static GameObject FindComponent<T>()
		{
			return FindComponent(typeof(T));
		}

		/// <summary>
		/// シーン内の対象となるオブジェクトに同じ処理を実行する
		/// かなり重い処理なので気を付けること
		/// </summary>
		/// <typeparam name="T">対象とするコンポーネント</typeparam>
		/// <param name="action">実行したい処理</param>
		/// <param name="includeInactive">非活性のオブジェクトに実行するか（基本は実行する）</param>
		public static void TraversalAction<T>(System.Action<T> action, bool includeInactive = true) where T : Component
		{
			var objects = GameObject.FindObjectsOfType<T>(includeInactive);
			foreach (var obj in objects)
			{
				action?.Invoke(obj);
			}
		}

		/// <summary>
		/// 対象オブジェクトを含む子要素に同じ処理を実行する
		/// かなり重い処理なので気を付けること
		/// </summary>
		/// <typeparam name="T">対象とするコンポーネント</typeparam>
		/// <param name="action">実行したい処理</param>
		/// <param name="includeInactive">非活性のオブジェクトに実行するか（基本は実行する）</param>
		public static void TraversalAction<T>(Transform parent, System.Action<T> action, bool includeInactive = true) where T : Component
		{
			//子要素をチェックしていく
			foreach (Transform child in parent)
			{
				//対象が非活性かつ非活性オブジェクトに実行しない場合のみcontinue
				if (!(child.gameObject.activeSelf && includeInactive)) continue;

				T component = child.gameObject.GetComponent<T>();
				if (component != null)
				{
					//コンポーネントを所持している場合実行する
					action?.Invoke(component);
				}

				//子要素を持つ場合のみ再帰的に実行する
				//上で実行したactionでオブジェクト破棄が行われた際に
				//NullReferenceExceptionが発生する可能性を考慮する
				if (child?.childCount > 0)
				{
					TraversalAction<T>(child, action, includeInactive);
				}
			}
		}
	}
}