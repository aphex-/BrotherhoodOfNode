using UnityEngine;
using System.Collections;
using Assets.Code.Bon.Graph;

public interface IGraphListener : INodeListener
{
	void OnLink(Edge edge);
	void OnUnLink(Socket s01, Socket s02);

	void OnNodeAdded(Node node);
	void OnNodeRemoved(Node node);
}
