using System;
using System.Collections.Generic;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Vector3")]
	public class Vector3Node : AbstractVector3Node
	{

		private InputSocket _inputX;
		private InputSocket _inputY;
		private InputSocket _inputZ;

		private Rect _tmpRect;

		public Vector3Node(int id, Graph parent) : base(id, parent)
		{
			_tmpRect = new Rect();

			_inputX = new InputSocket(this, typeof(AbstractNumberNode));
			_inputY = new InputSocket(this, typeof(AbstractNumberNode));
			_inputZ = new InputSocket(this, typeof(AbstractNumberNode));


			Sockets.Add(_inputX);
			Sockets.Add(_inputY);
			Sockets.Add(_inputZ);

			Sockets.Add(new OutputSocket(this, typeof(AbstractVector3Node)));

			Width = 60;
			Height = 84;
		}

		public override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "x");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "y");
			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "z");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, float x, float y, float z,
			float sizeX, float sizeY, float sizeZ, float seed)
		{
			float valueX = AbstractNumberNode.GetInputNumber(_inputX, x, y, z, seed);
			float valueY = AbstractNumberNode.GetInputNumber(_inputY, x, y, z, seed);
			float valueZ = AbstractNumberNode.GetInputNumber(_inputZ, x, y, z, seed);
			List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();
			positions.Add(new UnityEngine.Vector3(valueX, valueY, valueZ));
			return positions;
		}
	}
}
