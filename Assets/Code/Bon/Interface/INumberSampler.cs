using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Interface
{
	public interface INumberSampler
	{
		float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed);
	}
}
