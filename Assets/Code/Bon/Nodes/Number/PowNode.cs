using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Number", "Pow")]
public class PowNode : AbstractNumberNode
{

	private Socket _valueSocket01;
	private Socket _valueSocket02;

	public PowNode(int id, Graph parent) : base(id, parent)
	{
		_valueSocket01 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		_valueSocket02 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_valueSocket01);
		Sockets.Add(_valueSocket02);
		Width = 50;
		Height = 60;
	}

	public override void OnGUI()
	{

	}

	public override void Update()
	{

	}

	public override object GetResultOf(Socket outSocket)
	{
		return GetSampleAt(_x, _y, _seed);
	}

	public override float GetSampleAt(float x, float y, float seed)
	{
		float v1 = GetInputNumber(_valueSocket01, x, y, seed);
		float v2 = GetInputNumber(_valueSocket02, x, y, seed);
		if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;
		return (float) Math.Pow(v1, v2);
	}
}
