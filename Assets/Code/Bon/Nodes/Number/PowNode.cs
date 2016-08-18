using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Number", "Pow")]
public class PowNode : AbstractNumberNode
{

	private InputSocket _valueSocket01;
	private InputSocket _valueSocket02;

	public PowNode(int id, Graph parent) : base(id, parent)
	{
		_valueSocket01 = new InputSocket(this, typeof(AbstractNumberNode));
		_valueSocket02 = new InputSocket(this, typeof(AbstractNumberNode));
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

	public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
	{
		float v1 = GetInputNumber(_valueSocket01, x, y, z, seed);
		float v2 = GetInputNumber(_valueSocket02, x, y, z, seed);
		if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;
		return (float) Math.Pow(v1, v2);
	}
}
