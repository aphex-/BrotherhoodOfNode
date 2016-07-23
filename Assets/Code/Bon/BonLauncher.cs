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

		private List<Graph> _graphs;
		private IGraphListener _listener;


		public List<Graph> Graphs
		{
			get
			{
				if (_graphs == null) _graphs = new List<Graph>();
				return _graphs;
			}
		}

		public IGraphListener Listener
		{
			get
			{
				if (_listener == null) _listener = new StandardGraphController(); // use this implementation or add your own listsner
				return _listener;
			}
		}

		public void OnWindowOpen()
		{

		}

		// <summary> Gets called if the editor opens a new Graph </summary>
		public Graph LoadGraph(string path)
		{
			Graph g;
			if (path.Equals(BonConfig.DefaultGraphName)) g = CreateDefaultGraph();
			else g = Graph.Load(path, Listener);
			Graphs.Add(g);
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
			Graphs.Remove(g);
			Listener.OnClose(g);
		}

		// <summary> Gets the graph at the index </summary>
		public Graph GetGraph(int index)
		{
			return Graphs[index];
		}

		public void OnFocus(Graph graph)
		{
			Listener.OnFocus(graph);
		}

		// <summary> Creates a default Graph. Could also be an empty Graph </summary>
		public Graph CreateDefaultGraph()
		{
			Graph graph = new Graph();


			var numberNode01 = (NumberNode) graph.CreateNode<NumberNode>();
			numberNode01.X = 20;
			numberNode01.Y = 20;
			numberNode01.Number = "355";
			graph.AddNode(numberNode01);

			var numberNode02 = (NumberNode) graph.CreateNode<NumberNode>();
			numberNode02.X = 20;
			numberNode02.Y = 80;
			numberNode02.Number = "113";
			graph.AddNode(numberNode02);

			var operator01 = (MathOperatorNode) graph.CreateNode<MathOperatorNode>();
			operator01.X = 200;
			operator01.Y = 40;
			operator01.SetMode(Operator.Divide);
			graph.AddNode(operator01);

			var diplay01 = (MathDisplay) graph.CreateNode<MathDisplay>();
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

			var perlinNoise = graph.CreateNode<PerlinNoiseNode>();
			perlinNoise.X = 80;
			perlinNoise.Y = 250;
			graph.AddNode(perlinNoise);

			var displayMap = graph.CreateNode<MapDisplayNode>();
			displayMap.X = 300;
			displayMap.Y = 280;
			graph.AddNode(displayMap);

			graph.Link(perlinNoise.GetSocket(NumberNode.FloatType, SocketDirection.Output, 0),
				displayMap.GetSocket(NumberNode.FloatType, SocketDirection.Input, 0));

			// == test serialization an deserialization ==
			var serializedJSON = graph.ToJson();
			var deserializedGraph = Graph.FromJson(serializedJSON, Listener);
			// =====

			return deserializedGraph;
		}

	}
}


