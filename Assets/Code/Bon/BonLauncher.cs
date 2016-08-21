using System.Collections.Generic;
using UnityEngine;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Nodes.Noise;
using Assets.Code.Bon.Nodes.Number;
using Assets.Code.Bon.Socket;


namespace Assets.Code.Bon
{
	/// <summary>
	/// A class to controll the creation of Graphs. It contains loaded Grpahs.
	/// (A gameobject with this script is created by the editor if it is not in the scene)
	/// </summary>
	[ExecuteInEditMode]
	[System.Serializable]
	public class BonLauncher : MonoBehaviour
	{

		private List<Graph> _graphs = new List<Graph>();

		private StandardGraphController _controller;

		public List<Graph> Graphs
		{
			get { return _graphs; }
		}

		/// <summary>
		/// Loads a graph by its path, adds it to the internal list
		/// and returns it.
		/// (Also used by the editor to open Graphs)
		/// </summary>
		public Graph LoadGraph(string path)
		{
			Graph g;
			if (path.Equals(BonConfig.DefaultGraphName)) g = CreateDefaultGraph();
			else g = Graph.Load(path);
			g.Name = path;
			Graphs.Add(g);
			CreateGraphController(g);
			g.UpdateNodes();
			return g;
		}

		/// <summary>
		/// Saves a graph by its path.
		/// (Also used by the editor to save Graphs)
		/// </summary>
		public void SaveGraph(Graph g, string path)
		{
			Graph.Save(path, g);
		}

		/// <summary>
		/// Removes a Graph from the internal list.
		/// (Also used by the editor to close Graphs)
		/// </summary>
		public void RemoveGraph(Graph g)
		{
			Graphs.Remove(g);
		}

		/// <summary>
		/// Returns the graph at the index
		/// </summary>
		public Graph GetGraph(int index)
		{
			return Graphs[index];
		}

		/// <summary>
		/// Create a controller for the assigned Graph.
		/// </summary>
		private void CreateGraphController(Graph graph)
		{
			// in this case we create one controller for all graphs
			// you could also create different controllers for different graphs
			//if (_controller == null) _controller = new StandardGraphController();
		}

		public void OnEnable()
		{
			if (_controller == null) _controller = new StandardGraphController();
			_controller.Register();

			foreach (var graph in Graphs)
			{
				graph.ResetVisitCount();
			}
		}

		/// <summary>
		/// Creates a default Graph.
		/// (see: BonConfig.DefaultGraphName)
		/// </summary>
		public Graph CreateDefaultGraph()
		{
			Graph graph = new Graph();

			// Number Nodes
			var operator01 = (NumberOperatorNode) graph.CreateNode<NumberOperatorNode>();
			operator01.X = 200;
			operator01.Y = 40;
			operator01.SetMode(Operator.Add);
			graph.AddNode(operator01);

			var diplay01 = (NumberDisplayNode) graph.CreateNode<NumberDisplayNode>();
			diplay01.X = 330;
			diplay01.Y = 80;
			graph.AddNode(diplay01);

			graph.Link(
				(InputSocket) diplay01.GetSocket(typeof(AbstractNumberNode), typeof(InputSocket), 0),
				(OutputSocket) operator01.GetSocket(typeof(AbstractNumberNode), typeof(OutputSocket), 0));

			// Map2D Nodes
			var perlinNoise = graph.CreateNode<UnityPerlinNoiseNode>();
			perlinNoise.X = 80;
			perlinNoise.Y = 250;
			graph.AddNode(perlinNoise);

			var displayMap = graph.CreateNode<NoiseDisplayNode>();
			displayMap.X = 300;
			displayMap.Y = 280;
			graph.AddNode(displayMap);

			graph.Link(
				(InputSocket) displayMap.GetSocket(typeof(AbstractNumberNode), typeof(InputSocket), 0),
				(OutputSocket) perlinNoise.GetSocket(typeof(AbstractNumberNode), typeof(OutputSocket), 0));


			// == test serialization an deserialization ==
			var serializedJSON = graph.ToJson();
			var deserializedGraph = Graph.FromJson(serializedJSON);
			// =====

			return deserializedGraph;
		}

	}
}


