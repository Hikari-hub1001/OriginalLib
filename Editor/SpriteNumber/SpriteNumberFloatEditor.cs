using UnityEditor;

namespace OriginalLib.Behaviour
{
	[CustomEditor(typeof(SpriteNumberFloat))]
	public class SpriteNumberFloatEditor : Editor
	{

		private SpriteNumberFloat _target;

		private bool _isNumOpen = false;

		private bool _isSignOpen = false;

		private void Awake()
		{
			_target = target as SpriteNumberFloat;
		}


		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			serializedObject.Update();

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
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"));

			//EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberPrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Align"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Spacing"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("IntZeroFill"));

			if (_target.IntZeroFill)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("IntFillDigit"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("FloatZeroFill"));

			if (_target.FloatZeroFill)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("FloatFillDigits"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseSign"));

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}
	}
}