using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;

[Serializable]
[GraphContextMenuItem("Number", "Abs")]
public class AbsNode : AbstractNumberNode
{
	private InputSocket _inputSocket;

	public AbsNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocket = new InputSocket(this, typeof(AbstractNumberNode));
		Sockets.Add(_inputSocket);
		Width = 40;
		Height = 40;
	}

	public override void OnGUI()
	{

	}

	public override void Update()
	{

	}

	public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
	{
		var number = GetInputNumber(_inputSocket, x, y, z, seed);
		if (float.IsNaN(number)) return float.NaN;
		return Math.Abs(number);
	}
}
