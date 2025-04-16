using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace OriginalLib.Behaviour
{
	[CreateAssetMenu(menuName = "OriginalLib/SpriteNumberAtlas")]
	public class SpriteNumberAtlasSO : ScriptableObject
	{
		//public Sprite[] sourceSprites;
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

		/// <summary>
		/// マイナス符号を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _minusSprite;
		/// <summary>
		/// プラス符号を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _plusSprite;

		/// <summary>
		/// 少数点を表すSprite
		/// </summary>
		[SerializeField] protected Sprite _pointSprite;

		/// <summary>
		/// 桁区切りに使用するSprite
		/// </summary>
		[SerializeField] protected Sprite _groupSeparatorSprite;

		private Sprite[] _numberSprites;
		private Sprite[] NumberSprites => _numberSprites ??= new Sprite[] {
			_zero, _one, _two, _three, _four,
			_five, _six, _seven, _eight, _nine,
			_plusSprite,_minusSprite,_pointSprite,_groupSeparatorSprite
		};

		private Texture2D generatedAtlas;
		[SerializeField, HideInInspector] private byte[] _atlasTextureData;

		public Sprite[] Sprites { get; private set; }


		private void BuildAtlas()
		{
			try
			{
				_numberSprites = null;

				if (NumberSprites == null || NumberSprites.Length == 0) return;

				int count = NumberSprites.Length;
				int[] fullWidths = new int[count];
				int[] fullHeights = new int[count];

				int totalWidth = 0;
				int maxHeight = 0;

				// 透過込みの見た目サイズ（rect）を元に計算
				for (int i = 0; i < count; i++)
				{
					var sprite = NumberSprites[i];
					if (sprite == null)
					{
						Debug.LogWarning($"[BuildAtlas] Sprite index {i} が null やで！");
						continue; // スキップ！
					}

					fullWidths[i] = Mathf.RoundToInt(sprite.rect.width);
					fullHeights[i] = Mathf.RoundToInt(sprite.rect.height);
					totalWidth += fullWidths[i];
					maxHeight = Mathf.Max(maxHeight, fullHeights[i]);
				}

				// 旧テクスチャ破棄
				if (generatedAtlas != null)
				{
					DestroyImmediate(generatedAtlas);
					generatedAtlas = null;
				}

				// Atlas初期化
				generatedAtlas = new Texture2D(totalWidth, maxHeight, TextureFormat.RGBA32, false);
				generatedAtlas.filterMode = FilterMode.Point;
				generatedAtlas.wrapMode = TextureWrapMode.Clamp;

				// 全透明で初期化
				Color[] clear = Enumerable.Repeat(new Color(0, 0, 0, 0), totalWidth * maxHeight).ToArray();
				generatedAtlas.SetPixels(clear);

				Sprites = new Sprite[count]; // 初期化！
				int offsetX = 0;

				for (int i = 0; i < count; i++)
				{
					var sprite = NumberSprites[i];
					if (sprite == null)
					{
						Debug.LogWarning($"[BuildAtlas] Sprite index {i} が null やで！");
						//uvs[i] = new Rect(
						//	(float)offsetX / totalWidth,
						//	0f,
						//	0f,
						//	0f
						//);
						continue; // スキップ！
					}
					var tex = sprite.texture;

					var rect = sprite.rect; // 見た目サイズ
					var texRect = sprite.textureRect; // 実画像位置

					int fullW = Mathf.RoundToInt(sprite.rect.width);
					int fullH = Mathf.RoundToInt(sprite.rect.height);

					int px = Mathf.FloorToInt(sprite.textureRect.x);
					int py = Mathf.FloorToInt(sprite.textureRect.y);
					int pw = Mathf.FloorToInt(sprite.textureRect.width);
					int ph = Mathf.FloorToInt(sprite.textureRect.height);

					var rawPixels = sprite.texture.GetPixels(px, py, pw, ph);
					Color[] fullPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), fullW * fullH).ToArray();

					// 🧠 ←ここが超重要
					int offsetInnerX = Mathf.RoundToInt(sprite.textureRect.x - sprite.rect.x);
					int offsetInnerY = Mathf.RoundToInt(sprite.textureRect.y - sprite.rect.y);

					for (int y = 0; y < ph; y++)
					{
						for (int x = 0; x < pw; x++)
						{
							int src = y * pw + x;
							int dst = (y + offsetInnerY) * fullW + (x + offsetInnerX);
							fullPixels[dst] = rawPixels[src];
						}
					}

					// Atlasに貼り付け
					generatedAtlas.SetPixels(offsetX, 0, fullW, fullH, fullPixels);

					// UV記録
					//uvs[i] = new Rect(
					//	(float)offsetX / totalWidth,
					//	0f,
					//	(float)fullW / totalWidth,
					//	(float)fullH / maxHeight
					//);
					// 最後に追加：Sprite作成
					Sprites[i] = Sprite.Create(
						generatedAtlas,
						new Rect(offsetX, 0, fullW, fullH),
						new Vector2(0.5f, 0.5f),
						100f
					);

					offsetX += fullW;
				}

				generatedAtlas.Apply();
				_atlasTextureData = generatedAtlas.EncodeToPNG(); // PNGバイナリで保存

#if UNITY_EDITOR && false
		// PNG出力＆スプライト分割（おまけ）
		string path = "Assets/GeneratedAtlas_TransparentSafe.png";
		System.IO.File.WriteAllBytes(path, generatedAtlas.EncodeToPNG());
		UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);

		UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
		if (importer != null)
		{
			importer.textureType = UnityEditor.TextureImporterType.Sprite;
			importer.spriteImportMode = UnityEditor.SpriteImportMode.Multiple;
			importer.alphaIsTransparency = true;
			importer.filterMode = FilterMode.Point;

			UnityEditor.SpriteMetaData[] metas = new UnityEditor.SpriteMetaData[sprites.Length];
			for (int i = 0; i < sprites.Length; i++)
			{
				Vector2[] uvs = sprites[i].uv;
				Rect uv = new Rect(
					uvs[0].x, uvs[0].y,
					uvs[2].x - uvs[0].x,
					uvs[2].y - uvs[0].y
				);
				Rect pixelRect = new Rect(
					Mathf.RoundToInt(uv.x * generatedAtlas.width),
					generatedAtlas.height - Mathf.RoundToInt(uv.y * generatedAtlas.height) - Mathf.RoundToInt(uv.height * generatedAtlas.height),
					Mathf.RoundToInt(uv.width * generatedAtlas.width),
					Mathf.RoundToInt(uv.height * generatedAtlas.height)
				);

				metas[i] = new UnityEditor.SpriteMetaData()
				{
					name = $"Sprite_{i}",
					rect = pixelRect,
					alignment = 9,
					pivot = new Vector2(0.5f, 0.5f)
				};
			}

			importer.spritesheet = metas;
			UnityEditor.EditorUtility.SetDirty(importer);
			importer.SaveAndReimport();
		}

		Debug.Log("✅ 透過保持・合体完了！出力: GeneratedAtlas_TransparentSafe.png");
#endif
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{

			}
		}

		private void OnDestroy()
		{
			DestroyImmediate(generatedAtlas);
			generatedAtlas = null;
		}
		public Texture2D GetAtlas()
		{
			if (generatedAtlas == null && _atlasTextureData != null && _atlasTextureData.Length > 0)
			{
				generatedAtlas = new Texture2D(2, 2, TextureFormat.RGBA32, false);
				generatedAtlas.LoadImage(_atlasTextureData); // PNG形式バイナリから復元
				generatedAtlas.filterMode = FilterMode.Point;
				generatedAtlas.wrapMode = TextureWrapMode.Clamp;
			}
			return generatedAtlas;
		}

	}
}