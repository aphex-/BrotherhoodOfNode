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
			UpdateNodes();
		}

		// ======= Events =======
		public void OnLink(Edge edge)
		{
			//Debug.Log("OnLink: Node " + edge.Output.Parent.Id + " with Node " + edge.Input.Parent.Id);
			UpdateNodes();
		}

		public void OnUnLink(Socket s01, Socket s02)
		{
			//Debug.Log("OnUnLink: Node " + s01.Edge.Output.Parent.Id + " from Node " + s02.Edge.Input.Parent.Id);
			UpdateNodes();
		}

		public void OnUnLinked(Socket s01, Socket s02)
		{
			//Debug.Log("OnUnLinked: Socket " + s02 + " and Socket " + s02);
			UpdateNodes();
		}

		public void OnNodeAdded(Node node)
		{
			//Debug.Log("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
			UpdateNodes();
		}

		public void OnNodeRemoved(Node node)
		{
			//Debug.Log("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
			UpdateNodes();
		}

		public void OnNodeChanged(Node node)
		{
			//Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
			UpdateNodes();
		}

		private void UpdateNodes()
		{
			if (_graph.HasCicle())
			{
				_graph.LogCircleError();
				return;
			}

			for (var i = 0; i < _graph.GetNodeCount(); i++)
			{
				Node n = _graph.GetNodeAt(i);
				var updateable = n as IUpdateable;
				if (updateable != null)
				{
					updateable.Update();
				}
			}
		}

	}
}
