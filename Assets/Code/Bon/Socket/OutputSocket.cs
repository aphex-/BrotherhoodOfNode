using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Socket
{
	public class OutputSocket : AbstractSocket
	{

		public List<Edge> Edges;

		public OutputSocket(Node parent, Type type) : base(parent, type)
		{
			Edges = new List<Edge>();
		}

		public override bool IsConnected()
		{
			return Edges.Count > 0;
		}

		public override bool Intersects(Vector2 nodePosition)
		{
			if (Parent.Collapsed) return false;
			return BoxRect.Contains(nodePosition);
		}

		protected override void OnDraw()
		{
			GUI.Box(BoxRect, ">");
		}
	}
}
