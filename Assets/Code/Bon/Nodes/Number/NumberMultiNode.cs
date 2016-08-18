using System;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Multi")]
	public class NumberMultiNode : AbstractNumberNode
	{

		[NonSerialized] private InputSocket _inputSocket;

		public NumberMultiNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new InputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(_inputSocket);
			Sockets.Add(new OutputSocket(this, typeof(AbstractNumberNode))); // second output
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

		public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
		{
			return GetInputNumber(_inputSocket, x, y, z, seed);
		}
	}
}
