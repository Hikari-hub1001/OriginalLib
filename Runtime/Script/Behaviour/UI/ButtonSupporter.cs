using System;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
	public class ButtonSupporter : SelectableSupporter<Button>
	{
		public event Action OnButtonClick;

		protected override void BindUIEvent()
		{
			UIComponent.onClick.AddListener(InvokeOnButtonClick);
			UIComponent.onClick.AddListener(OnClick);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onClick.RemoveListener(InvokeOnButtonClick);
			UIComponent.onClick.RemoveListener(OnClick);
		}

		protected virtual void OnClick() { }

		private void InvokeOnButtonClick() => OnButtonClick?.Invoke();
	}
}
