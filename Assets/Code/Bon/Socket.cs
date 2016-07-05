using System.Security.Cryptography;
using UnityEngine;

namespace Assets.Code.Bon
{

	public class Socket
	{
		public Edge Edge;
		public Color Type;
		public Node Parent;

		// Editor related
		private Rect _boxRect = new Rect();
		public SocketDirection Direction;

		private static readonly RectOffset Padding = new RectOffset(0,0,0,0);

		public Socket(Node parent, Color type, SocketDirection direciton)
		{
			Parent = parent;
			Type = type;
			_boxRect.width = BonConfig.SocketSize;
			_boxRect.height = BonConfig.SocketSize;
			Direction = direciton;
		}

		/// The x position of the node
		public float X
		{
			get { return _boxRect.x; }
			set { _boxRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return _boxRect.y; }
			set { _boxRect.y = value; }
		}

		public Socket GetConnectedSocket()
		{
			if (Edge == null)
			{
				return null;
			}
			if (Direction == SocketDirection.Input)
			{
				return Edge.Output;
			}
			return Edge.Input;
		}

		public bool Intersects(Vector2 nodePosition)
		{
			return _boxRect.Contains(nodePosition);
		}

		public void Draw()
		{
			GUI.color = Type;
			GUI.skin.box.padding = Padding;
			GUI.skin.box.fontSize = 10;
			GUI.Box(_boxRect, ">");
		}
	}

	public enum SocketDirection
	{
		Input,
		Output
	}
}


