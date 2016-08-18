using System;
using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Vector", "Split")]
public class SplitNode : AbstractVector3Node
{
	private InputSocket _inputSocketVector;
	private InputSocket _inputSocketMask;

	private OutputSocket _outputSocket01;
	private OutputSocket _outputSocket02;

	private Rect _tmpRect;

	public SplitNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocketMask = new InputSocket(this, typeof(AbstractNumberNode));
		_inputSocketVector = new InputSocket(this, typeof(AbstractVector3Node));

		_outputSocket01 = new OutputSocket(this, typeof(AbstractVector3Node));
		_outputSocket02 = new OutputSocket(this, typeof(AbstractVector3Node));

		Sockets.Add(_inputSocketVector);
		Sockets.Add(_inputSocketMask);

		Sockets.Add(_outputSocket01);
		Sockets.Add(_outputSocket02);
		Height = 60;

	}

	public override void OnGUI()
	{
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		_tmpRect.Set(3, 0, 40, 20);
		GUI.Label(_tmpRect, "vec");

		_tmpRect.Set(3, 20, 40, 20);
		GUI.Label(_tmpRect, "mask");

		GUI.skin.label.alignment = TextAnchor.MiddleRight;
		_tmpRect.Set(45, 0, 50, 20);
		GUI.Label(_tmpRect, ">=0");

		_tmpRect.Set(45, 20, 50, 20);
		GUI.Label(_tmpRect, "<0");

		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
	}

	public override void Update()
	{

	}

	public override List<Vector3> GetVector3List(OutputSocket outSocket, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed)
	{
		List<Vector3> vec = GetInputVector3List(_inputSocketVector, x, y, z, sizeX, sizeY, sizeZ, seed);
		if (vec == null) return null;

		List<Vector3> removeList = new List<Vector3>();

		for (var i = 0; i < vec.Count; i++)
		{
			float maskValue = AbstractNumberNode.GetInputNumber(_inputSocketMask, vec[i].x, vec[i].y, vec[i].z, seed);
			if (maskValue < 0 && outSocket == _outputSocket01) removeList.Add(vec[i]);
			if (maskValue >= 0 && outSocket == _outputSocket02) removeList.Add(vec[i]);
		}

		foreach (var r in removeList) vec.Remove(r);
		return vec;
	}
}
