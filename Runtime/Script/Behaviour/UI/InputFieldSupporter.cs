using System;
using TMPro;

namespace OriginalLib.Behaviour
{
	public class InputFieldSupporter : SelectableSupporter<TMP_InputField>
	{
		public event Action<string> OnInputFieldValueChanged;
		public event Action<string> OnInputFieldEndEdit;
		public event Action<string> OnInputFieldSelect;
		public event Action<string> OnInputFieldDeselect;

		protected override void BindUIEvent()
		{
			UIComponent.onValueChanged.AddListener(InvokeOnInputFieldValueChanged);
			UIComponent.onEndEdit.AddListener(InvokeOnInputFieldEndEdit);
			UIComponent.onSelect.AddListener(InvokeOnInputFieldSelect);
			UIComponent.onDeselect.AddListener(InvokeOnInputFieldDeselect);

			UIComponent.onValueChanged.AddListener(OnValueChanged);
			UIComponent.onEndEdit.AddListener(OnEndEdit);
			UIComponent.onSelect.AddListener(OnSelect);
			UIComponent.onDeselect.AddListener(OnDeselect);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onValueChanged.RemoveListener(InvokeOnInputFieldValueChanged);
			UIComponent.onEndEdit.RemoveListener(InvokeOnInputFieldEndEdit);
			UIComponent.onSelect.RemoveListener(InvokeOnInputFieldSelect);
			UIComponent.onDeselect.RemoveListener(InvokeOnInputFieldDeselect);

			UIComponent.onValueChanged.RemoveListener(OnValueChanged);
			UIComponent.onEndEdit.RemoveListener(OnEndEdit);
			UIComponent.onSelect.RemoveListener(OnSelect);
			UIComponent.onDeselect.RemoveListener(OnDeselect);
		}

		protected virtual void OnValueChanged(string value) { }
		protected virtual void OnEndEdit(string value) { }
		protected virtual void OnSelect(string value) { }
		protected virtual void OnDeselect(string value) { }

		private void InvokeOnInputFieldValueChanged(string value) => OnInputFieldValueChanged?.Invoke(value);
		private void InvokeOnInputFieldEndEdit(string value) => OnInputFieldEndEdit?.Invoke(value);
		private void InvokeOnInputFieldSelect(string value) => OnInputFieldSelect?.Invoke(value);
		private void InvokeOnInputFieldDeselect(string value) => OnInputFieldDeselect?.Invoke(value);
	}
}
