using UnityEditor;

namespace OriginalLib.Behaviour
{
	[CustomEditor(typeof(SpriteNumberFloat))]
	public class SpriteNumberFloatEditor : SpriteNumberEditor<SpriteNumberFloat>
	{

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			base.OnInspectorGUI();
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