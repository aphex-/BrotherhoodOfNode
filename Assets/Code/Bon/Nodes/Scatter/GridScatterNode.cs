using Assets.Code.Bon;
using System;
using System.Collections.Generic;
using Assets.Code.Bon.Nodes;
using UnityEngine;


[Serializable]
[GraphContextMenuItem("Scatter", "Grid")]
public class GridScatterNode : AbstractVector3Node
{

	private Socket _inputX;
	private Socket _inputY;

	private Rect _tmpRect;

	public GridScatterNode(int id, Graph parent) : base(id, parent)
	{
		_inputX = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		_inputY = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);

		_inputX.SetDirectInputNumber(5, false);
		_inputY.SetDirectInputNumber(5, false);

		Sockets.Add(_inputX);
		Sockets.Add(_inputY);

		Sockets.Add(new Socket(this, typeof(AbstractVector3Node), SocketDirection.Output));

		_tmpRect = new Rect();

		Height = 60;
		Width = 50;
	}

	public override void OnGUI()
	{
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		_tmpRect.Set(3, 0, 50, 20);
		GUI.Label(_tmpRect, "scale x");
		_tmpRect.Set(3, 20, 50, 20);
		GUI.Label(_tmpRect, "scale y");
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
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
		return true;
	}

	public override List<Vector3> GetVector3List(float x, float y, float z, float width, float height, float depth, float seed)
	{
		float left = x;
		float right = x + width;

		if (width < 0)
		{
			left = x + width;
			right = x;
		}

		left = (float) Math.Floor(left);
		right = (float) Math.Ceiling(right);

		float bottom = y;
		float top = y + height;

		if (height < 0)
		{
			bottom = y + height;
			top = y;
		}

		bottom = (float) Math.Floor(bottom);
		top = (float) Math.Ceiling(top);

		float scaleX = AbstractNumberNode.GetInputNumber(_inputX, x, y, seed);
		float scaleY = AbstractNumberNode.GetInputNumber(_inputY, x, y, seed);


		List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();
		for (int leftIndex = (int) Math.Floor(left / scaleX); leftIndex <= (int) Math.Ceiling(right / scaleX); leftIndex++)
		{
			for (int bottomIndex = (int) Math.Floor(bottom / scaleY); bottomIndex <= (int) Math.Ceiling(top / scaleY); bottomIndex++)
			{
				if (leftIndex * scaleX >= left && leftIndex * scaleX < right)
				{
					if (bottomIndex * scaleY >= bottom && bottomIndex * scaleY < top)
					{
						positions.Add(new Vector3(leftIndex * scaleX, bottomIndex * scaleY, 0));
					}
				}
			}
		}
		return positions;
	}
}
