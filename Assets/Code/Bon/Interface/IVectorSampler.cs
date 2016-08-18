using System.Collections.Generic;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Interface
{
	public interface IVectorSampler
	{
		List<Vector3> GetVector3List(OutputSocket outSocket, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed);
	}
}
