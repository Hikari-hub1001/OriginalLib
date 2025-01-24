using System;
using UnityEngine;

namespace OriginalLib
{
	[Serializable]
	public class Integer : IComparable, IComparable<Int32>, IConvertible, IEquatable<Int32>, IFormattable
	{
		[SerializeField]
		private int _value;

		#region Constracter
		public Integer() { _value = default; }
		public Integer(int value) { _value = value; }
		#endregion

		#region Interface Method
		public int CompareTo(object obj)
		{
			return _value.CompareTo(obj);
		}

		public int CompareTo(int other)
		{
			return _value.CompareTo(other);
		}

		public bool Equals(int other)
		{
			return _value.Equals(other);
		}

		public TypeCode GetTypeCode()
		{
			return _value.GetTypeCode();
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToBoolean(provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToByte(provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToChar(provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToDateTime(provider);
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToDecimal(provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToDouble(provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToInt16(provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToInt32(provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToInt64(provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToSByte(provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToSingle(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return _value.ToString(provider);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return _value.ToString(format, formatProvider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)_value).ToType(conversionType, provider);
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToUInt16(provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToUInt32(provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)_value).ToUInt64(provider);
		}
		#endregion

		#region operator Method
		// 暗黙的なbool型へのキャスト
		public static implicit operator int(Integer myInt)
		{
			return myInt._value;
		}
		// bool型から暗黙的なキャスト
		public static implicit operator Integer(int value)
		{
			return new Integer(value);
		}
		public static bool operator true(Integer myInt) { return myInt._value != 0; }
		public static bool operator false(Integer myInt) { return myInt._value == 0; }

		public override string ToString()
		{
			return _value.ToString();
		}
		#endregion
	}

}