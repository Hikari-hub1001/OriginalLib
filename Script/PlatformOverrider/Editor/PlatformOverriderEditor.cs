#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OriginalLib.Platform
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(PlatformOverrider))]//拡張するクラスを指定
	public class PlatformOverriderEditor : Editor
	{

		private PlatformOverrider _target;
		private PlatformOverriderGroup pog => PlatformOverriderGroup.Instance;
		private OverriderSettings os;

		private RectTransform rect;


		private void Awake()
		{
			_target = target as PlatformOverrider;
			rect = _target.GetComponent<RectTransform>();
		}

		/// <summary>
		/// InspectorのGUIを更新
		/// </summary>
		public override void OnInspectorGUI()
		{
			//　シリアライズオブジェクトの更新
			serializedObject.Update();

			if (pog == null)
			{
				//親にPOGがない場合
				EditorGUILayout.HelpBox("Please set the 'PlatformOverriderGroup' component within the scene.", MessageType.Info);
				return;
			}
			else
			{
				if (_target.isChange)
				{
					os = SetSetting(pog.m_SelectTab);
					Rect2Os();
					_target.isChange = false;
				}

				EditorGUI.BeginChangeCheck();
				//Group�ݒ莞�̓{�^����\��
				PlatformOverriderGroupEditor.PratformButton(pog);
				if (EditorGUI.EndChangeCheck())
				{
					// 値が変更されたときの処理
					pog.isChange = true;
					os = SetSetting(pog.m_SelectTab);
					Rect2Os();
				}
				else
				{
					os = SetSetting(pog.m_SelectTab);
					Os2Rect();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private OverriderSettings SetSetting(PlatformType platform)
		{
			Type type = _target.GetType();
			FieldInfo fi = type.GetField(platform.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);
			var val = (OverriderSettings)fi.GetValue(_target);
			return val;
		}


		void Os2Rect()
		{
			os.position = rect.anchoredPosition;
			os.anchorMin = rect.anchorMin;
			os.anchorMax = rect.anchorMax;
			os.pivot = rect.pivot;
			os.sizeDelta = rect.sizeDelta;
			os.offsetMin = rect.offsetMin;
			os.offsetMax = rect.offsetMax;
			os.rotation = rect.rotation.eulerAngles;
			os.scale = rect.localScale;
			os.activation = _target.gameObject.activeSelf;
		}

		void Rect2Os()
		{
			rect.anchoredPosition = os.position;
			rect.anchorMin = os.anchorMin;
			rect.anchorMax = os.anchorMax;
			rect.pivot = os.pivot;
			rect.sizeDelta = os.sizeDelta;
			rect.offsetMin = os.offsetMin;
			rect.offsetMax = os.offsetMax;
			rect.rotation = Quaternion.Euler(os.rotation);
			rect.localScale = os.scale;
			_target.gameObject.SetActive(os.activation);
		}

	}
}
#endif