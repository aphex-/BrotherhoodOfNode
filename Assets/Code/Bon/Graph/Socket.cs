using UnityEngine;

namespace Assets.Code.Bon.Graph
{

	public class Socket
	{
		public Edge Edge;
		public Color Type;
		public Node Parent;

		// Editor related
		private Rect boxRect = new Rect();
		public SocketDirection Direction;


		public Socket(Node parent, Color type, SocketDirection direciton)
		{
			Parent = parent;
			Type = type;
			boxRect.width = BonConfig.SocketSize;
			boxRect.height = BonConfig.SocketSize;
			Direction = direciton;
		}

		/// The x position of the node
		public float X
		{
			get { return boxRect.x; }
			set { boxRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return boxRect.y; }
			set { boxRect.y = value; }
		}


		public bool Intersects(Vector2 nodePosition)
		{
			return boxRect.Contains(nodePosition);
		}

		public void Draw()
		{
			GUI.color = Type;
			GUI.Box(boxRect, "");
		}
	}

	public enum SocketDirection
	{
		Input,
		Output
	}
}


