using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriginalLib.Behaviour
{
	public class UIManager : Singleton_DontDestroy<UIManager>
	{
		[SerializeField]
		private Camera uiCamera;
		private Camera mainCam;

		[SerializeField]
		private bool useUICamera;

		[SerializeField]
		private List<UIObject> uiPrefabList;
		private List<CustomUI> uiInstanceList = new();

		public GameObject UIContainer { get; protected set; }

		private readonly List<string> showUI = new();

		protected override void Init()
		{
			foreach (var ui in uiPrefabList)
			{
				CustomUI o;
				if (ui.isPrefab)
				{
					o = Instantiate(ui.customUI);
				}
				else
				{
					o = ui.customUI;
				}

				uiInstanceList.Add(o);
				//DontDestroyOnLoad(o);

				if (useUICamera)
				{
					if (uiCamera == null)
					{
						uiCamera = new Camera();
						uiCamera.name = "UICamera";
						uiCamera.transform.parent = transform;
					}
					o.InitUI(uiCamera);
				}
				else
				{
					if (mainCam == null)
					{
						mainCam = Camera.main;
					}
					o.InitUI(mainCam);
				}
			}

			UIContainer = new GameObject("UIContainer");
			UIContainer.SetActive(false);
			UIContainer.transform.SetParent(transform);
		}

		public T GetUI<T>() where T : CustomUI
		{
			foreach (var ui in uiInstanceList)
			{
				if (ui as T) return (T)ui;
			}
			return default(T);
		}

		[Serializable]
		private class UIObject
		{
			public CustomUI customUI;
			public bool isPrefab = false;
		}
	}
}