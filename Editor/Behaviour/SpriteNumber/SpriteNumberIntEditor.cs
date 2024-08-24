using UnityEditor;

namespace OriginalLib.Behaviour
{
	[CustomEditor(typeof(SpriteNumberInt))]
	public class SpriteNumberIntEditor : SpriteNumberEditor<SpriteNumberInt>
	{

		protected new void Awake()
		{
			base.Awake();
			usePoint = false;
		}

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

			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseSign"));


			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

	}
}