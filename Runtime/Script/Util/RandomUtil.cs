using System;
using System.Collections.Generic;
using System.Linq;

namespace OriginalLib
{
	public class RandomUtil
	{
		/// <summary>
		/// ����͈̔͂̐��l���d���Ȃ��Ń����_���ɒ��o
		/// </summary>
		/// <param name="min">�ŏ��l</param>
		/// <param name="max">�ő�l</param>
		/// <param name="cnt">���o��</param>
		/// <returns>���o����</returns>
		/// <exception cref="ArgumentOutOfRangeException">���o�����Ώ۔͈͂�����ꍇ�A����0�������ꍇ�ɔ���</exception>
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
		/// ���X�g�̗v�f�������_���ɂP���o
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
		/// �z��̗v�f�������_���ɂP���o
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
		/// �C�ӂ�enum���烉���_���Ɏ擾
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomEnum<T>() where T : Enum
		{
			T[] enums = (T[])Enum.GetValues(typeof(T));
			return enums[UnityEngine.Random.Range(0, enums.Length)];
		}

		/// <summary>
		/// �_�C�X�֐�
		/// Y�܂ł̐��l��X��U�������v��ԋp
		/// </summary>
		/// <param name="x">��</param>
		/// <param name="y">�ő�l</param>
		/// <returns>���v</returns>
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
		/// �_�C�X�֐��̊�{
		/// </summary>
		/// <returns>1~6</returns>
		public static int OneDSix()
		{
			return XDY(1, 6);
		}

		/// <summary>
		/// �d�݂Â�����
		/// </summary>
		/// <param name="weighted">�m���̃��X�g</param>
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