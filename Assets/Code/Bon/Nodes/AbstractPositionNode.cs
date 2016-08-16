

using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using UnityEngine;

public abstract class AbstractPositionNode : Node, IPositionSampler
{
	protected float _x;
	protected float _y;
	protected float _z;
	protected float _width;
	protected float _height;
	protected float _depth;
	protected float _seed;

	protected AbstractPositionNode(int id, Graph parent) : base(id, parent)
	{

	}

	public static List<Vector3> GetInputPositions(Socket socket, float x, float y, float z,
													float width, float height,float depth, float seed)
	{
		if (socket.Type != typeof(AbstractPositionNode) || socket.Direction == SocketDirection.Output) return null;
		AbstractPositionNode node = (AbstractPositionNode) socket.GetConnectedSocket().Parent;
		return node.GetPositions(x, y, z, width, height, depth, seed);
	}

	public abstract List<Vector3> GetPositions(float x, float y, float z, float width, float height, float depth, float seed);

}
