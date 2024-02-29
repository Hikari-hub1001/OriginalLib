//============================================================================================================================
//
// このクラスはリストのシャッフルを行うクラスとなります。
// 複数のシャッフルを用意していますので、ゲームに合わせて利用してください。
//
// このクラスで定義されるメソッドは全てstaticとなっているので、利用の際にインスタンスの生成を行う必要はありません。
//
//============================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginalLib
{
	/// <summary>
	/// シャッフルクラス
	/// リストの中身をシャッフルし、返却する
	/// </summary>
	public class Shuffle
	{

		/// <summary>
		/// ディールシャッフル
		/// 配列を任意の数に分割したのちに、1つにまとめる
		/// ループを三回行うため少し重い
		/// 連番になることは無く、均等に分けられる
		/// リストが3未満はシャッフルできない
		/// </summary>
		/// <typeparam name="T">シャッフルするリストの型</typeparam>
		/// <param name="list">シャッフルするリスト</param>
		/// <param name="cnt">分割数(基本は7)</param>
		/// <returns>シャッフル後のリスト</returns>
		public static List<T> Shuffle_Deel<T>(List<T> list,int cnt=7)
		{
			//リスト配列が3未満の場合はシャッフルできないためキャンセル
			if (list.Count < 3) { return list; }

			List<List<T>> deckList = new();

			//7つの山を用意
			for(int i= 0; i < cnt; i++)
			{
				deckList.Add(new());
			}

			//元の山を指定の数に分割
			for(int i=0; i<list.Count;i++)
			{
				deckList[i%cnt].Add(list[i]);
			}

			List<T> result = new();

			//リザルトに順番に積んでいく
			while (0 < deckList.Count)
			{
				int index = Random.Range(0, deckList.Count);
				result.AddRange(deckList[index]);
				deckList.RemoveAt(index);
			}

			return result;
		}


		/// <summary>
		/// ランダムシャッフル
		/// 完全にランダムに積み上げていく
		/// ループ回数は1回なので比較的早い
		/// ただし乱数に左右されるため連番もあり得る
		/// リストが3未満はシャッフルできない
		/// </summary>
		/// <typeparam name="T">シャッフルするリストの型</typeparam>
		/// <param name="list">シャッフルするリスト</param>
		/// <returns>シャッフル後のリスト</returns>
		public static List<T> Shuffle_Random<T>(List<T> list)
		{
			//リスト配列が3未満の場合はシャッフルできないためキャンセル
			if (list.Count < 3) { return list; }
			
			List<T>result=new();

			while (0 < list.Count)
			{
				int index = Random.Range(0, list.Count);
				result.Add(list[index]);
				list.RemoveAt(index);
			}

			return result;
		}

		/// <summary>
		/// カットシャッフル
		/// 2か所の要素を入れ替えるのみのシンプルなシャッフル
		/// シャッフル回数次第だが一番早く完了する
		/// リストの大きさと回数によっては無駄が発生したり、
		/// 全く混ざらないこともあり
		/// リストが3未満はシャッフルできない
		/// </summary>
		/// <typeparam name="T">シャッフルするリストの型</typeparam>
		/// <param name="list">シャッフルするリスト</param>
		/// <param name="cnt">シャッフル回数(基本は20)</param>
		/// <returns>シャッフル後のリスト</returns>
		public static List<T> Shuffle_Cut<T>(List<T> list ,int cnt = 20)
		{
			//リスト配列が3未満の場合はシャッフルできないためキャンセル
			if (list.Count < 3) { return list; }

			for (int i = 0; i < cnt; i++)
			{
				//入れ替える任意の場所を乱数で取得
				int index1 = Random.Range(0, list.Count);
				int index2 = Random.Range(0, list.Count);
				//index1番目は避難
				T backUp = list[index1];
				//index2番目をindex1番目に入れる
				list[index2] = list[index1];
				//index1番目に避難した値を入れる
				list[index1] = backUp;
			}

			return list;
		}

		/// <summary>
		/// ヒンドゥーシャッフル（トランプ等で良く行うシャッフル）
		/// 真ん中の要素を後ろへ回すシャッフル
		/// 0番目の要素は変化しないが
		/// 処理速度は早め
		/// Cutシャッフルに似ているが、大きい移動が発生する
		/// リストが5未満はシャッフルできない
		/// </summary>
		/// <typeparam name="T">シャッフルするリストの型</typeparam>
		/// <param name="list">シャッフルするリスト</param>
		/// <param name="cnt">シャッフル回数(基本は10)</param>
		/// <returns>シャッフル後のリスト</returns>
		public static List<T> Shuffle_Hindu<T>(List<T> list,int cnt = 10)
		{
			//リスト配列が5未満の場合はシャッフルできないためキャンセル
			if (list.Count < 5) { return list; }

			for(int i = 0; i < cnt; i++)
			{
				//1～リストの後ろから3番目で乱数取得
				int index1 =  Random.Range(1, list.Count-3);
				//index+2～リスト-1の範囲で乱数取得
				int index2 = Random.Range(index1+1, list.Count-1);

				//index1番目からindex2番目までの要素を避難
				List<T> backUp = list.GetRange(index1, index2 - index1);
				//バックアップした要素を削除
				list.RemoveRange(index1, index2 - index1);
				//バックアップを後ろに追加
				list.AddRange(backUp);
			}

			return list;
		}

		/// <summary>
		/// ファローシャッフル
		/// リストを二つに分けてからランダムに上から並べていく
		/// 比較的早めに完了する
		/// 連番が発生する可能性は比較的高め
		/// </summary>
		/// <typeparam name="T">シャッフルするリストの型</typeparam>
		/// <param name="list">シャッフルするリスト</param>
		/// <returns>シャッフル後のリスト</returns>
		public static List<T> Shuffle_Fallow<T>(List<T> list)
		{
			//リスト配列が3未満の場合はシャッフルできないためキャンセル
			if (list.Count < 3) { return list; }

			List<T>[] decks = new List<T>[2];
			//listを二つに分割
			decks[0] = list.GetRange(0, list.Count / 2);
			decks[1] = list.GetRange(list.Count/2+1,list.Count-decks[0].Count);

			List<T> result = new();
			
			while(true)
			{
				//どちらかのリストから1つ結果リストに移動
				int index = Random.Range(0, 2);
				result.Add(decks[index][0]);
				decks[index].RemoveAt(0);

				//どちらかが使い切ったら
				if(decks[0].Count == 0 || decks[1].Count == 0)
				{
					//残りを結果に入れる
					result.AddRange(decks[0]);
					result.AddRange(decks[1]);
					break;
				}
			}

			return result;
		}
	}
}