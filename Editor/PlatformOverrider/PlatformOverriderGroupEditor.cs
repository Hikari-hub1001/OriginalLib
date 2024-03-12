#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OriginalLib.Platform
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(PlatformOverriderGroup))]
	public class PlatformOverriderGroupEditor : Editor
	{
		private PlatformOverriderGroup _target;

		private void Awake()
		{
			_target = target as PlatformOverriderGroup;
		}

		/// <summary>
		/// InspectorÇÃGUIÇçXêV
		/// </summary>
		public override void OnInspectorGUI()
		{
			//å≥ÇÃInspectorïîï™Çï\é¶
			//base.OnInspectorGUI();
			EditorGUI.BeginChangeCheck();
			PratformButton(_target);
			if (EditorGUI.EndChangeCheck()){
				_target.isChange = true;
			}
		}


		public static void PratformButton(PlatformOverriderGroup target)
		{
			FieldInfo fieldInfo = typeof(PlatformOverriderGroup).GetField("_SelectTab", BindingFlags.Instance | BindingFlags.NonPublic);
			fieldInfo.SetValue(target, (PlatformType)GUILayout.SelectionGrid((int)target.m_SelectTab, Enum2Strings(), 3));
		}

		private static string[] Enum2Strings()
		{
			int enumCount = System.Enum.GetValues(typeof(PlatformType)).Length;
			string[] TAB_NAMES = new string[enumCount];
			for (int i = 0; i < enumCount; i++)
			{
				TAB_NAMES[i] = ((PlatformType)i).ToString();
			}
			return TAB_NAMES;
		}


	}

}
#endif