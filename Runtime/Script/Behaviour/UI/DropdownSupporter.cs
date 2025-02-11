using System;
using TMPro;

namespace OriginalLib.Behaviour
{
	public class DropdownSupporter : SelectableSupporter<TMP_Dropdown>
	{
		public event Action<int> OnDropdownValueChanged;

		protected override void BindUIEvent()
		{
			UIComponent.onValueChanged.AddListener(InvokeOnDropdownValueChanged);
			UIComponent.onValueChanged.AddListener(OnValueChanged);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onValueChanged.RemoveListener(InvokeOnDropdownValueChanged);
			UIComponent.onValueChanged.RemoveListener(OnValueChanged);
		}

		protected virtual void OnValueChanged(int value) { }

		private void InvokeOnDropdownValueChanged(int value) => OnDropdownValueChanged?.Invoke(value);
	}
}
