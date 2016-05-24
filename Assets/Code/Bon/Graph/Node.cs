using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Bon.Graph
{
	public abstract class Node
	{

		public List<Socket> Sockets = new List<Socket>();
		public readonly int Id;

		// Editor related
		private Rect windowRect;
		private static int lastFocusedNodeId;

		protected Node(int id)
		{
			Id = id;
			// default size
			Width = 100;
			Height = 100;
		}

		public abstract void Draw();

		/// The x position of the node
		public float X
		{
			get { return windowRect.x; }
			set { windowRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return windowRect.y; }
			set { windowRect.y = value; }
		}

		/// The width of the node
		public float Width
		{
			get { return windowRect.width; }
			set { windowRect.width = value; }
		}

		/// The height of the node
		public float Height
		{
			get { return windowRect.height; }
			set { windowRect.height = value; }
		}

		/// <summary>Returns true if the node is focused.</summary>
		/// <returns>True if the node is focused.</returns>
		public bool HasFocus()
		{
			return lastFocusedNodeId == Id;
		}

		/// <summary>Returns true if this assigned position intersects the node.</summary>
		/// <param name="canvasPosition">The position in canvas coordinates.</param>
		/// <returns>True if this assigned position intersects the node.</returns>
		public bool Intersects(Vector2 canvasPosition)
		{
			return windowRect.Contains(canvasPosition);
		}

		/// <summary> Returns true if this node contains the assigned socket.</summary>
		/// <param name="socket"> The socket to use.</param>
		/// <returns>True if this node contains the assigned socket.</returns>
		public bool ContainsSocket(Socket socket)
		{
			return Sockets.Contains(socket);
		}

		/// <summary> Returns the socket of the type and index.</summary>
		/// <param name="type"> The type of the socket.</param>
		/// <param name="index"> The index of sockets of this type.
		/// You can have multiple sockets of the same type.</param>
		/// <returns>The socket of the type with the index or null.</returns>
		public Socket GetSocket(Color type, int index)
		{
			var searchIndex = -1;
			foreach (var socket in Sockets)
			{
				if (socket.Type == type)
				{
					searchIndex++;
					if (searchIndex == index) return socket;
				}
			}
			return null;
		}

		/// <summary>Projects the assigned position to a node relative position.</summary>
		/// <param name="canvasPosition">The position in canvas coordinates.</param>
		/// <returns>The position in node coordinates</returns>
		public Vector2 ProjectToNode(Vector2 canvasPosition)
		{
			canvasPosition.Set(canvasPosition.x - windowRect.x, canvasPosition.y - windowRect.y);
			return canvasPosition;
		}

		/// <summary> Searches for a socket that intesects the assigned position.</summary>
		/// <param name="canvasPosition">The position for intersection in canvas coordinates.</param>
		/// <returns>The at the position or null.</returns>
		public Socket SearchSocketAt(Vector2 canvasPosition)
		{
			Vector2 nodePosition = ProjectToNode(canvasPosition);
			foreach (var socket in Sockets)
			{
				if (socket.Intersects(nodePosition)) return socket;
			}
			return null;
		}




		public void GUIDrawWindow()
		{
			windowRect = GUI.Window(Id, windowRect, GUIDrawNodeWindow, this.GetType().Name);
			GUIAlignSockets();
			Draw();
		}

		public void GUIDrawEdges()
		{
			foreach (var socket in Sockets)
			{
				// draw edges only for source nodes
				if (socket.Edge != null && ContainsSocket(socket.Edge.Source)) socket.Edge.Draw();
			}
		}

		protected void GUIAlignSockets()
		{
			var leftCount = 0;
			var rightCount = 0;
			foreach (var socket in Sockets)
			{
				if (socket.AlignLeft)
				{
					socket.X = -1;
					socket.Y = GUICalcSocketTopOffset(leftCount);
					leftCount++;
				}
				else
				{
					socket.X = windowRect.width - BonConfig.SocketSize + 1;
					socket.Y = GUICalcSocketTopOffset(rightCount);
					rightCount++;
				}
			}
		}

		private int GUICalcSocketTopOffset(int socketTopIndex)
		{
			return BonConfig.SocketOffsetTop + (socketTopIndex*BonConfig.SocketSize) + (socketTopIndex*BonConfig.SocketMargin);
		}

		void GUIDrawNodeWindow(int id)
		{

			foreach (var socket in Sockets) socket.Draw();

			/*GUILayout.BeginHorizontal();
			GUILayout.Button("sdfs");
			GUILayout.EndHorizontal();*/

			GUI.DragWindow();

			if (Event.current.GetTypeForControl(id) == EventType.Used) 	lastFocusedNodeId = id;
		}


	}
}
