using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Bon
{

	public class Edge
	{
		public Socket Input;
		public Socket Output;

		// cached vectors for drawing
		private Vector2 tmpStartPos;
		private Vector2 tmpEndPos;
		private Vector2 tmpTangent01;
		private Vector2 tmpTangent02;

		public Edge(Socket socket01, Socket socket02)
		{
			if (socket01.Direction == SocketDirection.Input)
			{
				Input = socket01;
				Output = socket02;
			}
			else
			{
				Input = socket02;
				Output = socket01;
			}
		}

		public Socket GetOtherSocket(Socket socket)
		{
			if (socket == Input) return Output;
			else return Input;
		}

		public void Draw()
		{
			if (Input != null && Output != null)
			{
				tmpStartPos = GetEdgePosition(Output, tmpStartPos);
				tmpEndPos = GetEdgePosition(Input, tmpEndPos);
				tmpTangent01 = GetTangentPosition(Output, tmpStartPos);
				tmpTangent02 = GetTangentPosition(Input, tmpEndPos);
				DrawEdge(tmpStartPos, tmpTangent01, tmpEndPos, tmpTangent02, Output.Type);

				Handles.color = Color.black;
				tmpStartPos.Set(tmpEndPos.x - 5, tmpEndPos.y - 5);
				Handles.DrawLine(tmpEndPos, tmpStartPos);
				tmpStartPos.Set(tmpEndPos.x - 5, tmpEndPos.y + 5);
				Handles.DrawLine(tmpEndPos, tmpStartPos);
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
			if (socket.Direction == SocketDirection.Output)
			{
				width = BonConfig.SocketSize;
			}
			position.Set(
				socket.X + width,
				socket.Y + (BonConfig.SocketSize/2f));
			return position;
		}

		public static Vector2 GetTangentPosition(Socket socket, Vector2 position)
		{
			if (socket.Direction == SocketDirection.Input)
				return position + Vector2.left*BonConfig.EdgeTangent;
			else
				return position + Vector2.right*BonConfig.EdgeTangent;
		}

		///<summary>Creates a serializable version of this edge.</summary>
		/// <returns>A serializable version of this edge.</returns>
		public SerializableEdge ToSerializedEgde()
		{
			SerializableEdge s = new SerializableEdge();
			s.InputNodeId = Input.Parent.Id;
			s.InputSocketIndex = Input.Parent.Sockets.IndexOf(Input);
			s.OutputNodeId = Output.Parent.Id;
			s.OutputSocketIndex = Output.Parent.Sockets.IndexOf(Output);
			return s;
		}
	}


	[Serializable] public class SerializableEdge
	{
		[SerializeField] public int OutputNodeId = -1;
		[SerializeField] public int OutputSocketIndex = -1;
		[SerializeField] public int InputNodeId = -1;
		[SerializeField] public int InputSocketIndex = -1;
	}
}


