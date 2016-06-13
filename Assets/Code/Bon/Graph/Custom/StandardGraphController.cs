using UnityEngine;
using System.Collections;
using Assets.Code.Bon.Graph;

public class StandardGraphController : IGraphListener
{

	private Graph graph;

	public void OnCreate(Graph graph)
	{
		this.graph = graph;
	}

	// ======= Events =======
	public void OnLink(Edge edge)
	{
		Debug.Log("OnLink: Node " + edge.Source.Parent.Id + " with Node " + edge.Sink.Parent.Id);
		UpdateDisplayNodes();
	}

	public void OnUnLink(Socket s01, Socket s02)
	{
		Debug.Log("OnUnLink: Node " + s01.Edge.Source.Parent.Id + " from Node " + s02.Edge.Sink.Parent.Id);
		UpdateDisplayNodes();
	}

	public void OnUnLinked(Socket s01, Socket s02)
	{
		Debug.Log("OnUnLinked: Socket " + s02 + " and Socket " + s02);
		UpdateDisplayNodes();
	}

	public void OnNodeAdded(Node node)
	{
		Debug.Log("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
		UpdateDisplayNodes();
	}

	public void OnNodeRemoved(Node node)
	{
		Debug.Log("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
		UpdateDisplayNodes();
	}

	public void OnNodeChanged(Node node)
	{
		Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
		UpdateDisplayNodes();
	}

	private void UpdateDisplayNodes()
	{
		for (var i = 0; i < graph.GetNodeCount(); i++)
		{
			Node n = graph.GetNodeAt(i);
			if (typeof(MathDisplay) == n.GetType())
			{
				((MathDisplay) n).UpdateValue();
			}
		}
	}

}
