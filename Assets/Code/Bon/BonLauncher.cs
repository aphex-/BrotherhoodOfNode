using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Linq;
using Assets.Code.Bon.Nodes.Math;

namespace Assets.Code.Bon
{
	public class BonLauncher
	{


		public void OnWindowOpen()
		{
			Debug.Log("OnWindowOpen");
		}

		// =====  Graph Initialization =====
		public Graph LoadGraph(string path)
		{
			Debug.ClearDeveloperConsole();

			if (path.Equals(BonConfig.DefaultGraphName))
			{
				Graph graph = new Graph();

				var numberNode01 = new NumberNode(graph.GetUniqueId());
				numberNode01.X = 20;
				numberNode01.Y = 20;
				graph.AddNode(numberNode01);

				var operator01 = new MathOperatorNode(graph.GetUniqueId());
				operator01.X = 200;
				operator01.Y = 200;
				graph.AddNode(operator01);

				graph.Link(
					numberNode01.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
					operator01.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));


				// test serialization an deserialization
				string serializedJSON = graph.ToJson();
				Graph deserializedGraph = Graph.FromJson(serializedJSON, new StandardGraphController());

				return deserializedGraph;
			}
			else
			{
				Graph graph = Graph.Load(path, new StandardGraphController());
				return graph;
			}

		}


		public void SaveGraph(Graph g, string path)
		{
			Graph.Save(path, g);
		}

	}
}


