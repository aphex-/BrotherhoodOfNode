namespace Assets.Code.Bon.Interface
{
	public interface INodeListener {
		void OnNodeChanged(Node node);
		void OnFocus(Node node);
	}
}
