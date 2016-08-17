

using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using UnityEngine;

public abstract class AbstractVector3Node : Node, IPositionSampler
{
	protected float _x;
	protected float _y;
	protected float _z;
	protected float _width;
	protected float _height;
	protected float _depth;
	protected float _seed;

	protected AbstractVector3Node(int id, Graph parent) : base(id, parent)
	{

	}

	public static List<Vector3> GetInputVector3List(Socket socket, float x, float y, float z,
													float width, float height,float depth, float seed)
	{
		if (socket.Type != typeof(AbstractVector3Node) || socket.Direction == SocketDirection.Output) return null;
		AbstractVector3Node node = (AbstractVector3Node) socket.GetConnectedSocket().Parent;
		return node.GetVector3List(x, y, z, width, height, depth, seed);
	}

	public abstract List<Vector3> GetVector3List(float x, float y, float z, float width, float height, float depth, float seed);

}
