using System;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon
{

	public abstract class AbstractSocket
	{
		public Type Type;
		public Node Parent;

		protected Rect BoxRect;
		protected RectOffset Padding;

		protected AbstractSocket(Node parent, Type type)
		{
			Type = type;
			Padding = new RectOffset(0, 0, -2, 0);
			Parent = parent;
			BoxRect.width = BonConfig.SocketSize;
			BoxRect.height = BonConfig.SocketSize;
		}

		/// The x position of the node
		public float X
		{
			get { return BoxRect.x; }
			set { BoxRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return BoxRect.y; }
			set { BoxRect.y = value; }
		}

		public abstract bool IsConnected();
		protected abstract void OnDraw();
		public abstract bool Intersects(Vector2 nodePosition);


		public void Draw()
		{
			GUI.skin.box.normal.textColor = Node.GetEdgeColor(Type);
			GUI.skin.box.padding = Padding;
			GUI.skin.box.fontSize = 14;
			GUI.skin.box.fontStyle = FontStyle.Bold;
			OnDraw();
		}

		public bool IsInput()
		{
			return GetType() == typeof(InputSocket);
		}

		public bool IsOutput()
		{
			return GetType() == typeof(OutputSocket);
		}

	}

}


