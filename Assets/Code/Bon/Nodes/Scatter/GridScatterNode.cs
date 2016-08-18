using Assets.Code.Bon;
using System;
using System.Collections.Generic;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;


[Serializable]
[GraphContextMenuItem("Scatter", "Grid")]
public class GridScatterNode : AbstractVector3Node
{

	private InputSocket _inputX;
	private InputSocket _inputZ;

	private Rect _tmpRect;

	public GridScatterNode(int id, Graph parent) : base(id, parent)
	{
		_inputX = new InputSocket(this, typeof(AbstractNumberNode));
		_inputZ = new InputSocket(this, typeof(AbstractNumberNode));

		_inputX.SetDirectInputNumber(5, false);
		_inputZ.SetDirectInputNumber(5, false);

		Sockets.Add(_inputX);
		Sockets.Add(_inputZ);

		Sockets.Add(new OutputSocket(this, typeof(AbstractVector3Node)));

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
		GUI.Label(_tmpRect, "scale z");
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
	}

	public override void Update()
	{

	}

	public override List<Vector3> GetVector3List(OutputSocket s, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed)
	{
		float left = x;
		float right = x + sizeX;

		if (sizeX < 0)
		{
			left = x + sizeX;
			right = x;
		}

		left = (float) Math.Floor(left);
		right = (float) Math.Ceiling(right);

		float bottom = z;
		float top = z + sizeZ;

		if (sizeZ < 0)
		{
			bottom = z + sizeZ;
			top = z;
		}

		bottom = (float) Math.Floor(bottom);
		top = (float) Math.Ceiling(top);

		float scaleX = AbstractNumberNode.GetInputNumber(_inputX, x, y, z, seed);
		float scaleZ = AbstractNumberNode.GetInputNumber(_inputZ, x, y, z, seed);


		List<Vector3> positions = new List<Vector3>();
		for (int leftIndex = (int) Math.Floor(left / scaleX); leftIndex <= (int) Math.Ceiling(right / scaleX); leftIndex++)
		{
			for (int bottomIndex = (int) Math.Floor(bottom / scaleZ); bottomIndex <= (int) Math.Ceiling(top / scaleZ); bottomIndex++)
			{
				if (leftIndex * scaleX >= left && leftIndex * scaleX < right)
				{
					if (bottomIndex * scaleZ >= bottom && bottomIndex * scaleZ < top)
					{
						positions.Add(new Vector3(leftIndex * scaleX, 0, bottomIndex * scaleZ));
					}
				}
			}
		}
		return positions;
	}
}
