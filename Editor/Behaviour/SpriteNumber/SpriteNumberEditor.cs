using OriginalLib.EditorSuport;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditor.UI;
using UnityEngine;


namespace OriginalLib.Behaviour
{
	public class SpriteNumberEditor<T> : GraphicEditor where T : SpriteNumberBase
	{
		protected T _target;

		protected virtual bool useInteger => true;
		protected virtual bool useDecimal => true;

		private bool spriteFoldout = true;
		private bool numberFoldout = true;
		private bool signFoldout = true;

		protected virtual void Awake()
		{
			_target = target as T;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			serializedObject.Update();

			spriteFoldout = EditorGUILayout.Foldout(spriteFoldout, "Sprites", true);
			if (spriteFoldout)
			{
				EditorGUI.indentLevel++;
				if (!_target.TextureCheck())
				{
					EditorGUILayout.HelpBox("Some sprites reference different textures. Please ensure all sprites use the same texture.", MessageType.Warning);
				}
				if (GUILayout.Button("Open SpriteAtlasBuilder"))
				{
					SpriteAtlasBuilderWindow.OpenWindow(_target);
				}
				if (_target.mainTexture != null)
				{
					if (GUILayout.Button("Auto Fill Sprites"))
					{
						AutoFillSpritesFromTexture();
					}
				}

				numberFoldout = EditorGUILayout.Foldout(numberFoldout, "Number", true);
				if (numberFoldout)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_zero"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_one"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_two"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_three"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_four"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_five"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_six"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_seven"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_eight"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_nine"));
					EditorGUI.indentLevel--;
				}

				signFoldout = EditorGUILayout.Foldout(signFoldout, "Sign", true);
				if (signFoldout)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_plus"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_minus"));
					if (useDecimal)
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_point"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_separator"));
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}


			EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("preferredHeight"));
			//SetShowNativeSize(_target.NumberAtlas?.GetAtlas() != null, false);
			if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(EditorGUIUtility.labelWidth);
				if (GUILayout.Button(EditorGUIUtility.TrTextContent("Set Native Size", "Sets the size to match the content."), EditorStyles.miniButton))
				{
					Undo.RecordObject(_target, "Set Native Size");
					serializedObject.FindProperty("preferredHeight").floatValue = _target.mainTexture.height;
				}

				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Spacing"));

			if (useInteger)
			{
				var padIntegerPart = serializedObject.FindProperty("PadIntegerPart");
				EditorGUILayout.PropertyField(padIntegerPart);

				if (padIntegerPart.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("IntegerPartDigits"));
					EditorGUI.indentLevel--;
				}
			}
			if (useDecimal)
			{
				var padDecimalPart = serializedObject.FindProperty("PadDecimalPart");
				EditorGUILayout.PropertyField(padDecimalPart);

				if (padDecimalPart.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DecimalPartDigits"));
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SignMode"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("GroupDigits"));

			EditorGUILayout.Space();
			AppearanceControlsGUI();
			RaycastControlsGUI();
			MaskableControlsGUI();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			SpriteNumberBase spriteNumber = target as SpriteNumberBase;
			Texture tex = spriteNumber?.mainTexture;
			if (spriteNumber == null || tex == null) return;
			Rect outer = new(
			0.0f,
			0.0f,
			tex.width,
			tex.height);
			SpriteDrawUtility.DrawSprite(tex, r, outer, new Rect(0.0f, 0.0f, 1.0f, 1.0f), spriteNumber.canvasRenderer.GetColor());
		}
		public override string GetInfoString()
		{
			SpriteNumberBase spriteNumber = target as SpriteNumberBase;

			string text = string.Format("AtlasTexture Size: {0}x{1}",
				Mathf.RoundToInt(Mathf.Abs(spriteNumber.mainTexture.width)),
				Mathf.RoundToInt(Mathf.Abs(spriteNumber.mainTexture.height)));

			return text;
		}

		private void AutoFillSpritesFromTexture()
		{
			string path = AssetDatabase.GetAssetPath(_target.mainTexture);
			TextureImporter pngImporter = (TextureImporter)AssetImporter.GetAtPath(path);
			var factory = new SpriteDataProviderFactories();
			factory.Init();
			var dataProvider = factory.GetSpriteEditorDataProviderFromObject(pngImporter);
			dataProvider.InitSpriteEditorDataProvider();
			var oldRects = dataProvider.GetSpriteRects();


			if (serializedObject.FindProperty("_zero").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("_0") || x.name.EndsWith("_zero", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_zero").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_one").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("1") || x.name.EndsWith("one", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_one").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_two").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("2") || x.name.EndsWith("two", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_two").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_three").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("3") || x.name.EndsWith("three", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_three").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_four").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("4") || x.name.EndsWith("four", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_four").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_five").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("5") || x.name.EndsWith("five", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_five").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_six").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("6") || x.name.EndsWith("six", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_six").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_seven").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("7") || x.name.EndsWith("seven", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_seven").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_eight").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("8") || x.name.EndsWith("eight", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_eight").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_nine").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("9") || x.name.EndsWith("nine", StringComparison.OrdinalIgnoreCase));
				serializedObject.FindProperty("_nine").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_plus").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("10") || x.name.EndsWith("plus", StringComparison.OrdinalIgnoreCase) || x.name.EndsWith("+"));
				serializedObject.FindProperty("_plus").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_minus").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("11") || x.name.EndsWith("minus", StringComparison.OrdinalIgnoreCase) || x.name.EndsWith("-"));
				serializedObject.FindProperty("_minus").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_point").objectReferenceValue as Sprite == null && useDecimal)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("12") || x.name.EndsWith("point", StringComparison.OrdinalIgnoreCase) || x.name.EndsWith("."));
				serializedObject.FindProperty("_point").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}
			if (serializedObject.FindProperty("_separator").objectReferenceValue as Sprite == null)
			{
				var sr = oldRects.FirstOrDefault(x => x.name.EndsWith("13") || x.name.EndsWith("separator", StringComparison.OrdinalIgnoreCase) || x.name.EndsWith(","));
				serializedObject.FindProperty("_separator").objectReferenceValue = GetSpriteByName(path, sr?.name);
			}

		}
		private Sprite GetSpriteByName(string assetPath, string spriteName)
		{
			var sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>();
			return sprites.FirstOrDefault(s => s.name == spriteName);
		}
	}

}