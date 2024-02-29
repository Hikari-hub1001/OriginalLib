//============================================================================================================================
//
// このクラスはシングルトンのコンポーネントとなります。
// 任意のクラスに継承することでのみ使用可能です。
// ２つ以上のインスタンスが生成される際、あとから生成したインスタンスがゲームオブジェクトごと破棄されます。
// シーン遷移が行われるタイミングで破棄されます。
// シーンをまたいで保持したい場合は「Singleton_DontDestroy」クラスを使用してください。
//
// 使い方
// TestSingletonクラスを例とします
// public class TestSingleton : Singleton <TestSingleton>
//
//
// Awakeメソッドは基本的に実装せずにInitメソッドを使用してください。
// InitメソッドはAwakeメソッドで呼ばれており
// Awakeメソッドと同等の働きをします。
//
//============================================================================================================================

using UnityEngine;

namespace OriginalLib.Behaviour
{

	/// <summary>
	/// シングルトンベースクラス
	/// </summary>
	/// <typeparam name="T">シングルトンにするクラス</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{

		public static T Instance
		{
			private set;
			get;
		}

		/// <summary>
		/// インスタンスが作成済みかチェックする
		/// </summary>
		/// <returns></returns>
		public static bool IsValid() { return Instance != null; }


		protected void Awake()
		{
			if (Instance == null)
			{
				Instance = this.GetComponent<T>();
				Init();
			}
			else if (Instance != this)
			{
				Debug.Log($"2個目のインスタンスが作成されました。破棄します。<br>{this}");
				Destroy(gameObject);
			}
		}

		/// <summary>
		/// Awakeで処理したい初期化処理
		/// </summary>
		protected virtual void Init() { }

		/// <summary>
		/// オブジェクト破棄時にインスタンスも破棄する
		/// </summary>
		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}
	}
}