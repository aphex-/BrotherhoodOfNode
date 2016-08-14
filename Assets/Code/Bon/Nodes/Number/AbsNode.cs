using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;

[Serializable]
[GraphContextMenuItem("Number", "Abs")]
public class AbsNode : AbstractNumberNode
{
	private Socket _inputSocket;

	public AbsNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_inputSocket);
		Width = 40;
		Height = 40;
	}

	public override void OnGUI()
	{

	}

	public override object GetResultOf(Socket outSocket)
	{
		return GetSampleAt(_x, _y, _seed);
	}

	public override float GetSampleAt(float x, float y, float seed)
	{
		var number = GetInputNumber(_inputSocket, x, y, seed);
		if (float.IsNaN(number)) return float.NaN;
		return Math.Abs(number);
	}
}
