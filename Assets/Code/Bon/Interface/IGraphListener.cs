
using System.ComponentModel.Design;

namespace Assets.Code.Bon.Interface
{
	public interface IGraphListener : INodeListener
	{
		void OnCreate(Graph graph);
		void OnFocus(Graph graph);
		void OnClose(Graph graph);

		void OnLink(Graph graph, Edge edge);
		void OnUnLink(Graph graph, Socket s01, Socket s02);
		void OnUnLinked(Graph graph, Socket s01, Socket s02);

		void OnNodeAdded(Graph graph, Node node);
		void OnNodeRemoved(Graph graph, Node node);
	}
}
