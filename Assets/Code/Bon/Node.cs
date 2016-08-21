using System;
using System.Collections.Generic;
using System.Net;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon
{

	public abstract class Node : IUpdateable
	{
 		[NonSerialized] public List<AbstractSocket> Sockets = new List<AbstractSocket>();
		[NonSerialized] public  int Id;
		[NonSerialized] public string Name;
		[NonSerialized] private Graph _parent;

		[NonSerialized] public Rect WindowRect;
		[NonSerialized] public int VisitCount = 0;
		[NonSerialized] public Rect ContentRect;
		[NonSerialized] public static int LastFocusedNodeId;
		[NonSerialized] public bool Resizable = true;
		[NonSerialized] public Rect ResizeArea;
		[NonSerialized] public float SocketTopOffsetInput;
		[NonSerialized] public float SocketTopOffsetOutput;

		[NonSerialized] public bool Collapsed;
		[NonSerialized] public float Height;

		protected Node(int id, Graph parent)
		{
			ResizeArea = new Rect();
			Id = id;
			// default size
			Width = 100;
			Height = 100;
			Name = GetNodeName(GetType());
			_parent = parent;
		}

		public int GetId()
		{
			return Id;
		}

		public abstract void OnGUI();

		public abstract void Update();

		public virtual void OnSerialization(SerializableNode sNode)
		{
			// for overriding: gets called before this Node gets serialized
		}

		public virtual void OnDeserialization(SerializableNode sNode)
		{
			// for overriding: gets called after this Node has been deserialized
		}

		/// The x position of the node
		public float X
		{
			get { return WindowRect.x; }
			set { WindowRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return WindowRect.y; }
			set { WindowRect.y = value; }
		}

		/// The width of the node
		public float Width
		{
			get { return WindowRect.width; }
			set { WindowRect.width = value; }
		}

		/// <summary>Returns true if the node is focused.</summary>
		/// <returns>True if the node is focused.</returns>
		public bool HasFocus()
		{
			return LastFocusedNodeId == Id;
		}

		public virtual void OnFocus()
		{
			if (_parent.TriggerEvents)
			{
				EventManager.TriggerOnFocusNode(_parent, this);
			}
		}

		public void Collapse()
		{
			WindowRect.Set(WindowRect.x, WindowRect.y, WindowRect.width, 18);
			Collapsed = true;
		}

		public void Expand()
		{
			WindowRect.Set(WindowRect.x, WindowRect.y, WindowRect.width, Height);
			Collapsed = false;
			Update();
		}

		/// <summary>Returns true if this assigned position intersects the node.</summary>
		/// <param name="canvasPosition">The position in canvas coordinates.</param>
		/// <returns>True if this assigned position intersects the node.</returns>
		public bool Intersects(Vector2 canvasPosition)
		{
			return WindowRect.Contains(canvasPosition);
		}

		/// <summary> Returns true if this node contains the assigned socket.</summary>
		/// <param name="socket"> The socket to use.</param>
		/// <returns>True if this node contains the assigned socket.</returns>
		public bool ContainsSocket(AbstractSocket socket)
		{
			return Sockets.Contains(socket);
		}

		public int GetInputSocketCount()
		{
			var count = 0;
			foreach (var socket in Sockets) if (socket.IsInput()) count++;
			return count;
		}

		// TODO update docu
		/// <summary> Returns the socket of the type and index.</summary>
		/// <param name="type"> The type of the socket.</param>
		/// <param name="direction"> The input or output direction of the socket.</param>
		/// <param name="index"> The index of sockets of this type.
		/// You can have multiple sockets of the same type.</param>
		/// <returns>The socket of the type with the index or null.</returns>
		public AbstractSocket GetSocket(Type edgeType, Type socketType, int index)
		{
			var searchIndex = -1;
			foreach (var socket in Sockets)
			{
				if (socket.Type == edgeType && socket.GetType() == socketType)
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
			canvasPosition.Set(canvasPosition.x - WindowRect.x, canvasPosition.y - WindowRect.y);
			return canvasPosition;
		}

		/// <summary> Searches for a socket that intesects the assigned position.</summary>
		/// <param name="canvasPosition">The position for intersection in canvas coordinates.</param>
		/// <returns>The at the position or null.</returns>
		public AbstractSocket SearchSocketAt(Vector2 canvasPosition)
		{
			//Vector2 nodePosition = ProjectToNode(canvasPosition);
			foreach (var socket in Sockets)
			{
				if (socket.Intersects(canvasPosition)) return socket;
			}
			return null;
		}

		/// <summary> Triggers the OnChangedNode event from within a Node.
		/// Call this method if your nodes content has changed. </summary>
		public void TriggerChangeEvent()
		{
			if (_parent.TriggerEvents)
			{
				EventManager.TriggerOnChangedNode(_parent, this);
			}
		}

		/// <summary> Returns true if all input Sockets are connected.</summary>
		/// <returns> True if all input Sockets are connected.</returns>
		public bool AllInputSocketsConnected()
		{
			foreach (var socket in Sockets)
			{
				if (!socket.IsConnected() && socket.IsInput()) return false;
			}
			return true;
		}

		/// <summary> Returns the total number of input edges connected into this node.</summary>
		public int GetConnectedInputCount() {
			int count = 0;
			foreach (var socket in Sockets) 
			{
				if (socket.IsInput () && socket.IsConnected ()) 
				{
					count++;
				}
			}
			return count;
		}


		public void GUIDrawSockets()
		{
			if (!Collapsed) foreach (var socket in Sockets) socket.Draw();
		}

		public void GUIDrawEdges()
		{
			foreach (var socket in Sockets)
			{
				if (socket.IsInput()) // draw only input sockets to avoid double drawing of edges
				{
					InputSocket inputSocket = (InputSocket) socket;
					if (inputSocket.IsConnected()) inputSocket.Edge.Draw();
				}
			}
		}

		/// <summary> Aligns the position of the sockets of this node </summary>
		public void GUIAlignSockets()
		{
			var leftCount = 0;
			var rightCount = 0;
			foreach (var socket in Sockets)
			{
				if (socket.IsInput())
				{
					socket.X = - BonConfig.SocketSize + WindowRect.x;
					socket.Y = GUICalcSocketTopOffset(leftCount) + WindowRect.y + SocketTopOffsetInput;
					leftCount++;
				}
				else
				{
					socket.X = WindowRect.width + WindowRect.x;
					socket.Y = GUICalcSocketTopOffset(rightCount) + WindowRect.y + SocketTopOffsetOutput;
					rightCount++;
				}
			}
		}

		/// <summary> Calculates the offset of a socket from the top of this node </summary>
		private int GUICalcSocketTopOffset(int socketTopIndex)
		{
			return BonConfig.SocketOffsetTop + (socketTopIndex*BonConfig.SocketSize)
				+ (socketTopIndex*BonConfig.SocketMargin);
		}

		/// <summary> Gets the menu entry name of this node </summary>
		public static string GetNodeName(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
			return string.IsNullOrEmpty(attr.Name) ? nodeType.Name : attr.Name;
		}

		/// <summary> Gets the menu entry path of this node </summary>
		public static string GetNodePath(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
			return string.IsNullOrEmpty(attr.Path) ? null : attr.Path;
		}

		/// <summary> Converts this node to a SerializableNode </summary>
		public SerializableNode ToSerializedNode()
		{
			SerializableNode n = new SerializableNode();
			n.type = GetType().FullName;
			n.id = Id;
			n.X = WindowRect.xMin;
			n.Y = WindowRect.yMin;
			n.Collapsed = Collapsed;
			n.directInputValues = new float[Sockets.Count];

			for (var i = 0; i < n.directInputValues.Length; i++)
			{
				if (Sockets[i].IsInput())
				{
					InputSocket inputSocket = (InputSocket) Sockets[i];
					if (inputSocket.IsInDirectInputMode()) n.directInputValues[i] = inputSocket.GetDirectInputNumber();
				}

			}

			n.data = JsonUtility.ToJson(this); // custom node data can be used
			OnSerialization(n);
			return n;
		}

		public static Color GetEdgeColor(Type nodeType)
		{
			if (nodeType == typeof(AbstractNumberNode)) return new Color(0.32f, 0.58f, 0.86f, 1);
			if (nodeType == typeof(AbstractColorNode)) return new Color(0.54f, 0.70f, 0.50f, 1);
			if (nodeType == typeof(AbstractStringNode)) return new Color(0.84f, 0.45f, 0.39f, 1f);
			if (nodeType == typeof(AbstractVector3Node)) return new Color(0.9f, 0.9f, 0.9f, 1f);
			return Color.black;
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
		[SerializeField] public float[] directInputValues;
		[SerializeField] public bool Collapsed;
	}

	/// <summary> Annotation to register menu entries of Nodes to the editor.</summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class GraphContextMenuItem : Attribute
	{
		private  string _menuPath;
		public string Path { get { return _menuPath; } }

		private  string _itemName;
		public string Name { get { return _itemName; } }

		public GraphContextMenuItem(string menuPath) : this(menuPath, null)
		{
		}

		public GraphContextMenuItem(string menuPath, string itemName)
		{
			_menuPath = menuPath;
			_itemName = itemName;
		}
	}



}


