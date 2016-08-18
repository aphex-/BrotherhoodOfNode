using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes
{
	public abstract class AbstractVector3Node : Node, IVectorSampler
	{
		protected AbstractVector3Node(int id, Graph parent) : base(id, parent)
		{

		}

		public static List<UnityEngine.Vector3> GetInputVector3List(InputSocket socket, float x, float y, float z,
			float sizeX, float sizeY, float sizeZ, float seed)
		{
			if (socket.Type != typeof(AbstractVector3Node) || !socket.CanGetResult()) return null;
			AbstractVector3Node node = (AbstractVector3Node) socket.GetConnectedSocket().Parent;
			return node.GetVector3List(socket.GetConnectedSocket(), x, y, z, sizeX, sizeY, sizeZ, seed);
		}

		public abstract List<UnityEngine.Vector3> GetVector3List(OutputSocket socket, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed);

	}
}
