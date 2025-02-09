using System;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
	public class ScrollbarSupporter : SelectableSupporter<Scrollbar>
	{
		public event Action<float> OnScrollbarValueChanged;

		protected override void BindUIEvent()
		{
			UIComponent.onValueChanged.AddListener(InvokeOnScrollbarValueChanged);
			UIComponent.onValueChanged.AddListener(OnValueChanged);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onValueChanged.RemoveListener(InvokeOnScrollbarValueChanged);
			UIComponent.onValueChanged.RemoveListener(OnValueChanged);
		}

		protected virtual void OnValueChanged(float value) { }

		private void InvokeOnScrollbarValueChanged(float value) => OnScrollbarValueChanged?.Invoke(value);
	}
}
