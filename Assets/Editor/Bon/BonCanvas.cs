using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using UnityEngine;

namespace Assets.Editor.Bon
{
	public class BonCanvas
	{
		public readonly GUIStyle Style = new GUIStyle();

		public Rect DrawArea = new Rect();
		public float Zoom = 1;
		public Vector2 Position = new Vector2();
		private Graph graph;

		public BonCanvas(Graph graph)
		{
			this.graph = graph;
			Style.normal.background = Texture2D.whiteTexture;
		}

		public void DrawNodes()
		{
			foreach (Node node in graph.nodes)
			{
				node.GUIDrawWindow();
			}
		}

		public void DrawEdges()
		{
			foreach (Node node in graph.nodes)
			{
				node.GUIDrawEdges();
			}
		}

		public Node GetFocusedNode()
		{
			foreach (Node node in graph.nodes)
			{
				if (node.HasFocus()) return node;
			}
			return null;
		}

		/// <summary> Returns the socket at the window position.</summary>
		/// <param name="windowPosition"> The position to get the Socket from in window coordinates</param>
		/// <returns>The socket at the posiiton or null or null.</returns>
		public Socket GetSocketAt(Vector2 windowPosition)
		{
			Vector2 projectedPosition = ProjectToDrawArea(windowPosition);
			foreach (Node node in graph.nodes)
			{
				if (node.Intersects(projectedPosition))
				{
					Socket socket = node.SearchSocketAt(projectedPosition);
					if (socket != null)
					{
						return socket;
					}
				}
			}
			return null;
		}

		public Vector2 ProjectToDrawArea(Vector2 windowPosition)
		{
			windowPosition.y += (21) - ((BonWindow.TopOffset*2));
			windowPosition = windowPosition/this.Zoom;
			windowPosition.x -= (this.DrawArea.x);
			windowPosition.y -= (this.DrawArea.y);
			return windowPosition;
		}

	}

}


