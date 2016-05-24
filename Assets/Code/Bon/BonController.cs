using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;

namespace Assets.Code.Bon
{
	public class BonController
	{

		private int _uniqueIdOffset = 0;

		public void OnWindowOpen()
		{
			Debug.Log("OnWindowOpen");
		}

		// =====  Graph Initialization =====
		// =================================

		public List<Node> LoadGraph(string graphId)
		{
			List<Node> nodes = new List<Node>();
			var samplerNode01 = new SamplerNode(GetUniqueId());
			samplerNode01.X = 20;
			samplerNode01.Y = 20;
			nodes.Add(samplerNode01);

			var samplerNode02 = new SamplerNode(GetUniqueId());
			samplerNode02.X = 200;
			samplerNode02.Y = 200;
			nodes.Add(samplerNode02);

			Link(samplerNode01.GetSocket(Color.red, 0), samplerNode02.GetSocket(Color.red, 0));
			return nodes;
		}

		public List<string> CreateNodeTypeList(string graphiId)
		{
			List<string> nodeTypes = new List<string>();
			nodeTypes.Add("Standard/SamplerNode");
			nodeTypes.Add("Standard/Multiplex");
			nodeTypes.Add("Math/Value");
			return nodeTypes;
		}

		public Node CreateNode(string nodeType)
		{
			return new SamplerNode(GetUniqueId());
		}

		private int GetUniqueId()
		{
			_uniqueIdOffset += 1;
			return _uniqueIdOffset;
		}

		// ======= Events =======
		// ======================

		private static void OnLink(Edge edge)
		{
			Debug.Log("OnLink: Node " + edge.Source.Parent.Id + " with Node " + edge.Sink.Parent.Id);
		}

		private static void OnUnLink(Socket s01, Socket s02)
		{
			Debug.Log("OnUnLink: Node " + s01.Edge.Source.Parent.Id + " from Node " + s02.Edge.Sink.Parent.Id);
		}

		private static void OnNodeRemoved(Node node)
		{
			Debug.Log("OnNodeRemoved: Node " + node.Id);
		}


		// ===== Graph Management =====
		// ============================

		public void RemoveNode(Node node)
		{
			foreach (var socket in node.Sockets)
			{
				if (socket.Edge != null)
				{
					UnLink(socket);
				}
			}
			OnNodeRemoved(node);
		}

		public void UnLink(Socket s01, Socket s02)
		{
			OnUnLink(s01, s02);
			if (s01 != null && s01.Edge != null)
			{
				s01.Edge.Sink = null;
				s01.Edge.Source = null;
				s01.Edge = null;
			}
			if (s02 != null && s02.Edge != null)
			{
				s02.Edge.Sink = null;
				s02.Edge.Source = null;
				s02.Edge = null;
			}
		}

		public void UnLink(Socket socket)
		{
			Socket socket2 = null;
			if (socket.Edge != null)
			{
				socket2 = socket.Edge.GetOtherSocket(socket);
			}
			UnLink(socket, socket2);
		}

		public bool Link(Socket ownSocket, Socket foreignSocket)
		{
			if (ownSocket.Type == foreignSocket.Type)
			{
				Edge edge = new Edge(ownSocket, foreignSocket);
				ownSocket.Edge = edge;
				foreignSocket.Edge = edge;
				OnLink(edge);
			}
			return true;
		}

	}
}


