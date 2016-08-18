using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes.Vector3
{
	public class Vector3DisplayColorSampler : IColorSampler {

		private UnityEngine.Color transparentColor = new UnityEngine.Color(0f, 0f, 0f, 0f);

		public UnityEngine.Color GetColor(OutputSocket s, float i)
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
