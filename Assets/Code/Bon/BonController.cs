using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;

namespace Assets.Code.Bon
{
	public class BonController : IGraphListener
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
			graph.RegisterListener(this);

			var samplerNode01 = new SamplerNode(graph.GetUniqueId());
			samplerNode01.X = 20;
			samplerNode01.Y = 20;
			graph.nodes.Add(samplerNode01);

			var multiplexer01 = new Multiplexer(graph.GetUniqueId());
			multiplexer01.X = 200;
			multiplexer01.Y = 200;
			graph.nodes.Add(multiplexer01);

			graph.Link(samplerNode01.GetSocket(Color.red, 1), multiplexer01.GetSocket(Color.red, 0));

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

		public void OnLink(Edge edge)
		{
			Debug.Log("OnLink: Node " + edge.Source.Parent.Id + " with Node " + edge.Sink.Parent.Id);
		}

		public void OnUnLink(Socket s01, Socket s02)
		{
			Debug.Log("OnUnLink: Node " + s01.Edge.Source.Parent.Id + " from Node " + s02.Edge.Sink.Parent.Id);
		}

		public void OnNodeRemoved(Node node)
		{
			Debug.Log("OnNodeRemoved: Node " + node.Id);
		}


	}
}


