using System;
using UnityEngine;

namespace OriginalLib.SaveLoad
{
	[Serializable]
	public abstract class Saveable
	{
		[NonSerialized]
		public readonly string FileName;

		protected virtual bool Encrypt =>
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			false;
#else
			true;
#endif

		public Saveable(string fileName)
		{
			this.FileName = fileName;
			SaveManager.AddSaveable(this);
		}
		public Saveable() { }

		public virtual void Save()
		{
			Debug.Log($"<color=white>******************* Save {FileName} **********************</color>\r\n{this}");
			_ = SaveManager.Save(this, Encrypt);
		}

		public override string ToString() => JsonUtility.ToJson(this, true);
	}
}
