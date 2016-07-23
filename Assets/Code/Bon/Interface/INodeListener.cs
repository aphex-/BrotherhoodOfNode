namespace Assets.Code.Bon.Interface
{
	public interface INodeListener {
		void OnNodeChanged(Graph graph, Node node);
		void OnFocus(Graph graph, Node node);
	}
}
