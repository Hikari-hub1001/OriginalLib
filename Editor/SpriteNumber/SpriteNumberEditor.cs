using OriginalLib.Behaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OriginalLib
{
	public class SpriteNumberEditor<T> : Editor where T : Object
	{
		protected T _target;

		protected bool _isNumOpen = false;

		protected bool _isSignOpen = false;

		protected bool usePlus = true;
		protected bool useMinus = true;
		protected bool usePoint = true;


		protected void Awake()
		{
			_target = target as T;
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
			if (_isSignOpen && (usePlus || useMinus || usePoint))
			{
				EditorGUI.indentLevel++;
				if (usePlus) EditorGUILayout.PropertyField(serializedObject.FindProperty("_plusSprite"));
				if (useMinus) EditorGUILayout.PropertyField(serializedObject.FindProperty("_minusSprite"));
				if (usePoint) EditorGUILayout.PropertyField(serializedObject.FindProperty("_pointSprite"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"));

			//EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberPrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Align"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Spacing"));

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

	}
}