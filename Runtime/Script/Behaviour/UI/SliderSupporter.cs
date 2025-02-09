using System;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
	public class SliderSupporter : SelectableSupporter<Slider>
	{
		public event Action<float> OnSliderValueChanged;

		protected override void BindUIEvent()
		{
			UIComponent.onValueChanged.AddListener(InvokeOnSliderValueChanged);
			UIComponent.onValueChanged.AddListener(OnValueChanged);
		}

		protected override void UnbindUIEvent()
		{
			UIComponent.onValueChanged.RemoveListener(InvokeOnSliderValueChanged);
			UIComponent.onValueChanged.RemoveListener(OnValueChanged);
		}

		protected virtual void OnValueChanged(float value) { }

		private void InvokeOnSliderValueChanged(float value) => OnSliderValueChanged?.Invoke(value);
	}
}
