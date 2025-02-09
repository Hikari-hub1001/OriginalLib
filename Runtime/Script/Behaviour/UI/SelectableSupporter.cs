using UnityEngine;
using UnityEngine.UI;

namespace OriginalLib.Behaviour
{
    public abstract class SelectableSupporter<T> : MonoBehaviour where T:Selectable
    {
		protected T UIComponent => component ??= GetComponent<T>();

		private T component;

		protected abstract void BindUIEvent();
		protected abstract void UnbindUIEvent();

		protected virtual void OnEnable()
		{
			BindUIEvent();
		}

		protected virtual void OnDisable()
		{
			UnbindUIEvent();
		}
	}
}
