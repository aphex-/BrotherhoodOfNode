using System;
using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Bon
{

	public abstract class Node
	{
 		[System.NonSerialized]
		public List<Socket> Sockets = new List<Socket>();

		[System.NonSerialized]
		public readonly int Id;

		[System.NonSerialized]
		private INodeListener listener;

		[System.NonSerialized]
		public string nodeName;

		// Editor related
		 [System.NonSerialized]
		public Rect windowRect;

		 [System.NonSerialized]
		public Rect contentRect;

		[System.NonSerialized]
		public static int lastFocusedNodeId;

		protected Node(int id)
		{
			Id = id;
			// default size
			Width = 100;
			Height = 100;
			nodeName = GetNodeName(this.GetType());
		}

		public abstract void OnGUI();

		public abstract void OnSerialization(SerializableNode sNode);
		public abstract void OnDeserialization(SerializableNode sNode);

		public abstract object GetResultOf(Socket outSocket);
		public abstract bool CanGetResultOf(Socket outSocket);

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
		/// <param name="direction"> The input or output direction of the socket.</param>
		/// <param name="index"> The index of sockets of this type.
		/// You can have multiple sockets of the same type.</param>
		/// <returns>The socket of the type with the index or null.</returns>
		public Socket GetSocket(Color type, SocketDirection direction, int index)
		{
			var searchIndex = -1;
			foreach (var socket in Sockets)
			{
				if (socket.Type == type && socket.Direction == direction)
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
			//Vector2 nodePosition = ProjectToNode(canvasPosition);
			foreach (var socket in Sockets)
			{
				if (socket.Intersects(canvasPosition)) return socket;
			}
			return null;
		}

		public void RegisterListener(INodeListener l)
		{
			this.listener = l;
		}

		protected void TriggerChangeEvent() // call this method if your nodes content has changed
		{
			if (this.listener != null) this.listener.OnNodeChanged(this);
		}

		public bool AllInputSocketsConnected()
		{
			foreach (var socket in Sockets)
			{
				if (socket.Edge == null && socket.Direction == SocketDirection.Input) return false;
			}
			return true;
		}

		public void GUIDrawSockets()
		{
			foreach (var socket in Sockets) socket.Draw();
		}

		public void GUIDrawEdges()
		{
			foreach (var socket in Sockets)
			{
				if (socket.Edge != null && ContainsSocket(socket.Edge.Output)) socket.Edge.Draw();
			}
		}

		public void GUIAlignSockets()
		{
			var leftCount = 0;
			var rightCount = 0;
			foreach (var socket in Sockets)
			{
				if (socket.Direction == SocketDirection.Input)
				{
					socket.X = - BonConfig.SocketSize + windowRect.x;
					socket.Y = GUICalcSocketTopOffset(leftCount) + windowRect.y;
					leftCount++;
				}
				else
				{
					socket.X = windowRect.width + windowRect.x;
					socket.Y = GUICalcSocketTopOffset(rightCount) + windowRect.y;
					rightCount++;
				}
			}
		}

		private int GUICalcSocketTopOffset(int socketTopIndex)
		{
			return BonConfig.SocketOffsetTop + (socketTopIndex*BonConfig.SocketSize)
				+ (socketTopIndex*BonConfig.SocketMargin);
		}

		public static string GetNodeName(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
			return string.IsNullOrEmpty(attr.Name) ? nodeType.Name : attr.Name;
		}

		public static string GetNodePath(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
			return string.IsNullOrEmpty(attr.Path) ? null : attr.Path;
		}

		public SerializableNode ToSerializedNode()
		{
			SerializableNode n = new SerializableNode();
			n.type = this.GetType().FullName;
			n.id = Id;
			n.X = windowRect.xMin;
			n.Y = windowRect.yMin;
			n.data = JsonUtility.ToJson(this);
			OnSerialization(n);
			return n;
		}
	}

	/// <summary> A class to serialize a Node.</summary>
	[Serializable] public class SerializableNode
	{
		[SerializeField] public string type;
		[SerializeField] public int id;
		[SerializeField] public float X;
		[SerializeField] public float Y;
		[SerializeField] public string data;
	}

	/// <summary> Annotation to register menu entries of Nodes to the editor.</summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class GraphContextMenuItem : Attribute
	{
		private readonly string menuPath;
		public string Path { get { return menuPath; } }

		private readonly string itemName;
		public string Name { get { return itemName; } }

		public GraphContextMenuItem(string menuPath) : this(menuPath, null)
		{
		}

		public GraphContextMenuItem(string menuPath, string itemName)
		{
			this.menuPath = menuPath;
			this.itemName = itemName;
		}

	}
}


