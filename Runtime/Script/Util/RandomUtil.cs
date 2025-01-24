using System;
using System.Collections.Generic;
using System.Linq;

namespace OriginalLib
{
	public class RandomUtil
	{
		/// <summary>
		/// 特定の範囲の数値を重複なしでランダムに抽出
		/// </summary>
		/// <param name="min">最小値</param>
		/// <param name="max">最大値</param>
		/// <param name="cnt">抽出個数</param>
		/// <returns>抽出結果</returns>
		/// <exception cref="ArgumentOutOfRangeException">抽出幅が対象範囲を上回る場合、又は0を下回る場合に発生</exception>
		public static int[] GetUniqueRandomNumbers(int min, int max, int cnt)
		{
			int[] ints = new int[max - min + 1];
			if (ints.Length < cnt || cnt < 0) throw new ArgumentOutOfRangeException("The variable must be greater than or equal to 0.", "variable");
			for (int i = 0; i < ints.Length; i++)
			{
				ints[i] = i;
			}
			return ints.Shuffle_Random()[0..cnt];
		}

		/// <summary>
		/// リストの要素をランダムに１つ抽出
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static T GetRandomElement<T>(List<T> list)
		{
			if (list == null) throw new ArgumentNullException();
			if (list.Count == 0) throw new ArgumentOutOfRangeException();

			int idx = UnityEngine.Random.Range(0, list.Count);
			return list[idx];
		}

		/// <summary>
		/// 配列の要素をランダムに１つ抽出
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static T GetRandomElement<T>(params T[] array)
		{
			if (array == null) throw new ArgumentNullException();
			if (array.Length == 0) throw new IndexOutOfRangeException();
			return GetRandomElement(array?.ToList());
		}

		/// <summary>
		/// 任意のenumからランダムに取得
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomEnum<T>() where T : Enum
		{
			T[] enums = (T[])Enum.GetValues(typeof(T));
			return enums[UnityEngine.Random.Range(0, enums.Length)];
		}

		/// <summary>
		/// ダイス関数
		/// Yまでの数値をX回振った合計を返却
		/// </summary>
		/// <param name="x">回数</param>
		/// <param name="y">最大値</param>
		/// <returns>合計</returns>
		public static int XDY(int x, int y)
		{
			int result = 0;
			for (int i = 0; i < x; i++)
			{
				result += UnityEngine.Random.Range(1, y + 1);
			}
			return result;
		}

		/// <summary>
		/// 1D6
		/// ダイス関数の基本
		/// </summary>
		/// <returns>1~6</returns>
		public static int OneDSix()
		{
			return XDY(1, 6);
		}

		/// <summary>
		/// 重みづけ乱数
		/// </summary>
		/// <param name="weighted">確率のリスト</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static int GetWeightedRandom(params float[] weighted)
		{
			if(weighted == null) throw new ArgumentNullException();
			if(weighted.Length == 0) throw new IndexOutOfRangeException();

			float total = 0.0f;
			Array.ForEach(weighted, (f) => total += f);
			float select = UnityEngine.Random.Range(0.0f,total);

			var currentWeight = 0f;
			for (var i = 0; i < weighted.Length; i++)
			{
				currentWeight += weighted[i];

				if (select < currentWeight)
				{
					return i;
				}
			}
			return weighted.Length - 1;
		}
	}

	public static partial class RandomUtil_Ex
	{
		public static T GetRandomElement<T>(this List<T> list)
		{
			return RandomUtil.GetRandomElement(list);
		}
		public static T GetRandomElement<T>(this T[] array)
		{
			return RandomUtil.GetRandomElement(array);
		}
	}
}