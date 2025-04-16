using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	[CustomEditor(typeof(SpriteNumberAtlasSO))]
	public class SpriteNumberAtlasSOEditor : Editor
	{
		protected bool _isNumOpen = true;
		protected bool _isSignOpen = true;


		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			//base.OnInspectorGUI();
			_isNumOpen = EditorGUILayout.Foldout(_isNumOpen, "Number Sprites", true);

			if (_isNumOpen)
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
			_isSignOpen = EditorGUILayout.Foldout(_isSignOpen, "Another Sprite", true);
			if (_isSignOpen)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_plusSprite"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_minusSprite"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_pointSprite"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_groupSeparatorSprite"));
				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}

			SpriteNumberAtlasSO so = (SpriteNumberAtlasSO)target;

			if (GUILayout.Button("Rebuild Atlas"))
			{
				BuildAtlasSafely(so);
			}

			// ▼ 下にAtlas画像を表示
			if (so.GetAtlas() != null)
			{
				GUILayout.Space(10);
				GUILayout.Label("Preview of Generated Atlas", EditorStyles.boldLabel);

				float previewWidth = EditorGUIUtility.currentViewWidth - 40f;
				float ratio = (float)so.GetAtlas().height / so.GetAtlas().width;
				float previewHeight = previewWidth * ratio;

				Rect rect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
				//EditorGUI.DrawRect(rect, Color.yellow * 1.2f); // 背景グレー

				// 🚨 ここでガチで描画
				GUI.DrawTexture(rect, so.GetAtlas(), ScaleMode.ScaleToFit, true);

			}
		}

		private void BuildAtlasSafely(SpriteNumberAtlasSO so)
		{
			var sprites = typeof(SpriteNumberAtlasSO).GetProperty("NumberSprites", BindingFlags.NonPublic | BindingFlags.Instance);
			if (sprites.GetValue(so) == null) return;

			var changedAssets = new List<(string path, bool wasReadable)>();

			foreach (var sprite in (Sprite[])sprites.GetValue(so))
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

			var buildMethod = typeof(SpriteNumberAtlasSO).GetMethod("BuildAtlas", BindingFlags.NonPublic | BindingFlags.Instance);
			try
			{

				buildMethod?.Invoke(so, null);
			}
			finally
			{

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

#if UNITY_6000
				var all = GameObject.FindObjectsByType<SpriteNumbarBase>(FindObjectsSortMode.None);
#else
				var all = GameObject.FindObjectsOfType<SpriteNumbarBase>();
#endif
				foreach (var sn in all)
				{
					if (sn.NumberAtlas == target)
						sn.SetVerticesDirty();
				}
			}
			EditorUtility.SetDirty(so);
			Debug.Log("✅ Atlas successfully rebuilt.");
		}



	}
}