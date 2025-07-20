using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	public class SpriteAtlasBuilderWindow : EditorWindow
	{
		private string exportPath => exportDir + "/" + exportFileName + ".png";
		private string exportDir = "Assets";
		private string exportFileName = "SpriteNumber";
		private Sprite[] numberSprites = new Sprite[14];

		private bool numberFoldout = true;
		private bool signFoldout = true;

		private Vector2 scrollPos;

		private SpriteNumberBase snBase;

		[MenuItem("Tools/Sprite Atlas Builder")]
		public static SpriteAtlasBuilderWindow OpenWindow()
		{
			var window = GetWindow<SpriteAtlasBuilderWindow>("Sprite Atlas Builder");
			window.Show();
			window.exportDir = "Assets";
			window.exportFileName = "SpriteNumber";
			window.snBase = null;
			for (int i = 0; i < window.numberSprites.Length; i++)
				window.numberSprites[i] = null;
			return window;
		}

		public static void OpenWindow(SpriteNumberBase spriteNumber)
		{
			var window = OpenWindow();
			window.snBase = spriteNumber;
			string assetPath = AssetDatabase.GetAssetPath(spriteNumber.mainTexture);
			if (string.IsNullOrEmpty(assetPath)) return;
			window.exportDir = Path.GetDirectoryName(assetPath).Replace("\\", "/");
			window.exportFileName = Path.GetFileNameWithoutExtension(assetPath);
			for (int i = 0; i < window.numberSprites.Length; i++)
			{
				var sp = spriteNumber.GetSprite(i);
				if (sp != null)
				{
					window.numberSprites[i] = sp;
				}
				else
				{
					window.numberSprites[i] = window.GetSpriteByName(assetPath, $"{window.exportFileName}_{i}");
				}
			}
		}

		private void OnGUI()
		{
			exportDir = EditorGUILayout.TextField("Export Directory", exportDir);

			exportFileName = EditorGUILayout.TextField("Export FileName", exportFileName);

			EditorGUILayout.Space();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			numberFoldout = EditorGUILayout.Foldout(numberFoldout, "Number", true);
			if (numberFoldout)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < 10; i++)
				{
					numberSprites[i] = (Sprite)EditorGUILayout.ObjectField(i.ToString(), numberSprites[i], typeof(Sprite), false, GUILayout.Height(18));
				}
				EditorGUI.indentLevel--;
			}

			signFoldout = EditorGUILayout.Foldout(signFoldout, "Sign", true);
			if (signFoldout)
			{
				EditorGUI.indentLevel++;
				numberSprites[10] = (Sprite)EditorGUILayout.ObjectField("plus", numberSprites[10], typeof(Sprite), false, GUILayout.Height(18));
				numberSprites[11] = (Sprite)EditorGUILayout.ObjectField("minus", numberSprites[11], typeof(Sprite), false, GUILayout.Height(18));
				numberSprites[12] = (Sprite)EditorGUILayout.ObjectField("point", numberSprites[12], typeof(Sprite), false, GUILayout.Height(18));
				numberSprites[13] = (Sprite)EditorGUILayout.ObjectField("separator", numberSprites[13], typeof(Sprite), false, GUILayout.Height(18));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();

			if (GUILayout.Button("Export"))
			{
				BuildAndExportAtlas();
			}
		}

		private void BuildAndExportAtlas()
		{
			//=========================================================================
			// Read/Writeを許可
			//=========================================================================
			var changedAssets = new List<(string path, bool wasReadable)>();

			foreach (var sprite in numberSprites)
			{
				if (sprite == null) continue;

				string path = AssetDatabase.GetAssetPath(sprite.texture);
				var importer = AssetImporter.GetAtPath(path) as TextureImporter;

				if (importer != null)
				{
					bool wasReadable = importer.isReadable;
					if (!wasReadable)
					{
						importer.isReadable = true;
						importer.SaveAndReimport();
					}
					changedAssets.Add((path, wasReadable));
				}
			}

			//=========================================================================
			// 合成
			//=========================================================================
			Texture2D generatedAtlas = null;
			try
			{
				int count = numberSprites.Length;
				int totalWidth = 0;
				int maxHeight = 0;

				// 透過込みの見た目サイズ（rect）を元に計算
				foreach (var sp in numberSprites)
				{
					if (sp == null) continue;
					totalWidth += Mathf.CeilToInt(sp.rect.width);
					maxHeight = Mathf.Max(maxHeight, Mathf.CeilToInt(sp.rect.height));
				}

				// Atlas初期化
				generatedAtlas = new Texture2D(totalWidth, maxHeight, TextureFormat.RGBA32, false);
				generatedAtlas.filterMode = FilterMode.Point;
				generatedAtlas.wrapMode = TextureWrapMode.Clamp;

				// 全透明で初期化
				Color[] clear = Enumerable.Repeat(Color.clear, totalWidth * maxHeight).ToArray();
				generatedAtlas.SetPixels(clear);

				int offsetX = 0;

				List<SpriteRect> newRects = new();
				for (int i = 0; i < count; i++)
				{
					var sprite = numberSprites[i];
					if (sprite == null)
					{
						//Debug.LogWarning($"[BuildAtlas] Sprite index {i} が null やで！");
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
					Color[] fullPixels = Enumerable.Repeat(Color.clear, fullW * fullH).ToArray();

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

					//=========================================================================
					// スプライト情報の構築
					//=========================================================================
					newRects.Add(new SpriteRect
					{
						name = $"{exportFileName}_{i}",
						rect = new Rect(offsetX, 0, fullW, fullH),
						pivot = new Vector2(0.5f, 0.5f),
						alignment = SpriteAlignment.Center
					});
					offsetX += fullW;
				}
				generatedAtlas.Apply();

				File.WriteAllBytes(exportPath, generatedAtlas.EncodeToPNG());
				if (generatedAtlas != null)
				{
					DestroyImmediate(generatedAtlas);
					generatedAtlas = null;
				}
				AssetDatabase.ImportAsset(exportPath, ImportAssetOptions.ForceUpdate);
				TextureImporter pngImporter = (TextureImporter)AssetImporter.GetAtPath(exportPath);
				pngImporter.textureType = TextureImporterType.Sprite;
				pngImporter.spriteImportMode = SpriteImportMode.Multiple;
				var factory = new SpriteDataProviderFactories();
				factory.Init();
				var dataProvider = factory.GetSpriteEditorDataProviderFromObject(pngImporter);
				dataProvider.InitSpriteEditorDataProvider();
				//旧Sprite情報を取得
				var oldRects = dataProvider.GetSpriteRects().ToList();
				//新旧比較
				for (int i = 0; i < newRects.Count; i++)
				{
					//旧があればID保持の為、新リストを旧リストに上書き（Rectは新の物に）
					int idx = oldRects.FindIndex((item) => item.name == newRects[i].name);
					if (idx != -1)
					{
						var rect = newRects[i].rect;
						newRects[i] = oldRects[idx];
						newRects[i].rect = rect;
					}
				}

				dataProvider.SetSpriteRects(newRects.ToArray());
				dataProvider.Apply();

				pngImporter.SaveAndReimport();
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
				AssetDatabase.ImportAsset(exportPath, ImportAssetOptions.ForceUpdate);

				if (snBase != null)
				{
					for (int i = 0; i < numberSprites.Length; i++)
					{
						snBase.SetSprite(i, GetSpriteByName(exportPath, $"{exportFileName}_{i}"));
					}
				}
				for (int i = 0; i < numberSprites.Length; i++)
				{
					numberSprites[i] = GetSpriteByName(exportPath, $"{exportFileName}_{i}");
				}
				Debug.Log($"✅ 透過保持・合体完了！出力: {exportPath}");
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				if (generatedAtlas != null)
				{
					DestroyImmediate(generatedAtlas);
					generatedAtlas = null;
				}
				foreach (var (path, wasReadable) in changedAssets)
				{
					if (!wasReadable)
					{
						var importer = AssetImporter.GetAtPath(path) as TextureImporter;
						if (importer != null)
						{
							importer.isReadable = false;
							importer.SaveAndReimport();
						}
					}
				}
			}
		}

		private Sprite GetSpriteByName(string assetPath, string spriteName)
		{
			var sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>();
			return sprites.FirstOrDefault(s => s.name == spriteName);
		}
	}
}