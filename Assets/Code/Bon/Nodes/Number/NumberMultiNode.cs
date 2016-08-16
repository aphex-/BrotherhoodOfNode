using System;

namespace Assets.Code.Bon.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Multi")]
	public class NumberMultiNode : AbstractNumberNode
	{

		[NonSerialized] private Socket _inputSocket;

		public NumberMultiNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocket);
			Sockets.Add(new Socket(this, typeof(AbstractNumberNode), SocketDirection.Output)); // second output
			SocketTopOffsetInput = 15;
			Width = 50;
			Height = 60;
		}

		public override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed); // return same value for all out sockets
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			return GetInputNumber(_inputSocket, x, y, seed);
		}
	}
}
