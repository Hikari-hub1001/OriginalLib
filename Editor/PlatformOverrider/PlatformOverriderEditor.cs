#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OriginalLib.Behaviour.Platform
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(PlatformOverrider))]//拡張するクラスを指定
	public class PlatformOverriderEditor : Editor
	{

		private PlatformOverrider _target;
		private PlatformOverriderGroup pog => PlatformOverriderGroup.Instance;
		private OverriderSettings os;

		private RectTransform rect;

		private bool _isOptionOpen = false;
		private PlatformType _copyFrom;
		private PlatformType _copyTo;


		private void Awake()
		{
			_target = target as PlatformOverrider;
			rect = _target.GetComponent<RectTransform>();
			os = SetSetting(pog.m_SelectTab);
			OsFromRect();
			_target.isChange = false;
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
					RectFromOs();
					_target.isChange = false;
				}

				EditorGUI.BeginChangeCheck();

				EditorGUI.BeginDisabledGroup(UnityEngine.SystemInfo.deviceModel != UnityEngine.Device.SystemInfo.deviceModel);
				//Group�ݒ莞�̓{�^����\��
				PlatformOverriderGroupEditor.PratformButton(pog);
				EditorGUI.EndDisabledGroup();

				if (EditorGUI.EndChangeCheck())
				{
					// 値が変更されたときの処理
					pog.isChange = true;
					os = SetSetting(pog.m_SelectTab);
					RectFromOs();
				}
				else
				{
					os = SetSetting(pog.m_SelectTab);
					OsFromRect();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUI.BeginDisabledGroup(pog.m_SelectTab == PlatformType.Default);
				os.useDefault = EditorGUILayout.Toggle("UseDefault", os.useDefault);
				EditorGUI.EndDisabledGroup();
				if (EditorGUI.EndChangeCheck())
				{
					if (os.useDefault)
					{
						pog.isChange = true;
						os = SetSetting(PlatformType.Default);
						RectFromOs();
					}
					else
					{
						pog.isChange = true;
						os = SetSetting(pog.m_SelectTab);
						RectFromOs();
					}
				}
			}

			_isOptionOpen = EditorGUILayout.Foldout(_isOptionOpen, "Option", true);

			if (_isOptionOpen)
			{
				
				EditorGUI.indentLevel++;
				EditorGUILayout.LabelField("Copy======================================");
				_copyFrom = (PlatformType)EditorGUILayout.EnumPopup("From", _copyFrom);
				_copyTo = (PlatformType)EditorGUILayout.EnumPopup("To", _copyTo);

				if (GUILayout.Button("Copy Platform"))
				{
					if (_copyFrom != _copyTo)
					{
						var from = SetSetting(_copyFrom);
						var to = SetSetting(_copyTo);
						to.position		= from.position;
						to.anchorMin	= from.anchorMin;
						to.anchorMax	= from.anchorMax;
						to.pivot		= from.pivot;
						to.sizeDelta	= from.sizeDelta;
						to.offsetMin	= from.offsetMin;
						to.offsetMax	= from.offsetMax;
						to.rotation		= from.rotation;
						to.scale		= from.scale;
						to.activation	= from.activation;

						if (pog.m_SelectTab == _copyTo)
						{
							pog.isChange = true;
							os = SetSetting(pog.m_SelectTab);
							RectFromOs();
						}
						Debug.Log($"Copy from {_copyFrom} to {_copyTo}");
					}
				}

				EditorGUI.indentLevel--;
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

		/// <summary>
		/// 設定値に今のRecttransformを保存
		/// </summary>
		void OsFromRect()
		{
			if (!os.useDefault)
			{
				os.position = rect.anchoredPosition;
				os.anchorMin = rect.anchorMin;
				os.anchorMax = rect.anchorMax;
				os.pivot = rect.pivot;
				os.sizeDelta = rect.sizeDelta;
				os.offsetMin = rect.offsetMin;
				os.offsetMax = rect.offsetMax;
				os.rotation = rect.rotation;
				os.scale = rect.localScale;
				os.activation = _target.gameObject.activeSelf;
			}
			else
			{
				var setting = SetSetting(PlatformType.Default);
				setting.position = rect.anchoredPosition;
				setting.anchorMin = rect.anchorMin;
				setting.anchorMax = rect.anchorMax;
				setting.pivot = rect.pivot;
				setting.sizeDelta = rect.sizeDelta;
				setting.offsetMin = rect.offsetMin;
				setting.offsetMax = rect.offsetMax;
				setting.rotation = rect.rotation;
				setting.scale = rect.localScale;
				setting.activation = _target.gameObject.activeSelf;
			}
		}

		/// <summary>
		/// 設定値をRecttransformに反映させる
		/// </summary>
		void RectFromOs()
		{
			if (!os.useDefault)
			{
				rect.anchoredPosition = os.position;
				rect.anchorMin = os.anchorMin;
				rect.anchorMax = os.anchorMax;
				rect.pivot = os.pivot;
				rect.sizeDelta = os.sizeDelta;
				rect.offsetMin = os.offsetMin;
				rect.offsetMax = os.offsetMax;
				rect.rotation = os.rotation;
				rect.localScale = os.scale;
				_target.gameObject.SetActive(os.activation);
			}
			else
			{
				var setting = SetSetting(PlatformType.Default);
				rect.anchoredPosition = setting.position;
				rect.anchorMin = setting.anchorMin;
				rect.anchorMax = setting.anchorMax;
				rect.pivot = setting.pivot;
				rect.sizeDelta = setting.sizeDelta;
				rect.offsetMin = setting.offsetMin;
				rect.offsetMax = setting.offsetMax;
				rect.rotation = setting.rotation;
				rect.localScale = setting.scale;
				_target.gameObject.SetActive(setting.activation);
			}
		}

	}
}
#endif