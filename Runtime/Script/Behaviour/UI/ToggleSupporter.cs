using System;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
	public class ToggleSupporter : SelectableSupporter<Toggle>
	{
		public event Action<bool> OnToggleValueChanged;

		protected override void BindUIEvent()
		{
			UIComponent.onValueChanged.AddListener(InvokeOnToggleValueChanged);
			UIComponent.onValueChanged.AddListener(OnValueChanged);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onValueChanged.RemoveListener(InvokeOnToggleValueChanged);
			UIComponent.onValueChanged.RemoveListener(OnValueChanged);
		}

		protected virtual void OnValueChanged(bool value) { }

		private void InvokeOnToggleValueChanged(bool value) => OnToggleValueChanged?.Invoke(value);
	}
}
