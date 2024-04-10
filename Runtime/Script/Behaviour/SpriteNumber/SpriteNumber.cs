using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Numerics;

namespace OriginalLib.Behaviour
{
	/// <summary>
	/// 重複アタッチ防止用クラス
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class SpriteNumberBase : MonoBehaviour { }

	[ExecuteAlways]
	public abstract class SpriteNumber<T> : SpriteNumberBase
	{
		#region 変数
		/// <summary>
		/// 数値表示に使用するImageオブジェクトのリスト
		/// </summary>
		[SerializeField, HideInInspector]
		protected List<Image> _numberImageList = new();

		[SerializeField] protected T Value;
		protected T _oldValue;

		/// <summary>
		/// 数値のスプライトの配列
		/// 要素数は0~9の10個で固定
		/// </summary>
		[SerializeField] protected Sprite _zero;
		[SerializeField] protected Sprite _one;
		[SerializeField] protected Sprite _two;
		[SerializeField] protected Sprite _three;
		[SerializeField] protected Sprite _four;
		[SerializeField] protected Sprite _five;
		[SerializeField] protected Sprite _six;
		[SerializeField] protected Sprite _seven;
		[SerializeField] protected Sprite _eight;
		[SerializeField] protected Sprite _nine;
		public Sprite[] NumberSprites
		{
			get
			{
				return new Sprite[] {
					_zero,
					_one,
					_two,
					_three,
					_four,
					_five,
					_six,
					_seven,
					_eight,
					_nine
				};
			}
		}
		/// <summary>
		/// マイナス符号を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _minusSprite;
		public Sprite MinusSprite => _minusSprite;
		/// <summary>
		/// プラス符号を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _plusSprite;
		public Sprite PlusSprite => _plusSprite;

		/// <summary>
		/// 少数点を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _pointSprite;
		public Sprite PointSprite => _pointSprite;

		/// <summary>
		/// 整数部の0埋め
		/// </summary>
		public bool IntZeroFill = false;

		/// <summary>
		/// 整数部の0埋め桁数
		/// デフォルトは4桁
		/// 最大はintの最大桁数に準拠
		/// </summary>
		[Range(1, 10)]
		public uint IntFillDigit = 4;

		/// <summary>
		/// 少数部の0埋め
		/// </summary>
		public bool FloatZeroFill = true;

		/// <summary>
		/// 少数表示の際に使用する有効桁数
		/// デフォルトは2桁	
		/// </summary>
		[Range(0, 5)]
		public uint FloatFillDigits = 2;

		/// <summary>
		/// 符号付き
		/// </summary>
		public Sign UseSign = Sign.MinusOnly;

		public enum Sign
		{
			NoSign,
			PlusOnly,
			MinusOnly,
			PlusAndMinus,
		}

		/// <summary>
		/// 数値表示に使うオブジェクトのプレファブ
		/// </summary>
		//public GameObject NumberPrefab;

		public enum NumberAlign
		{
			UpperLeft = 6,
			UpperCenter = 7,
			UpperRight = 8,
			MiddleLeft = 3,
			MiddleCenter = 4,
			MiddleRight = 5,
			LowerLeft = 0,
			LowerCenter = 1,
			LowerRight = 2,
		}
		/// <summary>
		/// 基準とする位置
		/// </summary>
		public NumberAlign Align = NumberAlign.MiddleCenter;

		/// <summary>
		/// 数字間の距離
		/// </summary>
		public float Spacing = 0.0f;

		/// <summary>
		/// オブジェクト名のデフォルト部分
		/// </summary>
		private readonly string OBJECT_DEFAULT_NAME = "NumberSprite_";

		private RectTransform rectTransform;
		#endregion

		#region MonoBehaviour関数
		protected void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}
		protected void OnEnable()
		{
			CreateImage(CreateStrNum());
		}
		protected void Update()
		{
			//if (!Application.isPlaying) return;
			if (EqualityComparer<T>.Default.Equals(Value, _oldValue)) return;
			CreateImage(CreateStrNum());
			_oldValue = Value;
		}
		private void OnDestroy()
		{
			foreach (var item in _numberImageList)
			{
				try
				{
					if (Application.isPlaying)
					{
						Destroy(item.gameObject);
					}
					else
					{
						DestroyImmediate(item.gameObject);
					}
				}
				catch { }
			}
		}
		#endregion

		#region SpriteNumber用関数
		protected abstract string CreateStrNum();
		protected void CreateImage(string strNum)
		{
			//座標設定用
			float offset = 0;
			int digit = CalcDigit(strNum) - 1;
			float maxHeight = 0;
			if (UseSign == Sign.NoSign)
			{
				var first = strNum.Substring(0, 1);
				if (first == "-" || first == "+")
				{
					strNum = strNum.Substring(1, strNum.Length - 1);
				}
			}
			else if (UseSign == Sign.PlusOnly)
			{
				var first = strNum.Substring(0, 1);
				if (first == "-")
				{
					strNum = strNum.Substring(1, strNum.Length - 1);
				}
				else if (first != "+")
				{
					strNum = "+" + strNum;
				}
			}
			else if (UseSign == Sign.MinusOnly)
			{
				var first = strNum.Substring(0, 1);
				if (first == "+")
				{
					strNum = strNum.Substring(1, strNum.Length - 1);
				}
			}
			else if (UseSign == Sign.PlusAndMinus)
			{
				var first = strNum.Substring(0, 1);
				if (first != "+" && first != "-")
				{
					strNum = "+" + strNum;
				}
			}

			//Nullになってしまった要素は消す
			_numberImageList.RemoveAll((o) => o == null);

			//Imageを設定していく
			for (int i = 0; i < strNum.Length; i++)
			{
				GameObject go;
				Image img;
				try
				{
					//既に作られていたら取得する
					img = _numberImageList[i];//Argmentが発生可能性
					go = _numberImageList[i].gameObject;//Missing,Nullが発生可能性
					go.SetActive(true);
				}
				catch (Exception ex) when (ex is ArgumentException)
				{
					//Argment->桁が増えたとき
					//Missing,Null->途中で桁オブジェクトが削除された時
					/*if (ex is MissingReferenceException || ex is NullReferenceException)
					{
						//途中で削除していたら対象の要素を削除
						_numberImageList.RemoveAt(i);
					}*/

					//go = Instantiate(NumberPrefab);
					go = new();
					go.transform.parent = transform;
					img = go.AddComponent<Image>();
					_numberImageList.Add(img);
				}

				if (int.TryParse(strNum.Substring(i, 1), out int n))//数値に変換できる場合
				{
					img.sprite = NumberSprites[n];
					go.name = $"{OBJECT_DEFAULT_NAME}{Math.Pow(10, digit).ToString("e2")}_{strNum.Substring(i, 1)}";
					digit--;
				}
				else//数値に変換できない場合
				{
					if (strNum.Substring(i, 1) == ".")
					{
						img.sprite = _pointSprite;
					}
					else if (strNum.Substring(i, 1) == "-" && (UseSign == Sign.MinusOnly || UseSign == Sign.PlusAndMinus))
					{
						img.sprite = _minusSprite;
					}
					else if (strNum.Substring(i, 1) == "+" && (UseSign == Sign.PlusOnly || UseSign == Sign.PlusAndMinus))
					{
						img.sprite = _plusSprite;
					}
					go.name = $"{OBJECT_DEFAULT_NAME}Sign_{strNum.Substring(i, 1)}";
				}

				RectTransform rect = go.GetComponent<RectTransform>();
				//サイズを設定
				if (img.sprite != null)
				{
					rect.sizeDelta = img.sprite.bounds.size * img.sprite.pixelsPerUnit;
					if (maxHeight < rect.sizeDelta.y)
					{
						maxHeight = rect.sizeDelta.y;
					}
				}
				else
				{
					rect.sizeDelta = new(100.0f, 100.0f);
				}

				//アンカー、ピボットを設定
				rect.anchorMin = rect.anchorMax = rect.pivot =
					new(((int)Align % 3) * 0.5f, ((int)Align / 3) * 0.5f);
				rect.anchoredPosition = new(offset, 0.0f);//左揃えで仮計算 中央、右はのちに再計算

				//座標設定
				offset += rect.sizeDelta.x + Spacing;
			}

			//不要なものは非表示にする
			for (int i = strNum.Length; i < _numberImageList.Count; i++)
			{
				_numberImageList[i].gameObject.SetActive(false);
				_numberImageList[i].gameObject.name = $"{OBJECT_DEFAULT_NAME}X";
			}

			if ((int)Align % 3 == 0) return;//左揃えはここで終了

			//rectTransform.sizeDelta = new(offset, maxHeight);

			//最後に加算したスペースと画像サイズ分だけ取り消す
			offset -= Spacing + ((RectTransform)_numberImageList[strNum.Length - 1].transform).sizeDelta.x;

			for (int i = 0; i < strNum.Length; i++)
			{
				var vec = ((RectTransform)_numberImageList[i].transform).anchoredPosition;
				if ((int)Align % 3 == 2)//右揃え
				{
					vec = new(vec.x - offset, vec.y);
				}
				else if ((int)Align % 3 == 1)//中央揃え
				{
					vec = new(vec.x - offset / 2, vec.y);
				}

				((RectTransform)_numberImageList[i].transform).anchoredPosition = vec;
			}
		}

		private int CalcDigit(string strNum)
		{
			if (strNum == null) throw new FormatException();
			if (BigInteger.TryParse(strNum, out BigInteger intValue))
			{
				return BigInteger.Abs(intValue).ToString().Length;
			}
			else if (float.TryParse(strNum, out float floatValue))
			{
				return BigInteger.Abs((BigInteger)floatValue).ToString().Length;
			}
			else if (double.TryParse(strNum, out double doubleValue))
			{
				return BigInteger.Abs((BigInteger)doubleValue).ToString().Length;
			}
			else
			{
				throw new FormatException();
			}
		}
		#endregion

		#region 画像設定
		/// <summary>
		/// 画像設定
		/// </summary>
		/// <param name="zero"></param>
		/// <param name="one"></param>
		/// <param name="two"></param>
		/// <param name="three"></param>
		/// <param name="four"></param>
		/// <param name="five"></param>
		/// <param name="six"></param>
		/// <param name="seven"></param>
		/// <param name="eight"></param>
		/// <param name="nine"></param>
		/// <param name="plus"></param>
		/// <param name="minus"></param>
		/// <param name="point"></param>
		public void SetSprite(Sprite zero, Sprite one, Sprite two,
			Sprite three, Sprite four, Sprite five, Sprite six,
			Sprite seven, Sprite eight, Sprite nine, Sprite plus, Sprite minus, Sprite point)
		{
			_zero = zero;
			_one = one;
			_two = two;
			_three = three;
			_four = four;
			_five = five;
			_six = six;
			_seven = seven;
			_eight = eight;
			_nine = nine;

			_minusSprite = minus;
			_plusSprite = plus;
			_pointSprite = point;
		}

		/// <summary>
		/// 画像設定
		/// </summary>
		/// <param name="zero"></param>
		/// <param name="one"></param>
		/// <param name="two"></param>
		/// <param name="three"></param>
		/// <param name="four"></param>
		/// <param name="five"></param>
		/// <param name="six"></param>
		/// <param name="seven"></param>
		/// <param name="eight"></param>
		/// <param name="nine"></param>
		/// <param name="plus"></param>
		/// <param name="minus"></param>
		public void SetSprite(Sprite zero, Sprite one, Sprite two,
			Sprite three, Sprite four, Sprite five, Sprite six,
			Sprite seven, Sprite eight, Sprite nine, Sprite plus, Sprite minus)
		{
			SetSprite(zero, one, two,
			 three, four, five, six,
			 seven, eight, nine, plus, minus, _pointSprite);
		}


		/// <summary>
		/// 0~9の画像設定
		/// </summary>
		/// <param name="zero"></param>
		/// <param name="one"></param>
		/// <param name="two"></param>
		/// <param name="three"></param>
		/// <param name="four"></param>
		/// <param name="five"></param>
		/// <param name="six"></param>
		/// <param name="seven"></param>
		/// <param name="eight"></param>
		/// <param name="nine"></param>
		public void SetSprite(Sprite zero, Sprite one, Sprite two,
			Sprite three, Sprite four, Sprite five, Sprite six,
			Sprite seven, Sprite eight, Sprite nine)
		{
			SetSprite(zero, one, two, three, four, five, six, seven, eight, nine, _plusSprite, _minusSprite);
		}

		/// <summary>
		/// 符号の画像設定
		/// </summary>
		/// <param name="plus"></param>
		/// <param name="minus"></param>
		public void SetSprite(Sprite plus, Sprite minus)
		{
			SetSprite(_zero, _one, _two, _three, _four, _five, _six
				, _seven, _eight, _nine, plus, minus);
		}

		/// <summary>
		/// 少数点の画像設定
		/// </summary>
		/// <param name="point"></param>
		public void SetSprite(Sprite point)
		{
			SetSprite(_zero, _one, _two, _three, _four, _five, _six
				, _seven, _eight, _nine, _plusSprite, _minusSprite, point);
		}
		#endregion

		#region エディター関数
#if UNITY_EDITOR
		private void OnValidate()
		{
			UnityEditor.EditorApplication.update += OnValidateImpl;
		}

		protected void OnValidateImpl()
		{
			//イベントハンドラを削除
			UnityEditor.EditorApplication.update -= OnValidateImpl;
			//自身が削除済みであればなにもしない
			if (this == null) return;

			if (!enabled) return;
			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}
			CreateImage(CreateStrNum());
		}

		private void Reset()
		{
			foreach (var item in _numberImageList)
			{
				try
				{
					DestroyImmediate(item.gameObject);
				}
				catch
				{

				}
			}
		}
#endif
		#endregion
	}
}