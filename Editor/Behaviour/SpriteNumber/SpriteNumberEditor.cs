using OriginalLib.EditorSuport;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;


namespace OriginalLib.Behaviour
{
	//[CustomEditor(typeof(SpriteNumber<>))]
	public class SpriteNumberEditor<T> : GraphicEditor where T : SpriteNumbarBase
	{
		protected T _target;

		protected virtual bool useInteger => true;
		protected virtual bool useDecimal => true;

		protected virtual void Awake()
		{
			_target = target as T;
		}


		public override void OnInspectorGUI()
		{

			EditorGUI.BeginChangeCheck();

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberAtlas"));

			AppearanceControlsGUI();
			RaycastControlsGUI();
			MaskableControlsGUI();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("preferredHeight"));
			SetShowNativeSize(_target.NumberAtlas?.GetAtlas() != null, false);
			if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(EditorGUIUtility.labelWidth);
				if (GUILayout.Button(EditorGUIUtility.TrTextContent("Set Native Size", "Sets the size to match the content."), EditorStyles.miniButton))
				{
					//foreach (Graphic item in base.targets.Select((Object obj) => obj as Graphic))
					//{
					//	Undo.RecordObject(item.rectTransform, "Set Native Size");
					//	//item.SetNativeSize();
					//
					//	EditorUtility.SetDirty(item);
					//}
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

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

		public override bool HasPreviewGUI()
		{
			var numberAtlas = serializedObject.FindProperty("NumberAtlas");
			return _target != null && numberAtlas != null;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			SpriteNumbarBase spriteNumber = target as SpriteNumbarBase;
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
			SpriteNumbarBase spriteNumber = target as SpriteNumbarBase;

			// Image size Text
			string text = string.Format("AtlasTexture Size: {0}x{1}",
				Mathf.RoundToInt(Mathf.Abs(spriteNumber.mainTexture.width)),
				Mathf.RoundToInt(Mathf.Abs(spriteNumber.mainTexture.height)));

			return text;
		}

	}

}