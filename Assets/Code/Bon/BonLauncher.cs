using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Linq;
using Assets.Code.Bon.Nodes.Map;
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

				// create a default grpah programmatically
				var numberNode01 = new NumberNode(graph.GetUniqueId());
				numberNode01.X = 20;
				numberNode01.Y = 20;
				numberNode01.Number = "355";
				graph.AddNode(numberNode01);

				var numberNode02 = new NumberNode(graph.GetUniqueId());
				numberNode02.X = 20;
				numberNode02.Y = 80;
				numberNode02.Number = "113";
				graph.AddNode(numberNode02);

				var operator01 = new MathOperatorNode(graph.GetUniqueId());
				operator01.X = 200;
				operator01.Y = 40;
				operator01.SetMode(Operator.Divide);
				graph.AddNode(operator01);

				var diplay01 = new MathDisplay(graph.GetUniqueId());
				diplay01.X = 330;
				diplay01.Y = 80;
				graph.AddNode(diplay01);

				graph.Link(
					numberNode01.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
					operator01.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));

				graph.Link(
					numberNode02.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
					operator01.GetSocket(NumberNode.FloatType, SocketDirection.Input, 1));

				graph.Link(
					operator01.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
					diplay01.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));


				var perlinNoise = new PerlinNoiseNode(graph.GetUniqueId());
				perlinNoise.X = 80;
				perlinNoise.Y = 250;
				graph.AddNode(perlinNoise);

				var displayMap = new MapDisplayNode(graph.GetUniqueId());
				displayMap.X = 300;
				displayMap.Y = 280;
				graph.AddNode(displayMap);

				graph.Link(perlinNoise.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
					displayMap.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));

				// == test serialization an deserialization ==
				var serializedJSON = graph.ToJson();
				var deserializedGraph = Graph.FromJson(serializedJSON, new StandardGraphController());
				// =====

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


