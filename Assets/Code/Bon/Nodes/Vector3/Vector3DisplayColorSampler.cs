using Assets.Code.Bon.Interface;

namespace Assets.Code.Bon.Nodes.Vector3
{
	public class Vector3DisplayColorSampler : IColorSampler1D {

		private UnityEngine.Color transparentColor = new UnityEngine.Color(0f, 0f, 0f, 0f);

		public UnityEngine.Color GetColorFrom(float i)
		{
			if (i == 1f) return UnityEngine.Color.white;
			return transparentColor;
		}

		public int GetId()
		{
			return -1;
		}
	}
}
