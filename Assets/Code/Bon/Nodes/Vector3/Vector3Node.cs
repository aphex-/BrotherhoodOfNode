using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Vector3")]
	public class Vector3Node : AbstractVector3Node
	{

		private Socket _inputX;
		private Socket _inputY;
		private Socket _inputZ;

		private Rect _tmpRect;

		public Vector3Node(int id, Graph parent) : base(id, parent)
		{
			_tmpRect = new Rect();

			_inputX = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputY = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputZ = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);


			Sockets.Add(_inputX);
			Sockets.Add(_inputY);
			Sockets.Add(_inputZ);

			Sockets.Add(new Socket(this, typeof(AbstractVector3Node), SocketDirection.Output));

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

		public override object GetResultOf(Socket outSocket)
		{
			return GetVector3DList(_x, _y, _z, _width, _height, _depth, _seed);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public override List<UnityEngine.Vector3> GetVector3DList(float x, float y, float z, float width, float height, float depth, float seed)
		{
			float valueX = AbstractNumberNode.GetInputNumber(_inputX, x, y, seed);
			float valueY = AbstractNumberNode.GetInputNumber(_inputY, x, y, seed);
			float valueZ = AbstractNumberNode.GetInputNumber(_inputZ, x, y, seed);
			List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();
			positions.Add(new UnityEngine.Vector3(valueX, valueY, valueZ));
			return positions;
		}
	}
}
