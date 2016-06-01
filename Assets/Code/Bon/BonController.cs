using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;

namespace Assets.Code.Bon
{
	public class BonController
	{


		public void OnWindowOpen()
		{
			Debug.Log("OnWindowOpen");
		}

		// =====  Graph Initialization =====
		// =================================

		public Graph.Graph LoadGraph(string graphId)
		{
			Debug.ClearDeveloperConsole();

			Graph.Graph graph = new Graph.Graph();

			var samplerNode01 = new SamplerNode(graph.GetUniqueId());
			samplerNode01.X = 20;
			samplerNode01.Y = 20;
			graph.nodes.Add(samplerNode01);

			var multiplexer01 = new Multiplexer(graph.GetUniqueId());
			multiplexer01.X = 200;
			multiplexer01.Y = 200;
			graph.nodes.Add(multiplexer01);

			Link(samplerNode01.GetSocket(Color.red, 1), multiplexer01.GetSocket(Color.red, 0));

			graph.id = graphId;

			// test serialization an deserialization
			string serializedJSON = JsonUtility.ToJson(graph);
			Graph.Graph deserializedGraph = JsonUtility.FromJson<Graph.Graph>(serializedJSON);

			return deserializedGraph;
		}


		public void SaveGraph(Graph.Graph g, string graphId)
		{


		}


		public Dictionary<string, Type> CreateMenuEntries(string graphId)
		{
			Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();
			menuEntries.Add("Standard/SamplerNode", 	typeof(SamplerNode));
			menuEntries.Add("Standard/Multiplex", 	typeof(Multiplexer));
			return menuEntries;
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
		// move to Graph class

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
			if (ownSocket == null || foreignSocket == null)
			{
				Debug.LogWarning("Try to link sockets but at least one socket does not exist.");
				return false;
			}
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


