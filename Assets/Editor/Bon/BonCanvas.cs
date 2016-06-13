using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Code.Bon.Graph;
using UnityEditor;
using UnityEngine;

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
		public Graph graph;

		public Rect TabButton = new Rect();
		public Rect CloseTabButton = new Rect();

		private Vector2 _tmpVector01 = new Vector2();
		private Vector2 _tmpVector02 = new Vector2();

		public BonCanvas(Graph graph)
		{
			this.graph = graph;
			Style.normal.background = Texture2D.whiteTexture;
		}

		public void Draw(EditorWindow window, Rect region, Socket currentDragingSocket)
		{
			EditorZoomArea.Begin(Zoom, region);
			this.DrawArea.Set(this.Position.x, this.Position.y, CanvasSize, CanvasSize);
			GUILayout.BeginArea(this.DrawArea, this.Style);
			this.DrawEdges();

			window.BeginWindows();
			this.DrawNodes();
			window.EndWindows();
			this.DrawDragEdge(currentDragingSocket);


			for (var i = 0; i < graph.GetNodeCount(); i++)
			{
				graph.GetNodeAt(i).GUIDrawSockets();
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

		public void DrawTabButton(int canvasIndex)
		{

		}

		public void DrawNodes()
		{
			for (var i = 0; i < graph.GetNodeCount(); i++)
			{
				graph.GetNodeAt(i).GUIDrawWindow();
			}
		}

		public void DrawEdges()
		{
			for (var i = 0; i < graph.GetNodeCount(); i++)
			{
				graph.GetNodeAt(i).GUIDrawEdges();
			}
		}

		public Node GetFocusedNode()
		{
			for (var i = 0; i < graph.GetNodeCount(); i++)
			{
				Node node = graph.GetNodeAt(i);
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

			for (var i = 0; i < graph.GetNodeCount(); i++)
			{
				Node node = graph.GetNodeAt(i);
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
			Node node = (Node) graph.CreateNode(nodeType);
			var position = ProjectToCanvas(windowPosition);
			node.X = position.x;
			node.Y = position.y;
			graph.AddNode(node);
			return node;
		}

		public void RemoveFocusedNode()
		{

			Node node = GetFocusedNode();
			if (node != null) graph.RemoveNode(node);
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


