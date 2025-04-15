using UnityEditor;


namespace OriginalLib.Behaviour
{
	[CustomEditor(typeof(SpriteNumberInt))]
	public class SpriteNumberIntEditor : SpriteNumberEditor<SpriteNumberInt>
	{
		protected override bool useDecimal => false;
	}
}