using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon.Graph;

namespace Assets.Editor
{
	public class BonCanvas
	{
		public List<Node> Nodes;

		public readonly GUIStyle Style = new GUIStyle();

		public Rect DrawArea = new Rect();
		public float Zoom = 1;
		public Vector2 Position = new Vector2();



		public BonCanvas()
		{
			Nodes = new List<Node>();
			Style.normal.background = Texture2D.whiteTexture;
		}

		public void DrawNodes()
		{
			foreach (Node node in Nodes)
			{
				node.GUIDrawWindow();
			}
		}

		public void DrawEdges()
		{
			foreach (Node node in Nodes)
			{
				node.GUIDrawEdges();
			}
		}

	}

}


