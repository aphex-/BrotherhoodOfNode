using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes;
using UnityEngine;

namespace Assets.Code.Bon
{
	public class StandardGraphController
	{

		public StandardGraphController()
		{
			EventManager.OnCreateGraph += OnCreate;
			EventManager.OnFocusGraph += OnFocus;
			EventManager.OnCloseGraph += OnClose;
			EventManager.OnLinkEdge += OnLink;
			EventManager.OnUnLinkSockets += OnUnLink;
			EventManager.OnUnLinkedSockets += OnUnLinked;
			EventManager.OnAddedNode += OnNodeAdded;
			EventManager.OnNodeRemoved += OnNodeRemoved;
			EventManager.OnChangedNode += OnNodeChanged;
			EventManager.OnFocusNode += OnFocusNode;
			EventManager.OnEditorWindowOpen += OnWindowOpen;
		}

		private void OnWindowOpen()
		{

		}

		public void OnCreate(Graph graph)
		{

			graph.UpdateNodes();
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
		public void OnLink(Graph graph, Edge edge)
		{
			Debug.Log("OnLink: Node " + edge.Output.Parent.Id + " with Node " + edge.Input.Parent.Id);
			graph.UpdateNodes();
		}

		public void OnUnLink(Graph graph, Socket s01, Socket s02)
		{
			Debug.Log("OnUnLink: Node " + s01.Edge.Output.Parent.Id + " from Node " + s02.Edge.Input.Parent.Id);
		}

		public void OnUnLinked(Graph graph, Socket s01, Socket s02)
		{
			Debug.Log("OnUnLinked: Socket " + s02 + " and Socket " + s02);
			graph.UpdateNodes();
		}

		public void OnNodeAdded(Graph graph, Node node)
		{
			Debug.Log("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeRemoved(Graph graph, Node node)
		{
			Debug.Log("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeChanged(Graph graph, Node node)
		{
			Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
			graph.ForceUpdateNodes();
		}

		public void OnFocusNode(Graph graph, Node node)
		{
			Debug.Log("OnFocus: " + node.Id);
		}
	}
}
