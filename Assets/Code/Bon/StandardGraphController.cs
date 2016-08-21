using UnityEngine;

namespace Assets.Code.Bon
{
	public class StandardGraphController
	{

		public StandardGraphController()
		{
		}

		public void Register()
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
			Log.Info("OnFocus " + graph);
		}

		public void OnClose(Graph graph)
		{
			Log.Info("OnClose " + graph);
		}

		// ======= Events =======
		public void OnLink(Graph graph, Edge edge)
		{
			Log.Info("OnLink: Node " + edge.Output.Parent.Id + " with Node " + edge.Input.Parent.Id);
			graph.UpdateDependingNodes(edge.Output.Parent);
		}

		public void OnUnLink(Graph graph, AbstractSocket s01, AbstractSocket s02)
		{
			// Log.Info("OnUnLink: Node " + s01.Edge.Output.Parent.Id + " from Node " + s02.Edge.Input.Parent.Id);
		}

		public void OnUnLinked(Graph graph, AbstractSocket s01, AbstractSocket s02)
		{
			Log.Info("OnUnLinked: Socket " + s02 + " and Socket " + s02);
			var input = s01.IsInput () ? s01 : s02;
			graph.UpdateDependingNodes(input.Parent);
		}

		public void OnNodeAdded(Graph graph, Node node)
		{
			Log.Info("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeRemoved(Graph graph, Node node)
		{
			Log.Info("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeChanged(Graph graph, Node node)
		{
			Log.Info("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
			graph.UpdateDependingNodes(node);
		}

		public void OnFocusNode(Graph graph, Node node)
		{
			Log.Info("OnFocus: " + node.Id);
		}
	}
}
