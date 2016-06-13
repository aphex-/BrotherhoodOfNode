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
	public class BonController
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

				var numberNode01 = new NumberNode(graph.GetUniqueId());
				numberNode01.X = 20;
				numberNode01.Y = 20;
				graph.AddNode(numberNode01);

				var operator01 = new MathOperatorNode(graph.GetUniqueId());
				operator01.X = 200;
				operator01.Y = 200;
				graph.AddNode(operator01);

				graph.Link(
					numberNode01.GetSocket(Color.red, SocketDirection.Output, 0),
					operator01.GetSocket(Color.red, SocketDirection.Input, 0));


				// test serialization an deserialization
				string serializedJSON = graph.ToJson();
				Graph.Graph deserializedGraph = Graph.Graph.FromJson(serializedJSON, new StandardGraphController());

				return deserializedGraph;
			}
			else
			{
				Graph.Graph graph = Graph.Graph.Load(path, new StandardGraphController());
				return graph;
			}

		}


		public void SaveGraph(Graph.Graph g, string path)
		{
			Graph.Graph.Save(path, g);
		}

	}
}


