using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon
{
	public class StandardGraphController : IGraphListener
	{

		private Graph _graph;

		public void OnCreate(Graph graph)
		{
			this._graph = graph;
			_graph.UpdateNodes();
		}

		public void OnFocus(Graph graph)
		{
			Debug.Log("OnFocus " + graph);
		}

		public void OnClose(Graph graph)
		{
			Debug.Log("OnClose " + graph);
		}

		// ======= Events =======
		public void OnLink(Edge edge)
		{
			Debug.Log("OnLink: Node " + edge.Output.Parent.Id + " with Node " + edge.Input.Parent.Id);
			_graph.UpdateNodes();
		}

		public void OnUnLink(Socket s01, Socket s02)
		{
			Debug.Log("OnUnLink: Node " + s01.Edge.Output.Parent.Id + " from Node " + s02.Edge.Input.Parent.Id);
		}

		public void OnUnLinked(Socket s01, Socket s02)
		{
			Debug.Log("OnUnLinked: Socket " + s02 + " and Socket " + s02);
			_graph.ForceUpdateNodes();
		}

		public void OnNodeAdded(Node node)
		{
			Debug.Log("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
			_graph.UpdateNodes();
		}

		public void OnNodeRemoved(Node node)
		{
			Debug.Log("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
			_graph.UpdateNodes();
		}

		public void OnNodeChanged(Node node)
		{
			Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
			_graph.ForceUpdateNodes();
		}

		public void OnFocus(Node node)
		{
			Debug.Log("OnFocus: " + node.Id);
		}
	}
}
