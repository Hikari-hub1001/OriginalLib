using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class SpriteNumbarBase : MaskableGraphic
	{
		public override Texture mainTexture => NumberAtlas?.GetAtlas() ?? s_WhiteTexture;

		/// <summary>
		/// 表示用アトラス画像
		/// </summary>
		public SpriteNumberAtlasSO NumberAtlas;
	}
	public abstract class SpriteNumber<T> : SpriteNumbarBase
	{
		/// <summary>
		/// 画像サイズ
		/// </summary>
		[Tooltip("このGraphicの高さを決める値（幅はスプライトに合わせて自動算出）")]
		public float preferredHeight = 100f;
		[Tooltip("スプライト同士の間隔（px単位・高さスケールに基づく）")]
		/// <summary>
		/// 数字間の距離
		/// </summary>
		public float Spacing = 0.0f;

		[SerializeField] public T Value;
		protected T _oldValue;

		/// <summary>
		/// 整数部の0埋め
		/// </summary>
		public bool PadIntegerPart = false;

		/// <summary>
		/// 整数部の0埋め桁数
		/// デフォルトは4桁
		/// 最大はintの最大桁数に準拠
		/// </summary>
		[Range(1, 10)]
		public uint IntegerPartDigits = 4;

		/// <summary>
		/// 少数部の0埋め
		/// </summary>
		public bool PadDecimalPart = false;

		/// <summary>
		/// 少数表示の際に使用する有効桁数
		/// デフォルトは2桁	
		/// </summary>
		[Range(0, 5)]
		public uint DecimalPartDigits = 2;

		/// <summary>
		/// 符号付き
		/// </summary>
		public Sign SignMode = Sign.MinusOnly;

		public enum Sign
		{
			NoSign,
			PlusOnly,
			MinusOnly,
			Both,
		}

		public bool GroupDigits = true;

		private DrivenRectTransformTracker tracker = new();

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (NumberAtlas == null || NumberAtlas.GetAtlas() == null || NumberAtlas.Sprites == null) return;

			float height = preferredHeight;
			float atlasRatio = NumberAtlas.GetAtlas().width / (float)NumberAtlas.GetAtlas().height;

			// 各スプライトの幅を計算しながら、合計幅＋スペースも計算
			List<float> spriteWidths = new();
			float totalWidth = 0f;

			string numberStr = FormatNumber();

			foreach (char c in numberStr)
			{
				var index = GetIndexForChar(c);
				if (index < 0 || index >= NumberAtlas.Sprites.Length) continue;
				if (NumberAtlas?.Sprites[index] == null) continue;

				float width = NumberAtlas.Sprites[index].rect.width;
				spriteWidths.Add(width);
				totalWidth += width;
			}

			// スペース追加（スプライトが2個以上あれば間に spacing * (count - 1) 入る）
			if (spriteWidths.Count > 1)
				totalWidth += Spacing * (spriteWidths.Count - 1);

			float offsetX = 0f;
			Vector2 pivotOffset = new Vector2(-rectTransform.pivot.x * rectTransform.sizeDelta.x, -rectTransform.pivot.y * preferredHeight);

			foreach (char c in numberStr)
			{
				var index = GetIndexForChar(c);
				if (index < 0 || index >= NumberAtlas.Sprites.Length) continue;
				if (NumberAtlas?.Sprites[index] == null) continue;

				var uv = NumberAtlas.Sprites[index].rect;
				if (uv == null) continue;
				if (uv.height == 0) continue;
				float aspect = uv.width / uv.height;
				float drawWidth = preferredHeight * aspect;

				Vector2 pos = new Vector2(offsetX, 0) + pivotOffset;
				Rect rect = new Rect(pos, new Vector2(drawWidth, preferredHeight));

				AddQuad(vh, rect, color, uv);
				offsetX += drawWidth + Spacing;
			}

		}

		int GetIndexForChar(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';
			else if (c == '+')
				return 10;
			else if (c == '-')
				return 11;
			else if (c == '.')
				return 12;
			else if (c == ',')
				return 13;
			return -1;
		}

		void AddQuad(VertexHelper vh, Rect rect, Color color, Rect uv)
		{
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			int startIndex = vh.currentVertCount;
			Vector2 texSize = new(mainTexture.width, mainTexture.height);
			vh.AddVert(new Vector3(min.x, min.y), color, new Vector2(uv.xMin, uv.yMin) / texSize);
			vh.AddVert(new Vector3(min.x, max.y), color, new Vector2(uv.xMin, uv.yMax) / texSize);
			vh.AddVert(new Vector3(max.x, max.y), color, new Vector2(uv.xMax, uv.yMax) / texSize);
			vh.AddVert(new Vector3(max.x, min.y), color, new Vector2(uv.xMax, uv.yMin) / texSize);

			vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
			vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
		}


		public virtual string FormatNumber()
		{
			if (Value == null) throw new ArgumentNullException(nameof(Value));

			// 型を判定して decimal に統一（精度優先）
			decimal value;

			switch (Value)
			{
				case int i: value = i; break;
				case long l: value = l; break;
				case float f: value = (decimal)f; break;
				case double d: value = (decimal)d; break;
				case decimal m: value = m; break;
				case uint ui: value = ui; break;
				case ulong ul: value = ul; break;
				case short s: value = s; break;
				case ushort us: value = us; break;
				case byte b: value = b; break;
				case sbyte sb: value = sb; break;
				default: throw new ArgumentException("未対応の数値型です", nameof(Value));
			}

			// 符号処理
			string sign = "";
			if (value < 0)
			{
				if (SignMode == Sign.MinusOnly || SignMode == Sign.Both)
					sign = "-";
				value = -value;
			}
			else if (value > 0)
			{
				if (SignMode == Sign.PlusOnly || SignMode == Sign.Both)
					sign = "+";
			}

			// 整数部と小数部を分離
			decimal integerPart = Math.Floor(value);
			decimal fractionalPart = value - integerPart;

			string integerStr;

			if (PadIntegerPart)
			{
				// 0埋め優先（桁数固定）
				integerStr = ((ulong)integerPart).ToString().PadLeft((int)IntegerPartDigits, '0');

				if (GroupDigits)
				{
					// 三桁区切りを自力で挿入（カンマ固定）
					integerStr = InsertGroupingSeparator(integerStr);
				}
			}
			else
			{
				// パディングなし
				integerStr = ((ulong)integerPart).ToString();

				if (GroupDigits)
				{
					// 三桁区切りを自力で挿入（カンマ固定）
					integerStr = InsertGroupingSeparator(integerStr);
				}
			}

			string decimalStr = "";
			if (DecimalPartDigits > 0)
			{
				if (PadDecimalPart)
				{
					// 固定小数桁（0埋め）
					decimalStr = fractionalPart.ToString("F" + DecimalPartDigits, CultureInfo.InvariantCulture);
					decimalStr = decimalStr.Substring(decimalStr.IndexOf('.')); // ".0001"
				}
				else
				{
					// 表示はするけど末尾の0は省略
					string format = "0." + new string('#', (int)DecimalPartDigits);
					decimalStr = value.ToString(format, CultureInfo.InvariantCulture);

					int dotIndex = decimalStr.IndexOf('.');
					if (dotIndex >= 0)
					{
						decimalStr = decimalStr.Substring(dotIndex);
					}
					else
					{
						decimalStr = ""; // 小数点が出なかった
					}
				}
			}

			return sign + integerStr + decimalStr;
		}
		protected virtual string InsertGroupingSeparator(string raw)
		{
			if (string.IsNullOrEmpty(raw) || raw.Length <= 3)
				return raw;

			var chars = new List<char>();
			int count = 0;

			for (int i = raw.Length - 1; i >= 0; i--)
			{
				chars.Insert(0, raw[i]);
				count++;
				if (count == 3 && i > 0)
				{
					chars.Insert(0, ','); // ← カンマで固定！
					count = 0;
				}
			}

			return new string(chars.ToArray());
		}

		private void ApplyRectLock()
		{
			tracker.Clear();

			// ✨ Stretchされてたら、強制的に固定Anchorにする（0,0）
			if (rectTransform.anchorMin != rectTransform.anchorMax)
			{
				Debug.LogWarning($"[SpriteNumber] Stretchは非対応です！固定Anchorに変更されました。", this);
				rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			}

			tracker.Add(this, rectTransform,
				DrivenTransformProperties.SizeDelta
			);

		}

		private void UpdateRectSize()
		{
			if (NumberAtlas == null || NumberAtlas.Sprites == null || NumberAtlas.Sprites.Length == 0) return;

			float totalWidth = 0f;
			int visibleCount = 0;

			string numberStr = FormatNumber();
			foreach (char c in numberStr)
			{
				var i = GetIndexForChar(c);
				if (i < 0 || i >= NumberAtlas.Sprites.Length) continue;

				var sprite = NumberAtlas.Sprites[i];
				if (sprite == null) continue;
				if (sprite.rect.height == 0) continue;

				float aspect = sprite.rect.width / sprite.rect.height;
				totalWidth += preferredHeight * aspect;
				visibleCount++;
			}

			if (visibleCount > 1)
				totalWidth += Spacing * (visibleCount - 1);

			rectTransform.sizeDelta = new Vector2(totalWidth, preferredHeight);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ApplyRectLock();
			UpdateRectSize();
		}
		protected void LateUpdate()
		{
			// 最終サイズ更新（念のため）
			UpdateRectSize();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			tracker.Clear();
		}

	}
}