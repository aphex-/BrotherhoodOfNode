using System;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon;

namespace Assets.Editor.Bon
{
	public class BonCanvas
	{
		public readonly GUIStyle Style = new GUIStyle();

		public const float CanvasSize = 100000;

		public string FilePath;

		public Rect DrawArea = new Rect();
		public float Zoom = 1;
		public Vector2 Position = new Vector2();
		public Graph Graph;

		public Rect TabButton = new Rect();
		public Rect CloseTabButton = new Rect();

		private Vector2 _tmpVector01 = new Vector2();
		private Vector2 _tmpVector02 = new Vector2();

		private readonly Color _backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
		private readonly Color _backgroundLineColor01 = new Color(0.14f, 0.14f, 0.14f, 1f);
		private readonly Color _backgroundLineColor02 = new Color(0.10f, 0.10f, 0.10f, 1f);

		public BonCanvas(Graph graph)
		{
			this.Graph = graph;
			Style.normal.background = CreateBackgroundTexture();
			Style.normal.background.wrapMode = TextureWrapMode.Repeat;
			Style.fixedHeight = CanvasSize;
			Style.fixedWidth = CanvasSize;

		}

		private Texture2D CreateBackgroundTexture()
		{
			var texture = new Texture2D(100, 100, TextureFormat.ARGB32, false);
			for (var x = 0; x < texture.width; x++)
			{
				for (var y = 0; y < texture.width; y++)
				{
					bool isVerticalLine = (x%11 == 0);
					bool isHorizontalLine = (y % 11 == 0);
					if (x == 0 || y == 0) texture.SetPixel(x, y, _backgroundLineColor02);
					else if (isVerticalLine || isHorizontalLine) texture.SetPixel(x, y, _backgroundLineColor01);
					else texture.SetPixel(x, y, _backgroundColor);
				}
			}
			texture.filterMode = FilterMode.Trilinear;
			texture.wrapMode = TextureWrapMode.Repeat;
			texture.Apply();
			return texture;
		}

		public void Draw(EditorWindow window, Rect region, Socket currentDragingSocket)
		{
			EditorZoomArea.Begin(Zoom, region);

			if (this.Style.normal.background == null) 	this.Style.normal.background = CreateBackgroundTexture();
			GUI.DrawTextureWithTexCoords(this.DrawArea, this.Style.normal.background, new Rect(0, 0, 1000, 1000));
			this.DrawArea.Set(this.Position.x, this.Position.y, CanvasSize, CanvasSize);
			GUILayout.BeginArea(this.DrawArea);
			this.DrawEdges();
			window.BeginWindows();
			this.DrawNodes();
			window.EndWindows();
			this.DrawDragEdge(currentDragingSocket);

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Graph.GetNodeAt(i).GUIDrawSockets();
			}

			GUILayout.EndArea();
			EditorZoomArea.End();
		}

		private void DrawDragEdge(Socket currentDragingSocket)
		{
			if (currentDragingSocket != null)
			{
				_tmpVector01 = Edge.GetEdgePosition(currentDragingSocket, _tmpVector01);
				_tmpVector02 = Edge.GetTangentPosition(currentDragingSocket, _tmpVector01);
				Edge.DrawEdge(_tmpVector01, _tmpVector02, Event.current.mousePosition, Event.current.mousePosition,
					currentDragingSocket.Type);
			}
		}

		public void DrawNodes()
		{
			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				node.WindowRect = GUI.Window(node.Id, node.WindowRect, GUIDrawNodeWindow, node.Name + " (" + node.Id + ")");
				node.GUIAlignSockets();
			}
		}

		void GUIDrawNodeWindow(int nodeId)
		{
			Node node = Graph.GetNode(nodeId);
			node.ContentRect.Set(0, BonConfig.SocketOffsetTop,
				node.Width, node.Height - BonConfig.SocketOffsetTop);

			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				GenericMenu m = new GenericMenu();
				m.AddDisabledItem(new GUIContent(node.Name + " (" + nodeId + ")"));
				m.AddSeparator("");
				m.AddItem(new GUIContent("Delete"), false, DeleteNode, nodeId);
				m.ShowAsContext();
				Event.current.Use();
			}

			GUILayout.BeginArea(node.ContentRect);
			GUI.color = Color.white;
			node.OnGUI();
			GUILayout.EndArea();
			GUI.DragWindow();
			if (Event.current.GetTypeForControl(node.Id) == EventType.Used) Node.LastFocusedNodeId = node.Id;
		}

		private void DeleteNode(object nodeId)
		{
			Graph.RemoveNode((int) nodeId);
		}


		public void DrawEdges()
		{
			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Graph.GetNodeAt(i).GUIDrawEdges();
			}
		}

		public Node GetFocusedNode()
		{
			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				if (node.HasFocus()) return node;
			}
			return null;
		}

		/// <summary> Returns the socket at the window position.</summary>
		/// <param name="windowPosition"> The position to get the Socket from in window coordinates</param>
		/// <returns>The socket at the posiiton or null or null.</returns>
		public Socket GetSocketAt(Vector2 windowPosition)
		{
			Vector2 projectedPosition = ProjectToCanvas(windowPosition);

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				Socket socket = node.SearchSocketAt(projectedPosition);
				if (socket != null)
				{
					return socket;
				}
			}
			return null;
		}

		public Node CreateNode(Type nodeType, Vector2 windowPosition)
		{
			Node node = (Node) Graph.CreateNode(nodeType);
			var position = ProjectToCanvas(windowPosition);
			node.X = position.x;
			node.Y = position.y;
			Graph.AddNode(node);
			return node;
		}

		public void RemoveFocusedNode()
		{

			Node node = GetFocusedNode();
			if (node != null) Graph.RemoveNode(node);
		}

		public Vector2 ProjectToCanvas(Vector2 windowPosition)
		{
			windowPosition.y += (21) - ((BonWindow.TopOffset*2));
			windowPosition = windowPosition/this.Zoom;
			windowPosition.x -= (this.DrawArea.x);
			windowPosition.y -= (this.DrawArea.y);
			return windowPosition;
		}
	}



}


