using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Linq;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Map;
using Assets.Code.Bon.Nodes.Math;


namespace Assets.Code.Bon
{
	[ExecuteInEditMode]
	public class BonLauncher : MonoBehaviour
	{

		private IGraphListener _controller;
		private List<Graph> _graphs;

		void Awake()
		{
			_controller = new StandardGraphController();
			_graphs = new List<Graph>();
		}


		public void OnWindowOpen()
		{

		}

		// <summary> Gets called if the editor opens a new Graph </summary>
		public Graph LoadGraph(string path)
		{
			Graph g;
			if (path.Equals(BonConfig.DefaultGraphName)) g = CreateDefaultGraph();
			else g = Graph.Load(path, _controller);
			_graphs.Add(g);
			return g;
		}

		// <summary> Gets called if the editor requests to save the assigned graph </summary>
		public void SaveGraph(Graph g, string path)
		{
			Graph.Save(path, g);
		}

		// <summary> Gets called if the editor closes a graph </summary>
		public void CloseGraph(Graph g)
		{
			_graphs.Remove(g);
			_controller.OnClose(g);
		}

		// <summary> Gets the graph at the index </summary>
		public Graph GetGraph(int index)
		{
			return _graphs[index];
		}

		public void OnFocus(Graph graph)
		{
			_controller.OnFocus(graph);
		}

		// <summary> Creates a default Graph. Could also be an empty Graph </summary>
		public Graph CreateDefaultGraph()
		{
			Graph graph = new Graph();

			var numberNode01 = new NumberNode(graph.ObtainUniqueNodeId());
			numberNode01.X = 20;
			numberNode01.Y = 20;
			numberNode01.Number = "355";
			graph.AddNode(numberNode01);

			var numberNode02 = new NumberNode(graph.ObtainUniqueNodeId());
			numberNode02.X = 20;
			numberNode02.Y = 80;
			numberNode02.Number = "113";
			graph.AddNode(numberNode02);

			var operator01 = new MathOperatorNode(graph.ObtainUniqueNodeId());
			operator01.X = 200;
			operator01.Y = 40;
			operator01.SetMode(Operator.Divide);
			graph.AddNode(operator01);

			var diplay01 = new MathDisplay(graph.ObtainUniqueNodeId());
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

			var perlinNoise = new PerlinNoiseNode(graph.ObtainUniqueNodeId());
			perlinNoise.X = 80;
			perlinNoise.Y = 250;
			graph.AddNode(perlinNoise);

			var displayMap = new MapDisplayNode(graph.ObtainUniqueNodeId());
			displayMap.X = 300;
			displayMap.Y = 280;
			graph.AddNode(displayMap);

			graph.Link(perlinNoise.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
				displayMap.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));

			// == test serialization an deserialization ==
			var serializedJSON = graph.ToJson();
			var deserializedGraph = Graph.FromJson(serializedJSON, _controller);
			// =====

			return deserializedGraph;
		}

	}
}


