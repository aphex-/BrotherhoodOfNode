using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Assets.Code.Bon
{
	public class BonController : IGraphListener
	{


		public void OnWindowOpen()
		{
			Debug.Log("OnWindowOpen");
		}

		// =====  Graph Initialization =====
		public Graph.Graph LoadGraph(string path)
		{
			Debug.ClearDeveloperConsole();

			if (path.Equals(BonConfig.DefaultGraphName))
			{
				Graph.Graph graph = new Graph.Graph();
				graph.RegisterListener(this);

				var numberNode01 = new NumberNode(graph.GetUniqueId());
				numberNode01.X = 20;
				numberNode01.Y = 20;
				graph.nodes.Add(numberNode01);

				var multiplexer01 = new Multiplexer(graph.GetUniqueId());
				multiplexer01.X = 200;
				multiplexer01.Y = 200;
				graph.nodes.Add(multiplexer01);

				graph.Link(numberNode01.GetSocket(Color.red, 0), multiplexer01.GetSocket(Color.red, 0));


				// test serialization an deserialization
				string serializedJSON = graph.ToJson();
				Graph.Graph deserializedGraph = Graph.Graph.FromJson(serializedJSON);

				return deserializedGraph;
			}
			else
			{
				Graph.Graph graph = Graph.Graph.Load(path);
				graph.RegisterListener(this);
				return graph;
			}

		}


		public void SaveGraph(Graph.Graph g, string path)
		{
			Graph.Graph.Save(path, g);
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


