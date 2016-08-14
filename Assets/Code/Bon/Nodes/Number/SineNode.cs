using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;

[Serializable]
[GraphContextMenuItem("Number", "Sine")]
public class SineNode : AbstractNumberNode
{

	private Socket _inputSocket;

	public SineNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_inputSocket);
		Width = 50;
		Height = 50;
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
		return (float) Math.Sin(number);
	}
}
