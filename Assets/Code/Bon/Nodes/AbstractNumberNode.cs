using System;
using Assets.Code.Bon.Interface;

namespace Assets.Code.Bon.Nodes
{
	public abstract class AbstractNumberNode : Node, ISampler3D
	{

		[NonSerialized] protected float _x;
		[NonSerialized] protected float _y;
		[NonSerialized] protected float _seed;

		[NonSerialized] protected Socket _outSocket;

		protected AbstractNumberNode(int id, Graph parent) : base(id, parent)
		{
			_outSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Output);
			Sockets.Add(_outSocket);
		}

		public void SetPosition(float x, float y, float z)
		{
			_x = x;
			_y = y;
			_seed = z;
		}

		public abstract float GetSampleAt(float x, float y, float seed);

		public static float GetInputNumber(Socket socket, float x, float y, float seed)
		{
			if (socket.Type != typeof(AbstractNumberNode) || socket.Direction == SocketDirection.Output) return float.NaN;
			if (socket.IsInDirectInputMode()) return socket.GetDirectInputNumber();

			AbstractNumberNode node = (AbstractNumberNode) socket.GetConnectedSocket().Parent;
			node.SetPosition(x, y, seed);
			return (float) node.GetResultOf(socket.GetConnectedSocket());
		}

		/// AbstractNumberNodes can always return a result even if not all sockets are connected.
		/// This is because unconnected sockets always have a direct input value.
		///
		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}
	}
}
