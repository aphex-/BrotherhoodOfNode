using System;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes
{
	public abstract class AbstractNumberNode : Node, INumberSampler
	{

		[NonSerialized] protected OutputSocket _outSocket;

		protected AbstractNumberNode(int id, Graph parent) : base(id, parent)
		{
			_outSocket = new OutputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(_outSocket);
		}

		public abstract float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed);

		public static float GetInputNumber(InputSocket socket, float x, float y, float z, float seed)
		{
			if (socket.Type != typeof(AbstractNumberNode)) return float.NaN;
			if (socket.IsInDirectInputMode()) return socket.GetDirectInputNumber();

			AbstractNumberNode node = (AbstractNumberNode) socket.GetConnectedSocket().Parent;
			return node.GetNumber(socket.GetConnectedSocket(), x, y, z, seed);
		}

	}
}
