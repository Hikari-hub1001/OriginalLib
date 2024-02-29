using UnityEngine;
using OriginalLib.Behaviour;

namespace OriginalLib.Platform
{

	public enum PlatformType
	{
		Default,
		MobilePortrait,
		MobileLandscape,
		/*        TabletPortrait,
				TabletLandscape,*/
		MacOS,
		WindowsOS,
		PS5,
		PS4
	}

	//[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public sealed class PlatformOverriderGroup : Singleton<PlatformOverriderGroup>
	{

		/// <summary>
		/// �G�f�B�^�[����̂ݎQ�Ƃł���ϐ��Ȃ̂Œ���
		/// </summary>
		[HideInInspector]
		private PlatformType _SelectTab;

		public PlatformType m_SelectTab
		{
			get
			{
				return _SelectTab;
			}
		}


#if UNITY_IOS || UNITY_ANDROID
		private DeviceOrientation old;
#endif

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
		[HideInInspector]
		public bool isChange;
#endif
		protected override void Init()
		{
			Debug.Log($"{_SelectTab}�Ŏ��s���܂�");
		}

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
		private void Update()
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorWindow.focusedWindow != null)
			{
				string currentWindow = UnityEditor.EditorWindow.focusedWindow.titleContent.text;
				bool isSimulator = currentWindow.Contains("Simulator");

				if (isSimulator)
				{
					if (Screen.width < Screen.height)
					{
						_SelectTab = PlatformType.MobilePortrait;
					}
					else
					{
						_SelectTab = PlatformType.MobileLandscape;
					}
				}
			}
#elif UNITY_IOS || UNITY_ANDROID
			//���o�C���[���̍ۂ̂ݏc��������s��
			//�ύX�����ۂ̂ݍX�V���s��
			if (Input.deviceOrientation != old)
			{
				old = Input.deviceOrientation;
				if (old == DeviceOrientation.Portrait ||
					old == DeviceOrientation.PortraitUpsideDown)
				{
					_SelectTab = PlatformType.MobilePortrait;
				}
				else if (old == DeviceOrientation.LandscapeLeft ||
					old == DeviceOrientation.LandscapeRight)
				{
					_SelectTab = PlatformType.MobileLandscape;
				}
			}
#endif
			//Debug.Log(Application.platform);
			UpdateOverrider();
		}


		void UpdateOverrider()
		{
			//Platform�ύX����Ă��Ȃ���ΏI��
			if (!isChange) return;

			//PO�̎擾
			PlatformOverrider[] POs = FindObjectsOfType<PlatformOverrider>(true);

			foreach (PlatformOverrider po in POs)
			{
				Debug.Log(po.name);
				po.SetRectTransform(_SelectTab);
				po.isChange = true;
			}
			isChange = false;

			
		}
#endif


	}

}