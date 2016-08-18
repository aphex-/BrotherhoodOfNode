using UnityEngine;
using System.Collections;
using Assets.Code.Bon;

public class EventManager
{

	public delegate void GraphAction(Graph graph);
	public delegate void EdgeAction(Graph graph, Edge edge);
	public delegate void SocketAction(Graph graph, AbstractSocket s01, AbstractSocket s02);
	public delegate void NodeAction(Graph graph, Node node);

	public delegate void WindowAction();

	public static event GraphAction OnCreateGraph;
	public static event GraphAction OnFocusGraph;
	public static event GraphAction OnCloseGraph;

	public static event EdgeAction OnLinkEdge;

	public static event SocketAction OnUnLinkSockets;
	public static event SocketAction OnUnLinkedSockets;

	public static event NodeAction OnAddedNode;
	public static event NodeAction OnChangedNode;
	public static event NodeAction OnFocusNode;
	public static event NodeAction OnNodeRemoved;

	public static event WindowAction OnEditorWindowOpen;


	public static void TriggerOnCreateGraph(Graph graph)
	{
		if (OnCreateGraph != null) OnCreateGraph(graph);
	}

	public static void TriggerOnFocusGraph(Graph graph)
	{
		if (OnFocusGraph != null) OnFocusGraph(graph);
	}

	public static void TriggerOnCloseGraph(Graph graph)
	{
		if (OnCloseGraph != null) OnCloseGraph(graph);
	}

	public static void TriggerOnLinkEdge(Graph graph, Edge edge)
	{
		if (OnLinkEdge != null) OnLinkEdge(graph, edge);
	}

	public static void TriggerOnUnLinkSockets(Graph graph, AbstractSocket socket01, AbstractSocket socket02)
	{
		if (OnUnLinkSockets != null) OnUnLinkSockets(graph, socket01, socket02);
	}

	public static void TriggerOnUnLinkedSockets(Graph graph, AbstractSocket socket01, AbstractSocket socket02)
	{
		if (OnUnLinkedSockets != null) OnUnLinkedSockets(graph, socket01, socket02);
	}

	public static void TriggerOnAddedNode(Graph graph, Node node)
	{
		if (OnAddedNode != null) OnAddedNode(graph, node);
	}

	public static void TriggerOnChangedNode(Graph graph, Node node)
	{
		if (OnChangedNode != null) OnChangedNode(graph, node);
	}

	public static void TriggerOnFocusNode(Graph graph, Node node)
	{
		if (OnFocusNode != null) OnFocusNode(graph, node);
	}

	public static void TriggerOnNodeRemoved(Graph graph, Node node)
	{
		if (OnNodeRemoved != null) OnNodeRemoved(graph, node);
	}

	public static void TriggerOnWindowOpen()
	{
		if (OnEditorWindowOpen != null) OnEditorWindowOpen();
	}
}
