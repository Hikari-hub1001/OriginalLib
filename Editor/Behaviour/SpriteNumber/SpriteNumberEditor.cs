using UnityEditor;
using UnityEditor.UI;
using UnityEngine;


namespace OriginalLib.Behaviour
{
	//[CustomEditor(typeof(SpriteNumber<>))]
	public class SpriteNumberEditor<T> : GraphicEditor where T : Object
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

			var numberAtlas = serializedObject.FindProperty("NumberAtlas");
			if (numberAtlas != null && numberAtlas.objectReferenceValue != null)
			{
				// ”wŒi•`‰æ
				if (Event.current.type == EventType.Repaint)
					background.Draw(r, false, false, false, false);

				GUI.DrawTexture(r, ((SpriteNumberAtlasSO)numberAtlas?.objectReferenceValue)?.GetAtlas(), ScaleMode.ScaleToFit);
			}
		}


	}

}