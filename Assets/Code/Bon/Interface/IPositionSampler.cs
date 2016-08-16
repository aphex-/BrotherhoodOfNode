using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Interface
{
	public interface IPositionSampler
	{
		List<Vector3> GetPositions(float x, float y, float z, float width, float height, float depth, float seed);
		int GetId();
	}
}
