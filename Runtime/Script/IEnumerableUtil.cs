using System;
using System.Collections.Generic;
namespace OriginalLib
{
	public class IEnumerableUtil
	{
		public static T GetLastItem<T>(List<T> list)
		{
			if (list == null) throw new NullReferenceException();
			if (list.Count == 0) throw new ArgumentOutOfRangeException();
			return list[list.Count - 1];
		}

		public static T GetLastItem<T>(T[] array)
		{
			if (array == null) throw new NullReferenceException();
			if (array.Length == 0) throw new ArgumentOutOfRangeException();
			return array[array.Length - 1];
		}
	}

	public static partial class IEnumerableUtil_Ex
	{
		public static T LastItem<T>(this List<T> list)
		{
			if (list.Count == 0) throw new ArgumentOutOfRangeException();
			return list[list.Count - 1];
		}
		public static T LastItem<T>(this T[] array)
		{
			if (array.Length == 0) throw new ArgumentOutOfRangeException();
			return array[array.Length - 1];
		}
	}

}
