using System;
using System.Collections.Generic;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Geometry", "Model")]
public class ModelNode : Node
{
	private Socket _inputSocketPosition;
	private Socket _inputSocketRotation;
	private Socket _inputSocketScale;

	private Socket _inputSocketModelName;

	private Rect _tmpRect;

	public ModelNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocketPosition = new Socket(this, typeof(AbstractVector3Node), SocketDirection.Input);
		_inputSocketRotation = new Socket(this, typeof(AbstractVector3Node), SocketDirection.Input);
		_inputSocketScale = new Socket(this, typeof(AbstractVector3Node), SocketDirection.Input);
		_inputSocketModelName = new Socket(this, typeof(AbstractStringNode), SocketDirection.Input);

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
		return ((AbstractStringNode) _inputSocketModelName.GetConnectedSocket().Parent).GetString();
	}

	public List<Vector3> GetPositions(float x, float y, float width, float depth, float seed)
	{
		if (!_inputSocketPosition.CanGetResult()) return null;
		return AbstractVector3Node.GetInputVector3List(_inputSocketPosition, x, y, 0, width, 0, depth, seed);
	}

	public override void Update()
	{

	}

	public override object GetResultOf(Socket outSocket)
	{
		return null;
	}

	public override bool CanGetResultOf(Socket outSocket)
	{
		return false;
	}
}
