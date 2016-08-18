using System;
using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Geometry", "Model")]
public class ModelNode : Node
{
	private InputSocket _inputSocketPosition;
	private InputSocket _inputSocketRotation;
	private InputSocket _inputSocketScale;
	private InputSocket _inputSocketModelName;

	private Rect _tmpRect;

	public ModelNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocketPosition = new InputSocket(this, typeof(AbstractVector3Node));
		_inputSocketRotation = new InputSocket(this, typeof(AbstractVector3Node));
		_inputSocketScale = new InputSocket(this, typeof(AbstractVector3Node));
		_inputSocketModelName = new InputSocket(this, typeof(AbstractStringNode));

		Sockets.Add(_inputSocketModelName);
		Sockets.Add(_inputSocketPosition);
		Sockets.Add(_inputSocketRotation);
		Sockets.Add(_inputSocketScale);

		Height = 100;
		_tmpRect = new Rect();
	}

	public override void OnGUI()
	{
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		_tmpRect.Set(3, 0, 50, 20);
		GUI.Label(_tmpRect, "file");

		_tmpRect.Set(3, 20, 50, 20);
		GUI.Label(_tmpRect, "position");

		_tmpRect.Set(3, 40, 50, 20);
		GUI.Label(_tmpRect, "rotation");

		_tmpRect.Set(3, 60, 50, 20);
		GUI.Label(_tmpRect, "scale");

	}

	public string GetModelFileName()
	{
		if (!_inputSocketModelName.CanGetResult()) return null;
		return ((AbstractStringNode) _inputSocketModelName.GetConnectedSocket().Parent).GetString(_inputSocketModelName.GetConnectedSocket());
	}

	public List<Vector3> GetPositions(float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed)
	{
		if (!_inputSocketPosition.CanGetResult()) return null;
		return AbstractVector3Node.GetInputVector3List(_inputSocketPosition, x, y, z, sizeX, sizeY, sizeZ, seed);
	}

	public override void Update()
	{

	}
}
