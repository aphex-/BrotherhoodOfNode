using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Interface
{
	public interface IColorSampler
	{
		Color GetColor(OutputSocket socket, float i);
	}
}
