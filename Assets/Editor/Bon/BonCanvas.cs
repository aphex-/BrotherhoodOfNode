using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon;

namespace Assets.Editor.Bon
{
	[Serializable]
	public class BonCanvas
	{
		public  GUIStyle Style = new GUIStyle();

		public const float CanvasSize = 100000;

		public string FilePath;

		public Rect DrawArea = new Rect();

		[SerializeField]
		public float Zoom = 1;
		[SerializeField]
		public Vector2 Position = new Vector2();

		public Graph Graph;

		public Rect TabButton = new Rect();
		public Rect CloseTabButton = new Rect();

		private Vector2 _tmpVector01 = new Vector2();
		private Vector2 _tmpVector02 = new Vector2();

		private  Color _backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
		private  Color _backgroundLineColor01 = new Color(0.14f, 0.14f, 0.14f, 1f);
		private  Color _backgroundLineColor02 = new Color(0.10f, 0.10f, 0.10f, 1f);

		private GUIStyle centeredLabelStyle;

		public BonCanvas(Graph graph)
		{
			Graph = graph;
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

		public void Draw(EditorWindow window, Rect region, AbstractSocket currentDragingSocket)
		{
			if (centeredLabelStyle == null) centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.MiddleCenter;

			EditorZoomArea.Begin(Zoom, region);

			if (Style.normal.background == null) 	Style.normal.background = CreateBackgroundTexture();
			GUI.DrawTextureWithTexCoords(DrawArea, Style.normal.background, new Rect(0, 0, 1000, 1000));
			DrawArea.Set(Position.x, Position.y, CanvasSize, CanvasSize);
			GUILayout.BeginArea(DrawArea);
			DrawEdges();
			window.BeginWindows();
			DrawNodes();
			window.EndWindows();
			DrawDragEdge(currentDragingSocket);

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Graph.GetNodeAt(i).GUIDrawSockets();
			}

			GUILayout.EndArea();
			EditorZoomArea.End();
		}

		private void DrawDragEdge(AbstractSocket currentDragingSocket)
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
				if (!node.Collapsed) node.WindowRect.height = node.Height;
				node.WindowRect = GUI.Window(node.Id, node.WindowRect, GUIDrawNodeWindow, node.Name + "");
				if (node.Collapsed)
				{
					// title bar text is not visible if collapsed
					GUI.Label(node.WindowRect, node.Name + "", centeredLabelStyle);
				}

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

				if (node.Collapsed) m.AddItem(new GUIContent("Expand"), false, ExpandNode, nodeId);
				else m.AddItem(new GUIContent("Collapse"), false, CollapseNode, nodeId);

				m.ShowAsContext();
				Event.current.Use();
			}

			if (!node.Collapsed)
			{
				GUILayout.BeginArea(node.ContentRect);
				GUI.color = Color.white;
				node.OnGUI();
				GUILayout.EndArea();
			}

			GUI.DragWindow();
			if (Event.current.GetTypeForControl(node.Id) == EventType.Used)
			{
				if (Node.LastFocusedNodeId != node.Id) node.OnFocus();
				Node.LastFocusedNodeId = node.Id;
			}
		}

		private void CollapseNode(object nodeId)
		{
			Node node = Graph.GetNode((int) nodeId);
			node.Collapse();
		}

		private void ExpandNode(object nodeId)
		{
			Graph.GetNode((int) nodeId).Expand();
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
		public AbstractSocket GetSocketAt(Vector2 windowPosition)
		{
			Vector2 projectedPosition = ProjectToCanvas(windowPosition);

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				AbstractSocket socket = node.SearchSocketAt(projectedPosition);
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


