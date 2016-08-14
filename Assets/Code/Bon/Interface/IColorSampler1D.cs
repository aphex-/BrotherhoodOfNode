using UnityEngine;

namespace Assets.Code.Bon.Interface
{
	public interface IColorSampler1D {

		Color GetColorFrom(float i);
		int GetId();

	}
}
