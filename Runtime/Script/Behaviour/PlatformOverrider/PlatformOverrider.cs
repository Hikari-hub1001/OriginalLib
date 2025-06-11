using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OriginalLib.Behaviour.Platform
{


	[Serializable]
	public class OverriderSettings
	{
		[SerializeField, HideInInspector]
		public bool useDefault = false;

		[SerializeField]
		public Vector3 position;

		[SerializeField]
		public Vector2 sizeDelta = new(100.0f, 100.0f);
		[SerializeField]
		public Vector2 offsetMin;
		[SerializeField]
		public Vector2 offsetMax;

		[SerializeField]
		public Vector2 anchorMin = new(0.5f, 0.5f);
		[SerializeField]
		public Vector2 anchorMax = new(0.5f, 0.5f);

		[SerializeField]
		public Vector2 pivot = new(0.5f, 0.5f);

		[SerializeField]
		public Quaternion rotation = Quaternion.identity;

		[SerializeField]
		public Vector3 scale = new(1.0f, 1.0f, 1.0f);

		[SerializeField]
		public bool activation = true;

		[SerializeField]
		public Sprite sprite;
		[SerializeField]
		public Texture texture;
		[SerializeField]
		public string text;

		public OverriderSettings() { }
		public OverriderSettings(bool useDef)
		{
			useDefault = useDef;
		}
	}

	[DisallowMultipleComponent]
	public sealed class PlatformOverrider : MonoBehaviour
	{
		#region 変数宣言


		[SerializeField, HideInInspector] private OverriderSettings Default = new(false);
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
		[SerializeField, HideInInspector] private OverriderSettings MobilePortrait = new(false);
		[SerializeField, HideInInspector] private OverriderSettings MobileLandscape = new(false);
		//[SerializeField, HideInInspector] private OverriderSettings TabletPortrait;
		//[SerializeField, HideInInspector] private OverriderSettings TabletLandscape;
		[HideInInspector]
		public bool isChange;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		[SerializeField, HideInInspector] private OverriderSettings MacOS = new(true);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		[SerializeField, HideInInspector] private OverriderSettings WindowsOS = new(true);
#endif
#if UNITY_EDITOR || UNITY_PS5
		[SerializeField, HideInInspector] private OverriderSettings PS5 = new(true);
#endif
#if UNITY_EDITOR || UNITY_PS4
		[SerializeField, HideInInspector] private OverriderSettings PS4 = new(true);
#endif
#if (UNITY_EDITOR || UNITY_SWITCH) && false
		[SerializeField, HideInInspector] private OverriderSettings SWITCH = new(true);
#endif
		private RectTransform rect;
		#endregion

		private void Start()
		{
#if UNITY_EDITOR
			rect = GetComponent<RectTransform>();
#elif UNITY_IOS || UNITY_ANDROID
			if(Input.deviceOrientation == DeviceOrientation.Portrait||
				Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown){
				SetRectTransform(PlatformType.MobilePortrait);
			}
			else if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft||
				Input.deviceOrientation == DeviceOrientation.LandscapeRight){
				SetRectTransform(PlatformType.MobileLandscape);
			}
#elif UNITY_STANDALONE_WIN
			Debug.Log("Windowsです");
			SetRectTransform(PlatformType.WindowsOS);
#elif UNITY_STANDALONE_OSX
			Debug.Log("macOSです");
			SetRectTransform(PlatformType.MacOS);
#elif UNITY_PS4
			Debug.Log("PS4です");
			SetRectTransform(PlatformType.PS4);
#elif UNITY_PS5
			Debug.Log("PS5です");
			SetRectTransform(PlatformType.PS5);
#else
			Debug.Log("その他OSです");
			SetRectTransform(PlatformType.Default);
#endif
		}


		public void SetRectTransform(PlatformType type)
		{
			switch (type)
			{
				case PlatformType.Default:
					Rect2Os(Default);
					break;
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID

				case PlatformType.MobilePortrait:
					Rect2Os(MobilePortrait);
					break;
				case PlatformType.MobileLandscape:
					Rect2Os(MobileLandscape);
					break;
				/*				case Platform.TabletPortrait:
									SetData(MobilePortrait);
									break;
								case Platform.TabletLandscape:
									SetData(MobileLandscape);
									break;*/
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_OSX

				case PlatformType.MacOS:
					Rect2Os(MacOS);
					break;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

				case PlatformType.WindowsOS:
					Rect2Os(WindowsOS);
					break;
#endif
#if UNITY_EDITOR || UNITY_PS5

				case PlatformType.PS5:
					Rect2Os(PS5);
					break;
#endif
#if UNITY_EDITOR || UNITY_PS4

				case PlatformType.PS4:
					Rect2Os(PS4);
					break;
#endif
			}
		}

		private void Rect2Os(OverriderSettings os)
		{
			if (os.useDefault)
			{
				Rect2Os(Default);
				return;
			}
			if (rect == null)
			{
				rect = GetComponent<RectTransform>();
			}
			rect.anchoredPosition = os.position;
			rect.anchorMin = os.anchorMin;
			rect.anchorMax = os.anchorMax;
			rect.pivot = os.pivot;
			rect.sizeDelta = os.sizeDelta;
			rect.offsetMin = os.offsetMin;
			rect.offsetMax = os.offsetMax;
			rect.rotation = os.rotation;
			rect.localScale = os.scale;
			gameObject.SetActive(os.activation);

			Graphic graphic = GetComponent<Graphic>();

			if (graphic is Image img)
			{
				img.sprite = os.sprite;//osで変化したら対象コンポーネントに反映
			}
			else if (graphic is RawImage raw)
			{
				raw.texture = os.texture;
			}
			else if (graphic is TextMeshProUGUI tmp)
			{
				tmp.text = os.text;
			}
		}

	}
}
