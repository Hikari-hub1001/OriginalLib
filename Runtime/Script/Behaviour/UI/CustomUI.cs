using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if AVAILABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OriginalLib.Behaviour
{
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(Canvas))]
	public class CustomUI : MonoBehaviour
	{
		[SerializeField]
		private bool firstShow = false;

		[SerializeField]
		private bool isDontDestroy = false;
		private GameObject uiObject = null;

		public UnityEvent OnShowComplete;
		public UnityEvent OnHideComplete;

		private CanvasGroup canvasGroup;
		private Canvas canvas;

		private readonly float fadeTime = 0.35f;

		private UIManager uiManager => UIManager.Instance;

#if AVAILABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM

		public event Action<InputAction.CallbackContext> Perform;
		public event Action<InputAction.CallbackContext> Canceled;
		public event Action<InputAction.CallbackContext> Started;
#endif

		private void Awake()
		{
			if (isDontDestroy)
			{
				if (uiObject == null)
				{
					DontDestroyOnLoad(this.gameObject);
					uiObject = gameObject;
				}
				else
				{
					Destroy(gameObject);
					return;
				}
			}
			canvasGroup = GetComponent<CanvasGroup>();

			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}

		}

		public virtual void Show()
		{
			float alpha = canvasGroup.alpha;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			transform.SetParent(uiManager.transform);
			StartCoroutine(TweenFloat(alpha, 1.0f, fadeTime, (f) => canvasGroup.alpha = f, () => OnShowComplete?.Invoke()));

			//UIManager.Instance.ShowUI.Add(gameObject.name);
		}

		public virtual void Hide()
		{
			float alpha = canvasGroup.alpha;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			StartCoroutine(
				TweenFloat(
					alpha,
					0.0f,
					fadeTime,
					(f) => canvasGroup.alpha = f,
					() =>
					{
						OnHideComplete?.Invoke();
						transform.SetParent(uiManager.UIContainer.transform);
						transform.SetParent(uiManager.UIContainer.transform);
					}));

			//UIManager.Instance.ShowUI.Remove(gameObject.name);
		}


		private IEnumerator TweenFloat(float start, float end, float time, Action<float> onUpdate = null, Action onComplete = null)
		{
			float val = start;
			float elapsedTime = 0f;

			while (elapsedTime <= time)
			{
				float currentValue = Mathf.Lerp(start, end, elapsedTime / time);
				onUpdate?.Invoke(currentValue);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			onUpdate?.Invoke(end);
			onComplete?.Invoke();
		}

		public virtual void InitUI(Camera cam)
		{
			if (canvasGroup == null)
			{
				canvasGroup = GetComponent<CanvasGroup>();

				if (canvasGroup == null)
				{
					canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
			}

			if (canvas == null)
			{
				canvas = GetComponent<Canvas>();

				if (canvas == null)
				{
					canvas = gameObject.AddComponent<Canvas>();
				}
			}

			if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				canvas.worldCamera = cam;
			}

			if (firstShow)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
#if AVAILABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
		//private void InputPerformEvent(InputAction.CallbackContext context)
		//{
		//	if (UIManager.Instance.ShowUI.LastItem() != gameObject.name) return;
		//	Perform?.Invoke(context);
		//}
#endif
	}


}