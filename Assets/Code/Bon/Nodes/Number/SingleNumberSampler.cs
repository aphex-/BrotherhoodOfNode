using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes.Number
{
	public class SingleNumberSampler : INumberSampler
	{
		private float _number;

		public SingleNumberSampler(float number)
		{
			_number = number;
		}

		public float GetNumber(OutputSocket s, float x, float y, float z, float seed)
		{
			return _number;
		}

		public int GetId()
		{
			return -1;
		}
	}
}
