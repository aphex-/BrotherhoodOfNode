
namespace Assets.Code.Bon.Interface
{
	public interface IGraphListener : INodeListener
	{
		void OnCreate(Graph graph);

		void OnLink(Edge edge);
		void OnUnLink(Socket s01, Socket s02);
		void OnUnLinked(Socket s01, Socket s02);

		void OnNodeAdded(Node node);
		void OnNodeRemoved(Node node);
	}
}
