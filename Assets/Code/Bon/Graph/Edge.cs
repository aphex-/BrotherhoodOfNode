using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Bon.Graph
{

	public class Edge
	{
		public Socket Source;
		public Socket Sink;


		public Edge(Socket source, Socket sink)
		{
			Source = source;
			Sink = sink;
		}

		private Vector2 tmpStartPos;
		private Vector2 tmpEndPos;
		private Vector2 tmpTangent01;
		private Vector2 tmpTangent02;


		public Socket GetOtherSocket(Socket socket)
		{
			if (socket == Sink) return Source;
			else return Sink;
		}

		public void Draw()
		{
			if (Source != null && Sink != null)
			{
				tmpStartPos = GetEdgePosition(Source, tmpStartPos);
				tmpEndPos = GetEdgePosition(Sink, tmpEndPos);
				tmpTangent01 = GetTangentPosition(Source, tmpStartPos);
				tmpTangent02 = GetTangentPosition(Sink, tmpEndPos);
				DrawEdge(tmpStartPos, tmpTangent01, tmpEndPos, tmpTangent02, Source.Type);
			}
		}

		public static void DrawEdge(Vector2 position01, Vector2 tangent01, Vector2 position02, Vector2 tangent02, Color c)
		{
			Handles.DrawBezier(
				position01, position02,
				tangent01, tangent02, Color.black, null, 6);

			Handles.DrawBezier(
				position01, position02,
				tangent01, tangent02, c, null, 3);
		}

		public static Vector2 GetEdgePosition(Socket socket, Vector2 position)
		{
			float width = 0;
			if (!socket.AlignLeft)
			{
				width = BonConfig.SocketSize;
			}
			position.Set(
				socket.X + width + socket.Parent.X,
				socket.Y + (BonConfig.SocketSize/2f) + socket.Parent.Y);
			return position;
		}

		public static Vector2 GetTangentPosition(Socket socket, Vector2 position)
		{
			if (socket.AlignLeft)
				return position + Vector2.left*BonConfig.EdgeTangent;
			else
				return position + Vector2.right*BonConfig.EdgeTangent;
		}
	}
}


